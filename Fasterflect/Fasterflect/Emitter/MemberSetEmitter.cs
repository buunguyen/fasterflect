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
	internal class MemberSetEmitter : MemberEmitter
	{
		public MemberSetEmitter(Type targetType, MemberTypes memberTypes, string fieldOrProperty, bool isStatic)
		{
			callInfo = new CallInfo(targetType, memberTypes, fieldOrProperty,
			                        Constants.ArrayOfObjectType, isStatic);
		}

		protected override Delegate CreateDelegate()
		{
			MemberInfo member = GetMember();
			DynamicMethod method = callInfo.IsStatic
			                       	? CreateDynamicMethod("setter", callInfo.TargetType, null, new[] {Constants.ObjectType})
			                       	: CreateDynamicMethod("setter", callInfo.TargetType, null,
			                       	                      new[] {Constants.ObjectType, Constants.ObjectType});

			ILGenerator generator = method.GetILGenerator();
			bool handleInnerStruct = callInfo.ShouldHandleInnerStruct;

			generator.Emit(OpCodes.Ldarg_0); // arg0;
			if (handleInnerStruct)
			{
				generator.DeclareLocal(callInfo.TargetType); // loc_0: T tmp;
				LoadInnerStructToLocal(generator, 0); // tmp = ((ValueTypeHolder)arg0)).Value;
				generator.Emit(OpCodes.Ldarg_1); // arg1;
			}
			else if (!callInfo.IsStatic)
			{
				generator.Emit(OpCodes.Castclass, callInfo.TargetType); // (T)arg0
				generator.Emit(OpCodes.Ldarg_1); // arg1;
			}

			Type memberType = member is FieldInfo
			                  	? ((FieldInfo) member).FieldType
			                  	: ((PropertyInfo) member).PropertyType;
			UnboxOrCast(generator, memberType);
			if (member.MemberType == MemberTypes.Field)
			{
				// ((T)arg0).field = arg1;
				generator.Emit(callInfo.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, (FieldInfo) member);
			}
			else
			{
				MethodInfo setMethod = GetPropertySetMethod();

				// ((T)arg0).set_XXX(arg1);
				generator.Emit((callInfo.IsStatic || callInfo.IsTargetTypeStruct)
				               	? OpCodes.Call
				               	: OpCodes.Callvirt, setMethod);
			}

			if (handleInnerStruct)
			{
				StoreLocalToInnerStruct(generator, 0); // ((ValueTypeHolder)arg0)).Value = tmp; 
			}

			generator.Emit(OpCodes.Ret);

			return callInfo.IsStatic
			       	? method.CreateDelegate(typeof (StaticMemberSetter))
			       	: method.CreateDelegate(typeof (MemberSetter));
		}
	}
}