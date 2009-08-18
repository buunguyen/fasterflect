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
using System.Reflection.Emit;

namespace Fasterflect.Emitter
{
    internal abstract class InvocationEmitter : BaseEmitter
    {
        protected InvocationEmitter(CallInfo callInfo, DelegateCache cache) : base(callInfo, cache)
        {
        }

        protected int CreateLocalsForByRefParams(ILGenerator generator, int paramArrayIndex)
        {
            int numberOfByRefParams = 0;
            for (int i = 0; i < callInfo.ParamTypes.Length; i++)
            {
                var paramType = callInfo.ParamTypes[i];
                if (paramType.IsByRef)
                {
                    var type = paramType.GetElementType();
                    generator.DeclareLocal(type);
                    generator.Emit(OpCodes.Ldarg, paramArrayIndex);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    UnboxOrCast(generator, type);
                    generator.Emit(OpCodes.Stloc, numberOfByRefParams++);
                }
            }
            return numberOfByRefParams;
        }

        protected void AssignByRefParamsToArray(ILGenerator generator, int paramArrayIndex)
        {
            int currentByRefParam = 0;
            for (int i = 0; i < callInfo.ParamTypes.Length; i++)
            {
                var paramType = callInfo.ParamTypes[i];
                if (paramType.IsByRef)
                {
                    generator.Emit(OpCodes.Ldarg, paramArrayIndex);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Ldloc, currentByRefParam++);
                    var type = paramType.GetElementType();
                    if (type.IsValueType)
                    {
                        generator.Emit(OpCodes.Box, type);
                    }
                    generator.Emit(OpCodes.Stelem_Ref);
                }
            }
        }

        protected void PushParamsOrLocalsToStack(ILGenerator generator, int paramArrayIndex)
        {
            int currentByRefParam = 0;
            for (int i = 0; i < callInfo.ParamTypes.Length; i++)
            {
                var paramType = callInfo.ParamTypes[i];
                if (paramType.IsByRef)
                {
                    generator.Emit(OpCodes.Ldloca_S, currentByRefParam++);
                }
                else
                {
                    generator.Emit(OpCodes.Ldarg, paramArrayIndex);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    UnboxOrCast(generator, paramType);
                }
            }
        }

        protected void ReturnValue(ILGenerator generator, Type returnType)
        {
            if (returnType == VoidType)
            {
                generator.Emit(OpCodes.Ldnull);
            }
            else
            {
                BoxIfValueType(generator, returnType);
            }
            generator.Emit(OpCodes.Ret);
        }
    }
}
