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

namespace Fasterflect
{
    /// <summary>
    /// The Default API to use Fasterflect.  This API comprises of extensions
    /// methods for <see cref="Type"/> and <see cref="object"/>.  
    /// 
    /// This API is the wrapper for the core API available in <see cref="Reflector"/> class
    /// A single instance of <see cref="Reflector"/> is used internally to receive all invocations.  
    /// 
    /// This API is the recommended API.  Check the documentation of <see cref="Reflector"/>
    /// to see when that API makes sense.
    /// </summary>
    /// <seealso cref="Reflector"/>
    public static class ExtensionApi
    {
        private static readonly Reflector Reflector = Reflector.Create();

        #region Property
        /// <summary>
        /// Sets the static properties of <paramref name="targetType"/> based on
        /// the properties available in <paramref name="sample"/>. 
        /// </summary>
        /// <param name="targetType">The type whose static properties are to be set.</param>
        /// <param name="sample">An object whose properties will be used to set the static properties of
        /// <paramref name="targetType"/>.</param>
        /// <returns>The type whose static properties are to be set.</returns>
        public static Type SetProperties(this Type targetType, object sample)
        {
            Reflector.SetProperties(targetType, sample);
            return targetType;
        }

        /// <summary>
        /// Sets the properties of <paramref name="target"/> based on
        /// the properties available in <paramref name="sample"/>. 
        /// </summary>
        /// <param name="target">The object whose properties are to be set.</param>
        /// <param name="sample">An object whose properties will be used to set the properties of
        /// <paramref name="target"/>.</param>
        /// <returns>The object whose properties are to be set.</returns>
        public static object SetProperties(this object target, object sample)
        {
            Reflector.SetProperties(target, sample);
            return target;
        }

        /// <summary>
        /// Sets the static property <paramref name="propertyName"/> of type <paramref name="targetType"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <param name="targetType">The type whose static property is to be set.</param>
        /// <param name="propertyName">The name of the static property to be set.</param>
        /// <param name="value">The value used to set the static property.</param>
        /// <returns>The type whose static property is to be set.</returns>
        public static Type SetProperty(this Type targetType, string propertyName, object value)
        {
            Reflector.SetProperty(targetType, propertyName, value);
            return targetType;
        }

        /// <summary>
        /// Sets the property <paramref name="propertyName"/> of object <paramref name="target"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <param name="target">The object whose property is to be set.</param>
        /// <param name="propertyName">The name of the property to be set.</param>
        /// <param name="value">The value used to set the property.</param>
        /// <returns>The object whose property is to be set.</returns>
        public static object SetProperty(this object target, string propertyName, object value)
        {
            Reflector.SetProperty(target, propertyName, value);
            return target;
        }

        /// <summary>
        /// Gets the value of the static property <paramref name="propertyName"/> of type 
        /// <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type whose static property is to be retrieved.</param>
        /// <param name="propertyName">The name of the static property whose value is to be retrieved.</param>
        /// <returns>The value of the static property.</returns>
        public static TReturn GetProperty<TReturn>(this Type targetType, string propertyName)
        {
            return Reflector.GetProperty<TReturn>(targetType, propertyName);
        }

        /// <summary>
        /// Retrieves the value of the property <paramref name="propertyName"/> of object
        /// <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The object whose property is to be retrieved.</param>
        /// <param name="propertyName">The name of the property whose value is to be retrieved.</param>
        /// <returns>The value of the property.</returns>
        public static TReturn GetProperty<TReturn>(this object target, string propertyName)
        {
            return Reflector.GetProperty<TReturn>(target, propertyName);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the specified static property.
        /// </summary>
        /// <param name="targetType">The type which the static property belongs to.</param>
        /// <param name="propertyName">The name of the static property to be set.</param>
        /// <returns>A delegate which can set the value of the specified static property.</returns>
        public static StaticAttributeSetter DelegateForSetStaticProperty(this Type targetType, string propertyName)
        {
            return Reflector.DelegateForSetStaticProperty(targetType, propertyName);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the specified property.
        /// </summary>
        /// <param name="targetType">The type which the property belongs to.</param>
        /// <param name="propertyName">The name of the property to be set.</param>
        /// <returns>A delegate which can set the value of the specified property.</returns>
        public static AttributeSetter DelegateForSetProperty(this Type targetType, string propertyName)
        {
            return Reflector.DelegateForSetProperty(targetType, propertyName);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the specified static property.
        /// </summary>
        /// <param name="targetType">The type which the static property belongs to.</param>
        /// <param name="propertyName">The name of the static property to be retrieved.</param>
        /// <returns>A delegate which can get the value of the specified static property.</returns>
        public static StaticAttributeGetter DelegateForGetStaticProperty(this Type targetType, string propertyName)
        {
            return Reflector.DelegateForGetStaticProperty(targetType, propertyName);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the specified property.
        /// </summary>
        /// <param name="targetType">The type which the property belongs to.</param>
        /// <param name="propertyName">The name of the property to be retrieved.</param>
        /// <returns>A delegate which can get the value of the specified property.</returns>
        public static AttributeGetter DelegateForGetProperty(this Type targetType, string propertyName)
        {
            return Reflector.DelegateForGetProperty(targetType, propertyName);
        }
        #endregion

        #region Field
        /// <summary>
        /// Sets the static fields of <paramref name="targetType"/> based on
        /// the properties available in <paramref name="sample"/>. 
        /// </summary>
        /// <param name="targetType">The type whose static fields are to be set.</param>
        /// <param name="sample">An object whose properties will be used to set the static fields of
        /// <paramref name="targetType"/>.</param>
        /// <returns>The type whose static fields are to be set.</returns>
        public static Type SẹtFields(this Type targetType, object sample)
        {
            Reflector.SetFields(targetType, sample);
            return targetType;
        }

        /// <summary>
        /// Sets the fields of <paramref name="target"/> based on
        /// the properties available in <paramref name="sample"/>. 
        /// </summary>
        /// <param name="target">The object whose fields are to be set.</param>
        /// <param name="sample">An object whose fields will be used to set the properties of
        /// <paramref name="target"/>.</param>
        /// <returns>The object whose fields are to be set.</returns>
        public static object SetFields(this object target, object sample)
        {
            Reflector.SetFields(target, sample);
            return target;
        }

        /// <summary>
        /// Sets the static field <paramref name="fieldName"/> of type <paramref name="targetType"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <param name="targetType">The type whose sttic field is to be set.</param>
        /// <param name="fieldName">The name of the static field to be set.</param>
        /// <param name="value">The value used to set the static field.</param>
        /// <returns>The type whose sttic field is to be set.</returns>
        public static Type SetField(this Type targetType, string fieldName, object value)
        {
            Reflector.SetField(targetType, fieldName, value);
            return targetType;
        }

        /// <summary>
        /// Sets the field <paramref name="fieldName"/> of object <paramref name="target"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <param name="target">The object whose field is to be set.</param>
        /// <param name="fieldName">The name of the field to be set.</param>
        /// <param name="value">The value used to set the field.</param>
        /// <returns>The object whose field is to be set.</returns>
        public static object SetField(this object target, string fieldName, object value)
        {
            Reflector.SetField(target, fieldName, value);
            return target;
        }

        /// <summary>
        /// Gets the value of the static field <paramref name="fieldName"/> of type 
        /// <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type whose static field is to be retrieved.</param>
        /// <param name="fieldName">The name of the static field whose value is to be retrieved.</param>
        /// <returns>The value of the static field.</returns>
        public static TReturn GetField<TReturn>(this Type targetType, string fieldName)
        {
            return Reflector.GetField<TReturn>(targetType, fieldName);
        }

        /// <summary>
        /// Retrieves the value of the field <paramref name="fieldName"/> of object
        /// <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The object whose field is to be retrieved.</param>
        /// <param name="fieldName">The name of the field whose value is to be retrieved.</param>
        /// <returns>The value of the field.</returns>
        public static TReturn GetField<TReturn>(this object target, string fieldName)
        {
            return Reflector.GetField<TReturn>(target, fieldName);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the specified static field.
        /// </summary>
        /// <param name="targetType">The type which the static field belongs to.</param>
        /// <param name="fieldName">The name of the static field to be set.</param>
        /// <returns>A delegate which can set the value of the specified static field.</returns>
        public static StaticAttributeSetter DelegateForSetStaticField(this Type targetType, string fieldName)
        {
            return Reflector.DelegateForSetStaticField(targetType, fieldName);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the specified field.
        /// </summary>
        /// <param name="targetType">The type which the field belongs to.</param>
        /// <param name="fieldName">The name of the field to be set.</param>
        /// <returns>A delegate which can set the value of the specified field.</returns>
        public static AttributeSetter DelegateForSetField(this Type targetType, string fieldName)
        {
            return Reflector.DelegateForSetField(targetType, fieldName);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the specified static field.
        /// </summary>
        /// <param name="targetType">The type which the static field belongs to.</param>
        /// <param name="fieldName">The name of the static field to be retrieved.</param>
        /// <returns>A delegate which can get the value of the specified static field.</returns>
        public static StaticAttributeGetter DelegateForGetStaticField(this Type targetType, string fieldName)
        {
            return Reflector.DelegateForGetStaticField(targetType, fieldName);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the specified field.
        /// </summary>
        /// <param name="targetType">The type which the field belongs to.</param>
        /// <param name="fieldName">The name of the field to be retrieved.</param>
        /// <returns>A delegate which can get the value of the specified field.</returns>
        public static AttributeGetter DelegateForGetField(this Type targetType, string fieldName)
        {
            return Reflector.DelegateForGetField(targetType, fieldName);
        }
        #endregion

        #region Method
        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type
        /// <paramref name="targetType"/>.  Use this overload when the static method has no return type or 
        /// developers are not interested in the return value.
        /// </summary>
        /// <param name="targetType">The type whose static method is to be invoked.</param>
        /// <param name="methodName">The name of the static method to be invoked.</param>
        /// <returns>The type whose static method is to be invoked.</returns>
        public static Type Invoke(this Type targetType, string methodName)
        {
            Reflector.Invoke(targetType, methodName);
            return targetType;
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type
        /// <paramref name="targetType"/>.  Use this overload when the static method has no return type or 
        /// developers are not interested in the return value.
        /// </summary>
        /// <param name="targetType">The type whose static method is to be invoked.</param>
        /// <param name="methodName">The name of the static method to be invoked.</param>
        /// <param name="paramTypes">The types of the static method's parameters (must be in the right order).</param>
        /// <param name="parameters">The parameters of the static method (must be in the right order).</param>
        /// <returns>The type whose static method is to be invoked.</returns>
        public static Type Invoke(this Type targetType, string methodName, Type[] paramTypes, object[] parameters)
        {
            Reflector.Invoke(targetType, methodName, paramTypes, parameters);
            return targetType;
        }

        /// <summary>
        /// Invokes the method specified by <paramref name="methodName"/> of object
        /// <paramref name="target"/>.  Use this overload when the method has no return type or 
        /// developers are not interested in the return value.
        /// </summary>
        /// <param name="target">The object whose method is to be invoked.</param>
        /// <param name="methodName">The name of the method to be invoked.</param>
        /// <returns>The object whose method is to be invoked.</returns>
        public static object Invoke(this object target, string methodName)
        {
            Reflector.Invoke(target, methodName);
            return target;
        }

        /// <summary>
        /// Invokes the method specified by <paramref name="methodName"/> of object
        /// <paramref name="target"/>.  Use this overload when the method has no return type or 
        /// developers are not interested in the return value.
        /// </summary>
        /// <param name="target">The object whose method is to be invoked.</param>
        /// <param name="methodName">The name of the method to be invoked.</param>
        /// <param name="paramTypes">The types of the method parameters (must be in the right order).</param>
        /// <param name="parameters">The parameters of the method (must be in the right order).</param>
        /// <returns>The object whose method is to be invoked.</returns>
        public static object Invoke(this object target, string methodName, Type[] paramTypes, object[] parameters)
        {
            Reflector.Invoke(target, methodName, paramTypes, parameters);
            return target;
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type <paramref name="targetType"/>
        /// and get back the return value, casted to <typeparamref name="TReturn"/>.
        /// </summary>
        /// <typeparam name="TReturn">The return type of the static method.</typeparam>
        /// <param name="targetType">The type whose static method is to be invoked.</param>
        /// <param name="methodName">The name of the static method to be invoked.</param>
        /// <returns>The return value of the static method.</returns>
        public static TReturn Invoke<TReturn>(this Type targetType, string methodName)
        {
            return Reflector.Invoke<TReturn>(targetType, methodName);
        }

        /// <summary>
        /// Invokes the method specified by <paramref name="methodName"/> of object <paramref name="target"/>
        /// and get back the return value, casted to <typeparamref name="TReturn"/>.
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method.</typeparam>
        /// <param name="target">The object whose method is to be invoked.</param>
        /// <param name="methodName">The name of the method to be invoked.</param>
        /// <returns>The return value of the method.</returns>
        public static TReturn Invoke<TReturn>(this object target, string methodName)
        {
            return Reflector.Invoke<TReturn>(target, methodName);
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type <paramref name="targetType"/>
        /// and get back the return value, casted to <typeparamref name="TReturn"/>
        /// </summary>
        /// <typeparam name="TReturn">The return type of the static method.</typeparam>
        /// <param name="targetType">The type whose static method is to be invoked.</param>
        /// <param name="methodName">The name of the static method to be invoked.</param>
        /// <param name="paramTypes">The types of the static method's parameters (must be in the right order).</param>
        /// <param name="parameters">The parameters of the static method (must be in the right order).</param>
        /// <returns>The return value of the static method.</returns>
        public static TReturn Invoke<TReturn>(this Type targetType, string methodName, Type[] paramTypes, object[] parameters)
        {
            return Reflector.Invoke<TReturn>(targetType, methodName, paramTypes, parameters);
        }

        /// <summary>
        /// Invokes the method specified by <paramref name="methodName"/> of object <paramref name="target"/>
        /// and get back the return value, casted to <typeparamref name="TReturn"/>
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method.</typeparam>
        /// <param name="target">The object whose method is to be invoked.</param>
        /// <param name="methodName">The name of the method to be invoked.</param>
        /// <param name="paramTypes">The types of the method parameters (must be in the right order).</param>
        /// <param name="parameters">The parameters of the method (must be in the right order).</param>
        /// <returns>The return value of the method.</returns>
        public static TReturn Invoke<TReturn>(this object target, string methodName, Type[] paramTypes, object[] parameters)
        {
            return Reflector.Invoke<TReturn>(target, methodName, paramTypes, parameters);
        }

        /// <summary>
        /// Creates a delegate which can invoke the specified static method.
        /// </summary>
        /// <param name="targetType">The type which the static method belongs to.</param>
        /// <param name="methodName">The name of the static method to be invoked.</param>
        /// <returns>A delegate which can invoke the specified static method.</returns>
        public static StaticMethodInvoker DelegateForStaticInvoke(this Type targetType, string methodName)
        {
            return Reflector.DelegateForStaticInvoke(targetType, methodName);
        }

        /// <summary>
        /// Creates a delegate which can invoke the specified static method.
        /// </summary>
        /// <param name="targetType">The type which the static method belongs to.</param>
        /// <param name="methodName">The name of the static method to be invoked.</param>
        /// <param name="paramTypes">The types of the static method's parameters (must be in the right order).</param>
        /// <returns>A delegate which can invoke the specified static method.</returns>
        public static StaticMethodInvoker DelegateForStaticInvoke(this Type targetType, string methodName,
            Type[] paramTypes)
        {
            return Reflector.DelegateForStaticInvoke(targetType, methodName, paramTypes);
        }

        /// <summary>
        /// Creates a delegate which can invoke the specified method.
        /// </summary>
        /// <param name="targetType">The type which the method belongs to.</param>
        /// <param name="methodName">The name of the method to be invoked.</param>
        /// <returns>A delegate which can invoke the specified method.</returns>
        public static MethodInvoker DelegateForInvoke(this Type targetType, string methodName)
        {
            return Reflector.DelegateForInvoke(targetType, methodName);
        }

        /// <summary>
        /// Creates a delegate which can invoke the specified method.
        /// </summary>
        /// <param name="targetType">The type which the method belongs to.</param>
        /// <param name="methodName">The name of the method to be invoked.</param>
        /// <param name="paramTypes">The types of the method parameters (must be in the right order).</param>
        /// <returns>A delegate which can invoke the specified method.</returns>
        public static MethodInvoker DelegateForInvoke(this Type targetType, string methodName,
            Type[] paramTypes)
        {
            return Reflector.DelegateForInvoke(targetType, methodName, paramTypes);
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Sets the value of the indexer of object <paramref name="target"/>
        /// </summary>
        /// <param name="target">The object whose indexer is to be set.</param>
        /// <param name="paramTypes">The types of the indexer parameters (must be in the right order), plus
        /// the type of the indexer.</param>
        /// <param name="parameters">The list of the indexer parameters plus the value to be set to the indexer.
        /// This list must match with the <paramref name="paramTypes"/> list.</param>
        /// <returns>The object whose indexer is to be set.</returns>
        /// <example>
        /// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
        /// method should be invoked as follow:
        /// <code>
        /// target.SetIndexer(new Type[]{typeof(int), typeof(string)}, new object[]{1, "a"});
        /// </code>
        /// </example>
        public static object SetIndexer(this object target, Type[] paramTypes, object[] parameters)
        {
            Reflector.SetIndexer(target, paramTypes, parameters);
            return target;
        }

        /// <summary>
        /// Gets the value of the indexer of object <paramref name="target"/>
        /// </summary>
        /// <typeparam name="TReturn">The type of the indexer.</typeparam>
        /// <param name="target">The object whose indexer is to be retrieved.</param>
        /// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
        /// <param name="parameters">The list of the indexer parameters.</param>
        /// <returns>The value returned by the indexer.</returns>
        public static TReturn GetIndexer<TReturn>(this object target, Type[] paramTypes, object[] parameters)
        {
            return Reflector.GetIndexer<TReturn>(target, paramTypes, parameters);
        }

        /// <summary>
        /// Creates a delegate which can set an indexer
        /// </summary>
        /// <param name="targetType">The type which the indexer belongs to.</param>
        /// <param name="paramTypes">The types of the indexer parameters (must be in the right order), plus
        /// the type of the indexer.</param>
        /// <returns>A delegate which can set an indexer.</returns>
        /// <example>
        /// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
        /// method should be invoked as follow:
        /// <code>
        /// MethodInvoker invoker = type.DelegateForSetIndexer(new Type[]{typeof(int), typeof(string)});
        /// </code>
        /// </example>
        public static MethodInvoker DelegateForSetIndexer(this Type targetType, Type[] paramTypes)
        {
            return Reflector.DelegateForSetIndexer(targetType, paramTypes);
        }

        /// <summary>
        /// Creates a delegate which can get the value of an indexer.
        /// </summary>
        /// <param name="targetType">The type which the indexer belongs to.</param>
        /// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
        /// <returns>The delegate which can get the value of an indexer.</returns>
        public static MethodInvoker DelegateForGetIndexer(this Type targetType, Type[] paramTypes)
        {
            return Reflector.DelegateForGetIndexer(targetType, paramTypes);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Invokes the no-arg constructor on type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type whose constructor is to be invoked.</param>
        /// <returns>An instance of type <paramref name="targetType"/>.</returns>
        public static object Construct(this Type targetType)
        {
            return Reflector.Construct(targetType);
        }

        /// <summary>
        /// Invokes a constructor specified by <param name="parameters" /> on the type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type whose constructor is to be invoked.</param>
        /// <param name="paramTypes">The types of the constructor parameters (must be in the right order).</param>
        /// <param name="parameters">The parameters of the constructor (must be in the right order).</param>
        /// <returns>An instance of type <paramref name="targetType"/>.</returns>
        public static object Construct(this Type targetType, Type[] paramTypes, object[] parameters)
        {
            return Reflector.Construct(targetType, paramTypes, parameters);
        }

        /// <summary>
        /// Creates a delegate which can invoke the constructor of type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type which has the constructor to be invoked.</param>
        /// <returns>The delegate which can invoke the constructor of type <paramref name="targetType"/>.</returns>
        public static ConstructorInvoker DelegateForConstruct(this Type targetType)
        {
            return Reflector.DelegateForConstruct(targetType);
        }

        /// <summary>
        /// Creates a delegate which can invoke the constructor of type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type which has the constructor to be invoked.</param>
        /// <param name="paramTypes">The types of the constructor parameters (must be in the right order).</param>
        /// <returns>The delegate which can invoke the constructor of type <paramref name="targetType"/>.</returns>
        public static ConstructorInvoker DelegateForConstruct(this Type targetType, Type[] paramTypes)
        {
            return Reflector.DelegateForConstruct(targetType, paramTypes);
        }
        #endregion
    }
}
