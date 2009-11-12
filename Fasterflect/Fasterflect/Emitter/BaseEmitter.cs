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
using System.Reflection.Emit;

namespace Fasterflect.Emitter
{
    internal abstract class BaseEmitter
    {
        protected CallInfo callInfo;
        protected DelegateCache cache;

        protected BaseEmitter(DelegateCache cache)
        {
            this.cache = cache;
        }

        public Delegate GetDelegate()
        {
            return cache.GetDelegate(callInfo, CreateDelegate);
        }

        protected abstract Delegate CreateDelegate();

        protected static DynamicMethod CreateDynamicMethod(string name, Type targetType, Type returnType, Type[] paramTypes)
        {
            return new DynamicMethod(name, MethodAttributes.Static | MethodAttributes.Public,
                                     CallingConventions.Standard, returnType, paramTypes,
                                     targetType.IsArray ? targetType.GetElementType() : targetType, 
                                     true);
        }

        protected BindingFlags ScopeFlag
        {
            get
            {
                return callInfo.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
            }
        }

        protected void LoadInnerStructToLocal(ILGenerator generator, int localPosition)
        {
            generator.Emit(OpCodes.Castclass, Constants.StructType);
            var getMethod = Constants.StructType.GetMethod("get_Value", BindingFlags.Public |
                                                                       BindingFlags.Instance);
            generator.Emit(OpCodes.Callvirt, getMethod);
            generator.Emit(OpCodes.Unbox_Any, callInfo.TargetType);
            generator.Emit(OpCodes.Stloc, localPosition);
            generator.Emit(OpCodes.Ldloca_S, localPosition);
        }

        protected void StoreLocalToInnerStruct(ILGenerator generator, int localPosition)
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, Constants.StructType);
            var setMethod = Constants.StructType.GetMethod("set_Value", BindingFlags.Public |
                                                                       BindingFlags.Instance);
            generator.Emit(OpCodes.Ldloc, localPosition);
            BoxIfValueType(generator, callInfo.TargetType);
            generator.Emit(OpCodes.Callvirt, setMethod);
        }

        protected void BoxIfValueType(ILGenerator generator, Type type)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
        }

        protected void UnboxOrCast(ILGenerator generator, Type type)
        {
            generator.Emit(type.IsValueType
                               ? OpCodes.Unbox_Any
                               : OpCodes.Castclass, type);
        }
    }
}
