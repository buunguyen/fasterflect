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
	internal class ArrayGetEmitter : BaseEmitter
	{
		public ArrayGetEmitter(Type targetType)
		{
			callInfo = new CallInfo(targetType, MemberTypes.Method,
			                        Constants.ArrayGetterName, new[] {typeof (int)});
		}

		protected override Delegate CreateDelegate()
		{
			DynamicMethod method = CreateDynamicMethod(Constants.ArrayGetterName, callInfo.TargetType,
			                                           Constants.ObjectType, new[] {Constants.ObjectType, Constants.IntType});
			ILGenerator generator = method.GetILGenerator();
			Type elementType = callInfo.TargetType.GetElementType();

			generator.Emit(OpCodes.Ldarg_0); // arg0;
			generator.Emit(OpCodes.Castclass, callInfo.TargetType); // (T)arg0
			generator.Emit(OpCodes.Ldarg_1); // arg1;
			if (elementType.IsValueType)
			{
				generator.Emit(OpCodes.Ldelem, elementType);
			}
			else
			{
				generator.Emit(OpCodes.Ldelem_Ref);
			}
			BoxIfValueType(generator, elementType);
			generator.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof (ArrayElementGetter));
		}
	}
}