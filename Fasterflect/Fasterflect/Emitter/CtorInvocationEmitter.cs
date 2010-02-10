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
	internal class CtorInvocationEmitter : InvocationEmitter
    {
        public CtorInvocationEmitter(ConstructorInfo ctorInfo)
            : this(ctorInfo.DeclaringType, ctorInfo.GetParameters().GetTypeArray(), ctorInfo)
        {
        }

        public CtorInvocationEmitter(Type targetType, Type[] paramTypes)
            : this (targetType, paramTypes, null)
        {
        }

		private CtorInvocationEmitter(Type targetType, Type[] paramTypes, ConstructorInfo ctorInfo)
		{
            callInfo = new CallInfo(targetType, MemberTypes.Constructor, targetType.Name, paramTypes, false, ctorInfo);
		}

		protected internal override Delegate CreateDelegate()
		{
			DynamicMethod method = CreateDynamicMethod("ctor",
			                                           callInfo.TargetType, Constants.ObjectType, new[] {Constants.ObjectType});
			ILGenerator generator = method.GetILGenerator();

			if (callInfo.IsTargetTypeStruct && callInfo.HasNoParam)
			{
				generator.DeclareLocal(callInfo.TargetType); // loc_0: T tmp;
				generator.Emit(OpCodes.Ldloca_S, 0); // &tmp;
				generator.Emit(OpCodes.Initobj, callInfo.TargetType); // init_obj(&tmp);
				generator.Emit(OpCodes.Ldloc, 0); // tmp;;
			}
			else if (callInfo.TargetType.IsArray)
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Ldelem_Ref);
				generator.Emit(OpCodes.Unbox_Any, typeof (int));
				generator.Emit(OpCodes.Newarr, callInfo.TargetType.GetElementType());
			}
			else
			{
                ConstructorInfo ctorInfo = LookupUtils.GetConstructor(callInfo);
				if (callInfo.HasRefParam)
				{
					int byRefParamsCount = CreateLocalsForByRefParams(generator, 0, ctorInfo);
					generator.DeclareLocal(callInfo.TargetType); // T tmp;
					GenerateNewObjInvocation(generator, ctorInfo); // new T();
					generator.Emit(OpCodes.Stloc, byRefParamsCount); // tmp = <stack>;
					AssignByRefParamsToArray(generator, 0); // 
					generator.Emit(OpCodes.Ldloc, byRefParamsCount); // tmp;
				}
				else
				{
					GenerateNewObjInvocation(generator, ctorInfo); // new T();
				}
			}
			BoxIfValueType(generator, callInfo.TargetType); // return (box)<stack>;
			generator.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof (ConstructorInvoker));
		}

		private void GenerateNewObjInvocation(ILGenerator generator, ConstructorInfo ctorInfo)
		{
			PushParamsOrLocalsToStack(generator, 0);
			generator.Emit(OpCodes.Newobj, ctorInfo);
		}
	}
}