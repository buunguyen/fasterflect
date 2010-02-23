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
    internal class MemberSetEmitter : BaseEmitter
    {
        public MemberSetEmitter(MemberInfo memberInfo, Flags bindingFlags)
            : this(memberInfo.DeclaringType, bindingFlags, memberInfo.MemberType, memberInfo.Name, memberInfo)
        {
        }

		public MemberSetEmitter(Type targetType, Flags bindingFlags, MemberTypes memberType, string fieldOrProperty)
            : this(targetType, bindingFlags, memberType, fieldOrProperty, null)
		{
		}

        private MemberSetEmitter(Type targetType, Flags bindingFlags, MemberTypes memberType, string fieldOrProperty, MemberInfo memberInfo)
        {
            callInfo = new CallInfo(targetType, bindingFlags, memberType, fieldOrProperty, Constants.ArrayOfObjectType, memberInfo);
        }

        protected internal override DynamicMethod CreateDynamicMethod()
        {
            return callInfo.IsStatic
                                    ? CreateDynamicMethod("setter", callInfo.TargetType, null, new[] { Constants.ObjectType })
                                    : CreateDynamicMethod("setter", callInfo.TargetType, null,
                                                          new[] { Constants.ObjectType, Constants.ObjectType });
        }

		protected internal override Delegate CreateDelegate()
		{
            MemberInfo member = LookupUtils.GetMember(callInfo);
			bool handleInnerStruct = callInfo.ShouldHandleInnerStruct;

			generator.ldarg_0.end();                            // load arg-0 (this or value-to-be-set)
			if (handleInnerStruct)
			{
				generator.DeclareLocal(callInfo.TargetType);    // TargetType tmpStr
				LoadInnerStructToLocal(0);                      // tmpStr = ((ValueTypeHolder)this)).Value;
                generator.ldarg_1.end();                        // load value-to-be-set;
			}
			else if (!callInfo.IsStatic)
			{
                generator.castclass( callInfo.TargetType )      // (TargetType)this
				         .ldarg_1.end();                        // load value-to-be-set;
			}

			Type memberType = member is FieldInfo
			                  	? ((FieldInfo) member).FieldType
			                  	: ((PropertyInfo) member).PropertyType;
            generator.CastFromObject(memberType);               // unbox | cast value-to-be-set
			if (member.MemberType == MemberTypes.Field)
			{
                generator.stfld(callInfo.IsStatic, (FieldInfo)member);  // (this|tmpStr).field = value-to-be-set;
			}
			else
			{
				MethodInfo setMethod = LookupUtils.GetPropertySetMethod((PropertyInfo) member, callInfo);
                generator.call(callInfo.IsStatic || callInfo.IsTargetTypeStruct, setMethod); // (this|tmpStr).set_Prop(value-to-be-set);
			}

			if (handleInnerStruct)
			{
                StoreLocalToInnerStruct(0); // ((ValueTypeHolder)this)).Value = tmpStr
			}

		    generator.ret();

			return callInfo.IsStatic
			       	? method.CreateDelegate(typeof (StaticMemberSetter))
			       	: method.CreateDelegate(typeof (MemberSetter));
		}
	}
}