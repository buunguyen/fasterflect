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

        protected void LoadLocalsFromArguments(ILGenerator generator, int paramArrayIndex)
        {
            for (int i = 0; i < callInfo.ParamTypes.Length; i++)
            {
                var paramType = callInfo.ParamTypes[i];
                generator.DeclareLocal(paramType);
                generator.Emit(OpCodes.Ldarg, paramArrayIndex);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldelem_Ref);
                if (paramType.IsValueType)
                {
                    generator.Emit(OpCodes.Unbox_Any, paramType);
                }
                else
                {
                    generator.Emit(OpCodes.Castclass, paramType);
                }
                generator.Emit(OpCodes.Stloc, i);
            }
        }

        protected void PushLocalsToStack(ILGenerator generator)
        {
            for (int i = 0; i < callInfo.ParamTypes.Length; i++)
            {
                generator.Emit(OpCodes.Ldloc, i);
            }
        }

        protected void LoadTarget(ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, callInfo.TargetType);
        }

        protected void ReturnValue(ILGenerator generator, Type returnType)
        {
            if (returnType == VoidType)
            {
                generator.Emit(OpCodes.Ldnull);
                generator.Emit(OpCodes.Ret);
            }
            else
            {
                if (returnType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, returnType);
                }
                generator.Emit(OpCodes.Ret);
            }
        }
    }
}
