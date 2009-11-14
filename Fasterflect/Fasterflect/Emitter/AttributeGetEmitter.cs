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
    internal class AttributeGetEmitter : AttributeEmitter
    {
        public AttributeGetEmitter(DelegateCache cache, Type targetType, 
            MemberTypes memberTypes, string fieldOrPropertyName, bool isStatic)
            : base(cache)
        {
            callInfo = new CallInfo(targetType, memberTypes, fieldOrPropertyName, 
                Type.EmptyTypes, isStatic);
        }

        protected override Delegate CreateDelegate()
        {
            MemberInfo member = GetAttribute();
            var method = callInfo.IsStatic 
                ? CreateDynamicMethod("getter", callInfo.TargetType, Constants.ObjectType, null)
                : CreateDynamicMethod("getter", callInfo.TargetType, Constants.ObjectType, new[] { Constants.ObjectType });

            ILGenerator generator = method.GetILGenerator();
            var handleInnerStruct = callInfo.ShouldHandleInnerStruct;

            if (handleInnerStruct)
            {
                generator.Emit(OpCodes.Ldarg_0); // arg0
                generator.DeclareLocal(callInfo.TargetType); // loc_0: T tmp;
                LoadInnerStructToLocal(generator, 0); // tmp = ((ValueTypeHolder)arg0)).Value;
                generator.DeclareLocal(Constants.ObjectType); // loc_1: T result;
            }
            else if (!callInfo.IsStatic)
            {
                generator.Emit(OpCodes.Ldarg_0); // arg0
                generator.Emit(OpCodes.Castclass, callInfo.TargetType); // (T)arg0
            }

            if (member.MemberType == MemberTypes.Field)
            {
                var field = member as FieldInfo;

                // (object)((T)arag0|tmp).field
                generator.Emit(callInfo.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);

                // (object)((T)arag0|tmp).field
                BoxIfValueType(generator, field.FieldType);
            }
            else
            {
                var prop = member as PropertyInfo;
                MethodInfo getMethod = GetPropertyGetMethod();

                // ((T)arag0|tmp).prop
                generator.Emit((callInfo.IsStatic || callInfo.IsTargetTypeStruct) 
                    ? OpCodes.Call : OpCodes.Callvirt, getMethod);

                // (object)((T)arag0|tmp).prop
                BoxIfValueType(generator, prop.PropertyType);
            }

            if (handleInnerStruct)
            {
                generator.Emit(OpCodes.Stloc_1); // result = <stack>
                StoreLocalToInnerStruct(generator, 0); // ((ValueTypeHolder)arg0)).Value = tmp; 
                generator.Emit(OpCodes.Ldloc_1); // push result
            }

            generator.Emit(OpCodes.Ret);

            return callInfo.IsStatic 
                ? method.CreateDelegate(typeof(StaticAttributeGetter))
                : method.CreateDelegate(typeof(AttributeGetter));
        }
    }
}
