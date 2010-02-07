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
using Fasterflect.Common;

namespace Fasterflect.Emitter
{
	internal class ArraySetEmitter : BaseEmitter
	{
		public ArraySetEmitter(Type targetType)
		{
			callInfo = new CallInfo(targetType, MemberTypes.Method, Constants.ArraySetterName,
			                        new[] {typeof (int), targetType.GetElementType()});
		}

		protected internal override Delegate CreateDelegate()
		{
			DynamicMethod method = CreateDynamicMethod(Constants.ArraySetterName, callInfo.TargetType, null,
			                                           new[] {Constants.ObjectType, Constants.IntType, Constants.ObjectType});
			ILGenerator generator = method.GetILGenerator();
			Type elementType = callInfo.TargetType.GetElementType();

			generator.Emit(OpCodes.Ldarg_0); // arg0;
			generator.Emit(OpCodes.Castclass, callInfo.TargetType); // (T)arg0
			generator.Emit(OpCodes.Ldarg_1); // arg1;
			generator.Emit(OpCodes.Ldarg_2); // arg2;
			UnboxOrCast(generator, elementType);
			if (elementType.IsValueType)
			{
				generator.Emit(OpCodes.Stelem, elementType);
			}
			else
			{
				generator.Emit(OpCodes.Stelem_Ref);
			}
			generator.Emit(OpCodes.Ret);
			return method.CreateDelegate(typeof (ArrayElementSetter));
		}
	}
}