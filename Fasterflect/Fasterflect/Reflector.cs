#region License
// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/
#endregion

using System;
using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
    /// <summary>
    /// An low-level API for Fasterflect.  The extension method-based
    /// wrapper API, declared in <see cref="ExtensionApi"/>, should be used as the default API.  
    /// 
    /// You should only consider this API only if you need fine-grain control 
    /// of <see cref="Reflector"/> instances and their cache (e.g. use an instance 
    /// for a certain period and then have it garbaged collected to release
    /// memory of the cached methods).
    /// </summary>
    /// <seealso cref="ExtensionApi"/>
    public class Reflector
    {
        /// <summary>
        /// Used to cache all generated delegates in the declaring  <see cref="Reflector"/> instance.
        /// </summary>
        private readonly DelegateCache cache = new DelegateCache();
        
        private Reflector() { }

        /// <summary>
        /// Factory method to create an instance of <see cref="Reflector"/>.  
        /// Reflection invocations done through this instance are optimized in a way
        /// that subsequent invocations (of the same operation) happen faster.  This
        /// is possible by internal cache used to cache necessary invocation information.
        /// 
        /// Other instances of this type will have their own cache.  Therefore, consider
        /// reusing an instance created by this method for as long as it makes sense.
        /// </summary>
        /// <returns>An instance of <see cref="Reflector"/>.</returns>
        public static Reflector Create()
        {
            return new Reflector();
        }

        #region Batch Setters/Getters
        public Reflector SetProperties(Type targetType, object sample)
        {
            sample.GetProperties().ForEach(prop =>
                SetProperty(targetType, prop.Name, prop.GetValue(sample))
            );
            return this;
        }

        public Reflector SetProperties(object target, object sample)
        {
            sample.GetProperties().ForEach(prop =>
                SetProperty(target, prop.Name, prop.GetValue(sample))
            );
            return this;
        }

        public Reflector SetFields(object target, object sample)
        {
            sample.GetProperties().ForEach(prop =>
                SetField(target, prop.Name, prop.GetValue(sample))
            );
            return this;
        }

        public Reflector SetFields(Type targetType, object sample)
        {
            sample.GetProperties().ForEach(prop =>
                SetField(targetType, prop.Name, prop.GetValue(sample))
            );
            return this;
        }
        #endregion

        #region Static Setters
        public Reflector SetProperty(Type targetType, string propertyName, object value)
        {
            return SetFieldOrProperty(targetType, MemberTypes.Property, propertyName, value);
        }

        public StaticAttributeSetter DelegateForSetStaticProperty(Type targetType, string propertyName)
        {
            return DelegateForSetStaticFieldOrProperty(targetType, MemberTypes.Property, propertyName);
        }

        public Reflector SetField(Type targetType, string fieldName, object value)
        {
            return SetFieldOrProperty(targetType, MemberTypes.Field, fieldName, value);
        }

        public StaticAttributeSetter DelegateForSetStaticField(Type targetType, string fieldName)
        {
            return DelegateForSetStaticFieldOrProperty(targetType, MemberTypes.Field, fieldName);
        }

        private Reflector SetFieldOrProperty(Type targetType, MemberTypes memberTypes,
            string fieldOrProperty, object value)
        {
            var callInfo = ReflectorUtils.CreateStaticSetCallInfo(targetType, memberTypes, 
                fieldOrProperty, value);
            new AttributeSetEmitter(callInfo, cache).Execute();
            return this;
        }

        private StaticAttributeSetter DelegateForSetStaticFieldOrProperty(Type targetType, 
            MemberTypes memberTypes,
            string fieldOrProperty)
        {
            var callInfo = ReflectorUtils.CreateStaticSetCallInfo(targetType, memberTypes, 
                fieldOrProperty, null);
            return (StaticAttributeSetter)new AttributeSetEmitter(callInfo, cache).GetDelegate();
        }
        #endregion

        #region Static Getters
        public TReturn GetProperty<TReturn>(Type targetType, string propertyName)
        {
            return GetFieldOrProperty<TReturn>(targetType, MemberTypes.Property, propertyName);
        }

        public StaticAttributeGetter DelegateForGetStaticProperty(Type targetType, string propertyName)
        {
            return DelegateForGetStaticFieldOrProperty(targetType, MemberTypes.Property, propertyName);
        }

        public TReturn GetField<TReturn>(Type targetType, string fieldName)
        {
            return GetFieldOrProperty<TReturn>(targetType, MemberTypes.Field, fieldName);
        }

        public StaticAttributeGetter DelegateForGetStaticField(Type targetType, string fieldName)
        {
            return DelegateForGetStaticFieldOrProperty(targetType, MemberTypes.Field, fieldName);
        }

        private TReturn GetFieldOrProperty<TReturn>(Type targetType, MemberTypes memberTypes, 
            string fieldOrPropertyName)
        {
            var callInfo = ReflectorUtils.CreateStaticGetCallInfo(targetType, memberTypes, fieldOrPropertyName);
            return new AttributeGetEmitter(callInfo, cache).Execute<TReturn>();
        }

        private StaticAttributeGetter DelegateForGetStaticFieldOrProperty(Type targetType, MemberTypes memberTypes,
            string fieldOrPropertyName)
        {
            var callInfo = ReflectorUtils.CreateStaticGetCallInfo(targetType, memberTypes, fieldOrPropertyName);
            return (StaticAttributeGetter)new AttributeGetEmitter(callInfo, cache).GetDelegate();
        }
        #endregion

        #region Instance Setters
        public Reflector SetProperty(object target, string propertyName, object value)
        {
            return SetFieldOrProperty(target, MemberTypes.Property, propertyName, value);
        }

        public AttributeSetter DelegateForSetProperty(Type targetType, string propertyName)
        {
            return DelegateForSetFieldOrProperty(targetType, MemberTypes.Property, propertyName);
        }

        public Reflector SetField(object target, string fieldName, object value)
        {
            return SetFieldOrProperty(target, MemberTypes.Field, fieldName, value);
        }

        public AttributeSetter DelegateForSetField(Type targetType, string propertyName)
        {
            return DelegateForSetFieldOrProperty(targetType, MemberTypes.Field, propertyName);
        }

        private Reflector SetFieldOrProperty(object target, MemberTypes memberTypes,
            string fieldOrProperty, object value)
        {
            var callInfo = ReflectorUtils.CreateSetCallInfo(target.GetType(), target,
                memberTypes, fieldOrProperty, value);
            new AttributeSetEmitter(callInfo, cache).Execute();
            return this;
        }

        private AttributeSetter DelegateForSetFieldOrProperty(Type targetType, MemberTypes memberTypes,
            string fieldOrProperty)
        {
            var callInfo = ReflectorUtils.CreateSetCallInfo(targetType, null, memberTypes, fieldOrProperty, null);
            return (AttributeSetter)new AttributeSetEmitter(callInfo, cache).GetDelegate();
        }
        #endregion

        #region Instance Getters
        public TReturn GetProperty<TReturn>(object target, string propertyName)
        {
            return GetFieldOrProperty<TReturn>(target, MemberTypes.Property, propertyName);
        }

        public AttributeGetter DelegateForGetProperty(Type targetType, string propertyName)
        {
            return DelegateForGetFieldOrProperty(targetType, MemberTypes.Property, propertyName);
        }

        public TReturn GetField<TReturn>(object target, string fieldName)
        {
            return GetFieldOrProperty<TReturn>(target, MemberTypes.Field, fieldName);
        }

        public AttributeGetter DelegateForGetField(Type targetType, string fieldName)
        {
            return DelegateForGetFieldOrProperty(targetType, MemberTypes.Field, fieldName);
        }

        private TReturn GetFieldOrProperty<TReturn>(object target, MemberTypes memberTypes, string fieldOrPropertyName)
        {
            var callInfo = ReflectorUtils.CreateGetCallInfo(target.GetType(), target, memberTypes, fieldOrPropertyName);
            return new AttributeGetEmitter(callInfo, cache).Execute<TReturn>();
        }

        private AttributeGetter DelegateForGetFieldOrProperty(Type targetType, MemberTypes memberTypes, string fieldOrPropertyName)
        {
            var callInfo = ReflectorUtils.CreateGetCallInfo(targetType, null, memberTypes, fieldOrPropertyName);
            return (AttributeGetter)new AttributeGetEmitter(callInfo, cache).GetDelegate();
        }
        #endregion

        #region Static Methods
        public Reflector Invoke(Type targetType, string methodName)
        {
            return Invoke(targetType, methodName, Type.EmptyTypes,
                ReflectorUtils.EmptyObjectArray);
        }

        public Reflector Invoke(Type targetType, string methodName, Type[] paramTypes, object[] parameters)
        {
            var callInfo = ReflectorUtils.CreateStaticMethodCallInfo(targetType, methodName, paramTypes, parameters);
            new MethodInvocationEmitter(callInfo, cache).Execute();
            return this;
        }

        public TReturn Invoke<TReturn>(Type targetType, string methodName)
        {
            return Invoke<TReturn>(targetType, methodName, Type.EmptyTypes, ReflectorUtils.EmptyObjectArray);
        }

        public TReturn Invoke<TReturn>(Type targetType, string methodName, Type[] paramTypes, object[] parameters)
        {
            var callInfo = ReflectorUtils.CreateStaticMethodCallInfo(targetType, methodName, paramTypes, parameters);
            return new MethodInvocationEmitter(callInfo, cache).Execute<TReturn>();
        }

        public StaticMethodInvoker DelegateForStaticInvoke(Type targetType, string methodName)
        {
            return DelegateForStaticInvoke(targetType, methodName, Type.EmptyTypes);
        }

        public StaticMethodInvoker DelegateForStaticInvoke(Type targetType, string methodName, Type[] paramTypes)
        {
            var callInfo = ReflectorUtils.CreateStaticMethodCallInfo(targetType, methodName,
                paramTypes, null);
            return (StaticMethodInvoker)new MethodInvocationEmitter(callInfo, cache).GetDelegate();
        }
        #endregion

        #region Instance Methods
        public Reflector Invoke(object target, string methodName)
        {
            return Invoke(target, methodName, Type.EmptyTypes,
                ReflectorUtils.EmptyObjectArray);
        }

        public Reflector Invoke(object target, string methodName, Type[] paramTypes, object[] parameters)
        {
            var callInfo = ReflectorUtils.CreateMethodCallInfo(target.GetType(), target, methodName, paramTypes, parameters);
            new MethodInvocationEmitter(callInfo, cache).Execute();
            return this;
        }

        public TReturn Invoke<TReturn>(object target, string methodName)
        {
            return Invoke<TReturn>(target, methodName, Type.EmptyTypes, ReflectorUtils.EmptyObjectArray);
        }

        public TReturn Invoke<TReturn>(object target, string methodName, Type[] paramTypes, object[] parameters)
        {
            var callInfo = ReflectorUtils.CreateMethodCallInfo(target.GetType(), target, methodName, paramTypes, parameters);
            return new MethodInvocationEmitter(callInfo, cache).Execute<TReturn>();
        }

        public MethodInvoker DelegateForInvoke(Type targetType, string methodName)
        {
            return DelegateForInvoke(targetType, methodName, Type.EmptyTypes);
        }

        public MethodInvoker DelegateForInvoke(Type targetType, string methodName, Type[] paramTypes)
        {
            var callInfo = ReflectorUtils.CreateMethodCallInfo(targetType, null, methodName, paramTypes, null);
            return (MethodInvoker)new MethodInvocationEmitter(callInfo, cache).GetDelegate();
        }
        #endregion

        #region Indexers
        public Reflector SetIndexer(object target, Type[] paramTypes, object[] parameters)
        {
            var callInfo = ReflectorUtils.CreateIndexerSetCallInfo(target.GetType(), target, paramTypes, parameters);
            new MethodInvocationEmitter(callInfo, cache).Execute();
            return this;
        }

        public TReturn GetIndexer<TReturn>(object target, Type[] paramTypes, object[] parameters)
        {
            var callInfo = ReflectorUtils.CreateIndexerGetCallInfo(target.GetType(), target, paramTypes, parameters);
            return new MethodInvocationEmitter(callInfo, cache).Execute<TReturn>();
        }

        public MethodInvoker DelegateForSetIndexer(Type targetType, Type[] paramTypes)
        {
            var callInfo = ReflectorUtils.CreateIndexerSetCallInfo(targetType, null, paramTypes, null);
            return (MethodInvoker)new MethodInvocationEmitter(callInfo, cache).GetDelegate();
        }

        public MethodInvoker DelegateForGetIndexer(Type targetType, Type[] paramTypes)
        {
            var callInfo = ReflectorUtils.CreateIndexerGetCallInfo(targetType, null, paramTypes, null);
            return (MethodInvoker)new MethodInvocationEmitter(callInfo, cache).GetDelegate();
        }
        #endregion

        #region Construction
        public object Construct(Type targetType)
        {
            return Construct(targetType, Type.EmptyTypes, ReflectorUtils.EmptyObjectArray);
        }

        public object Construct(Type targetType, Type[] paramTypes, object[] parameters)
        {
            var callInfo = ReflectorUtils.GetConstructorCallInfo(targetType, paramTypes, parameters);
            return new CtorInvocationEmitter(callInfo, cache).Execute<object>();
        }

        public ConstructorInvoker DelegateForConstruct(Type targetType)
        {
            return DelegateForConstruct(targetType, Type.EmptyTypes);
        }

        public ConstructorInvoker DelegateForConstruct(Type targetType, Type[] paramTypes)
        {
            var callInfo = ReflectorUtils.GetConstructorCallInfo(targetType, paramTypes, null);
            return (ConstructorInvoker)new CtorInvocationEmitter(callInfo, cache).GetDelegate();
        }
        #endregion
    }
}
