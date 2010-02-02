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
	internal class MethodInvocationEmitter : InvocationEmitter
	{
		public MethodInvocationEmitter(string name, Type targetType, Type[] paramTypes, bool isStatic)
		{
			callInfo = new CallInfo(targetType, MemberTypes.Method, name, paramTypes, isStatic);
		}

		protected override Delegate CreateDelegate()
		{
			MethodInfo methodInfo = GetMethodInfo();
			DynamicMethod method = CreateDynamicMethod();
			ILGenerator generator = method.GetILGenerator();

			int paramArrayIndex = callInfo.IsStatic ? 0 : 1;
			bool hasReturnType = methodInfo.ReturnType != Constants.VoidType;

			int startUsableLocalIndex = 0;
			if (callInfo.HasRefParam)
			{
				startUsableLocalIndex = CreateLocalsForByRefParams(generator, paramArrayIndex);
				generator.DeclareLocal(hasReturnType
				                       	? methodInfo.ReturnType
				                       	: Constants.ObjectType); // T result;
				GenerateInvocation(methodInfo, generator, paramArrayIndex, startUsableLocalIndex + 1);
				if (hasReturnType)
					generator.Emit(OpCodes.Stloc, startUsableLocalIndex); // result = <stack>;
				AssignByRefParamsToArray(generator, paramArrayIndex);
			}
			else
			{
				generator.DeclareLocal(hasReturnType
				                       	? methodInfo.ReturnType
				                       	: Constants.ObjectType); // T result;
				GenerateInvocation(methodInfo, generator, paramArrayIndex, startUsableLocalIndex + 1);
				if (hasReturnType)
					generator.Emit(OpCodes.Stloc, startUsableLocalIndex); // result = <stack>;
			}

			if (callInfo.ShouldHandleInnerStruct)
			{
				StoreLocalToInnerStruct(generator, startUsableLocalIndex + 1); // ((ValueTypeHolder)arg0)).Value = tmp; 
			}
			if (hasReturnType)
			{
				generator.Emit(OpCodes.Ldloc, startUsableLocalIndex); // push result;
				BoxIfValueType(generator, methodInfo.ReturnType); // (box)result;
			}
			else
			{
				generator.Emit(OpCodes.Ldnull);
			}
			generator.Emit(OpCodes.Ret);

			return method.CreateDelegate(callInfo.IsStatic
			                             	? typeof (StaticMethodInvoker)
			                             	: typeof (MethodInvoker));
		}

		private void GenerateInvocation(MethodInfo methodInfo, ILGenerator generator,
		                                int paramArrayIndex, int structLocalPosition)
		{
			if (!callInfo.IsStatic)
			{
				generator.Emit(OpCodes.Ldarg_0); // arg0;
				if (callInfo.ShouldHandleInnerStruct)
				{
					generator.DeclareLocal(callInfo.TargetType); // T tmp;
					LoadInnerStructToLocal(generator, structLocalPosition); // tmp = ((ValueTypeHolder)arg0)).Value;
				}
				else
				{
					generator.Emit(OpCodes.Castclass, callInfo.TargetType); // (T)arg0;
				}
			}
			PushParamsOrLocalsToStack(generator, paramArrayIndex);
			generator.Emit(callInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo);
		}

		protected MethodInfo GetMethodInfo()
		{
			MethodInfo methodInfo = callInfo.TargetType.GetMethod(callInfo.Name,
			                                                      ScopeFlag | BindingFlags.Public | BindingFlags.NonPublic,
			                                                      null, callInfo.ParamTypes, null);
			if (methodInfo == null)
				throw new MissingMethodException(callInfo.IsStatic
				                                 	?
				                                 		"Static method "
				                                 	: "Method " + callInfo.Name + " does not exist");
			return methodInfo;
		}

		protected DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("invoke", callInfo.TargetType, Constants.ObjectType,
			                           callInfo.IsStatic
			                           	? new[] {Constants.ObjectType.MakeArrayType()}
			                           	: new[] {Constants.ObjectType, Constants.ObjectType.MakeArrayType()});
		}
	}
}