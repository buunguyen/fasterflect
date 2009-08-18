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
        public CtorInvocationEmitter(CallInfo callInfo, DelegateCache cache)
            : base(callInfo, cache)
        {
        }

        protected override object Invoke(Delegate action)
        {
            var invocation = (ConstructorInvoker)action;
            return invocation.Invoke(callInfo.Parameters);
        }

        protected override Delegate CreateDelegate()
        {
            var method = CreateDynamicMethod("ctor",
                callInfo.TargetType, ObjectType, new[] { ObjectType });
            ILGenerator generator = method.GetILGenerator();

            if (callInfo.IsTargetTypeStruct && callInfo.HasNoParam)
            {
                generator.DeclareLocal(callInfo.TargetType); // loc_0: T tmp;
                generator.Emit(OpCodes.Ldloca_S, 0); // &tmp;
                generator.Emit(OpCodes.Initobj, callInfo.TargetType); // init_obj(&tmp);
                generator.Emit(OpCodes.Ldloc, 0); // tmp;;
            }
            else
            {
                ConstructorInfo ctorInfo = callInfo.TargetType.GetConstructor(
                                BindingFlags.ExactBinding | BindingFlags.Instance |
                                BindingFlags.Public | BindingFlags.NonPublic,
                                null, CallingConventions.HasThis, callInfo.ParamTypes, null);
                if (ctorInfo == null)
                    throw new MissingMemberException("Constructor does not exist");

                if (callInfo.HasRefParam)
                {
                    int byRefParamsCount = CreateLocalsForByRefParams(generator, 0);
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
            ReturnValue(generator, callInfo.TargetType); // return (box)<stack>;
            return method.CreateDelegate(typeof(ConstructorInvoker));
        }

        private void GenerateNewObjInvocation(ILGenerator generator, ConstructorInfo ctorInfo)
        {
            PushParamsOrLocalsToStack(generator, 0);
            generator.Emit(OpCodes.Newobj, ctorInfo);
        }
    }
}
