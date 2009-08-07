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
        public AttributeGetEmitter(CallInfo callInfo, DelegateCache cache)
            : base(callInfo, cache)
        {
        }

        protected override object Invoke(Delegate action)
        {
            if (callInfo.IsStatic)
            {
                var invocation = (StaticAttributeGetter)action;
                return invocation.Invoke();
            }
            else
            {
                var invocation = (AttributeGetter)action;
                return invocation.Invoke(callInfo.Target);
            }
        }

        protected override Delegate CreateDelegate()
        {
            MemberInfo member = GetAttribute(callInfo);
            var method = callInfo.IsStatic 
                ? CreateDynamicMethod("getter", callInfo.TargetType, ObjectType, null)
                : CreateDynamicMethod("getter", callInfo.TargetType, ObjectType, new[] { ObjectType });
            
            ILGenerator generator = method.GetILGenerator();

            if (!callInfo.IsStatic)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Castclass, callInfo.TargetType);
            }

            if (member.MemberType == MemberTypes.Field)
            {
                var field = member as FieldInfo;
                generator.Emit(callInfo.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);
                if (field.FieldType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, field.FieldType);
                }
            }
            else
            {
                var prop = member as PropertyInfo;
                MethodInfo getMethod = GetPropertyGetMethod();
                generator.Emit(callInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, getMethod);
                if (prop.PropertyType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, prop.PropertyType);
                }
            }
            generator.Emit(OpCodes.Ret);

            return callInfo.IsStatic 
                ? method.CreateDelegate(typeof(StaticAttributeGetter))
                : method.CreateDelegate(typeof(AttributeGetter));
        }
    }
}
