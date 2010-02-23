#region License

// Copyright 2010 Buu Nguyen, Morten Mertner
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
	internal class MemberGetEmitter : BaseEmitter
    {
        public MemberGetEmitter(MemberInfo memberInfo, Flags bindingFlags)
            : this(memberInfo.DeclaringType, bindingFlags, memberInfo.MemberType, memberInfo.Name, memberInfo)
        {
        }

        public MemberGetEmitter(Type targetType, Flags bindingFlags, MemberTypes memberType, string fieldOrPropertyName)
            : this(targetType, bindingFlags, memberType, fieldOrPropertyName, null)
        {
        }

        private MemberGetEmitter(Type targetType, Flags bindingFlags, MemberTypes memberType, string fieldOrPropertyName, MemberInfo memberInfo)
		{
            callInfo = new CallInfo(targetType, bindingFlags, memberType, fieldOrPropertyName, Type.EmptyTypes, memberInfo);
		}

        protected internal override DynamicMethod CreateDynamicMethod()
        {
            return callInfo.IsStatic
                                    ? CreateDynamicMethod("getter", callInfo.TargetType, Constants.ObjectType, null)
                                    : CreateDynamicMethod("getter", callInfo.TargetType, Constants.ObjectType,
                                                          new[] { Constants.ObjectType });
        }

	    protected internal override Delegate CreateDelegate()
		{
			MemberInfo member = LookupUtils.GetMember(callInfo);
			bool handleInnerStruct = callInfo.ShouldHandleInnerStruct;

			if (handleInnerStruct)
			{
                generator.ldarg_0                               // load arg-0 (this)
                         .DeclareLocal(callInfo.TargetType);    // TargetType tmpStr
                LoadInnerStructToLocal(0);                      // tmpStr = ((ValueTypeHolder)this)).Value
				generator.DeclareLocal(Constants.ObjectType);   // object result;
			}
			else if (!callInfo.IsStatic)
			{
                generator.ldarg_0                               // load arg-0 (this)
				         .castclass( callInfo.TargetType);      // (TargetType)this
			}

			if (member.MemberType == MemberTypes.Field)
			{
				var field = member as FieldInfo;
                generator.ldfld(callInfo.IsStatic, field)       // (this|tmpStr).field OR TargetType.field
                         .boxIfValueType(field.FieldType);      // (object)<stack>
			}
			else
			{
				var prop = member as PropertyInfo;
                MethodInfo getMethod = LookupUtils.GetPropertyGetMethod(prop, callInfo);
                generator.call(callInfo.IsStatic || callInfo.IsTargetTypeStruct, getMethod) // (this|tmpStr).prop OR TargetType.prop
                         .boxIfValueType(prop.PropertyType);                                // (object)<stack>
			}

			if (handleInnerStruct)
			{
                generator.stloc_1.end();        // resultLocal = <stack>
				StoreLocalToInnerStruct(0);     // ((ValueTypeHolder)this)).Value = tmpStr
				generator.ldloc_1.end();        // push resultLocal
			}

	        generator.ret();

			return callInfo.IsStatic
			       	? method.CreateDelegate(typeof (StaticMemberGetter))
			       	: method.CreateDelegate(typeof (MemberGetter));
		}
	}
}