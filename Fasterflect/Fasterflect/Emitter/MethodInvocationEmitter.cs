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
        public MethodInvocationEmitter(CallInfo callInfo, DelegateCache cache) : base(callInfo, cache)
        {
        }

        protected override object Invoke(Delegate action)
        {
            if (callInfo.IsStatic)
            {
                var invocation = (StaticMethodInvoker)action;
                return invocation.Invoke(callInfo.Parameters);
            }
            else
            {
                var invocation = (MethodInvoker)action;
                return invocation.Invoke(callInfo.Target, callInfo.Parameters);
            }
        }

        private void LoadTarget(ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldarg_0);
            if (callInfo.IsTargetTypeStruct)
            {
                generator.DeclareLocal(callInfo.TargetType);
                generator.Emit(OpCodes.Unbox_Any, callInfo.TargetType);
                generator.Emit(OpCodes.Stloc_0);
                generator.Emit(OpCodes.Ldloca_S, 0);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, callInfo.TargetType);    
            }
        }

        protected override Delegate CreateDelegate()
        {
            MethodInfo methodInfo = GetMethodInfo();
            DynamicMethod method = CreateDynamicMethod();
            ILGenerator generator = method.GetILGenerator();

            int paramArrayIndex = callInfo.IsStatic ? 0 : 1;
            bool hasReturnType = methodInfo.ReturnType != VoidType;

            if (callInfo.HasRefParam)
            {
                int byRefParamsCount = CreateLocalsForByRefParams(generator, paramArrayIndex);
                if (hasReturnType) 
                    generator.DeclareLocal(methodInfo.ReturnType);
                GenerateInvocation(methodInfo, generator, paramArrayIndex);
                if (hasReturnType) 
                    generator.Emit(OpCodes.Stloc, byRefParamsCount);
                AssignByRefParamsToArray(generator, paramArrayIndex);
                if (hasReturnType) 
                    generator.Emit(OpCodes.Ldloc, byRefParamsCount);
            }
            else
            {
                GenerateInvocation(methodInfo, generator, paramArrayIndex);
            } 

            ReturnValue(generator, methodInfo.ReturnType);
            return method.CreateDelegate(callInfo.IsStatic
                ? typeof(StaticMethodInvoker)
                : typeof(MethodInvoker));
        }

        private void GenerateInvocation(MethodInfo methodInfo, ILGenerator generator, 
            int paramArrayIndex)
        {
            if (!callInfo.IsStatic)
                LoadTarget(generator);
            PushLocalsToStack(generator, paramArrayIndex);
            generator.Emit(callInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo);
        }

        protected MethodInfo GetMethodInfo()
        {
            var methodInfo = callInfo.TargetType.GetMethod(callInfo.Name,
                BindingFlags.ExactBinding | ScopeFlag |
                BindingFlags.Public | BindingFlags.NonPublic,
                null, callInfo.ParamTypes, null);
            if (methodInfo == null)
                throw new MissingMethodException(callInfo.IsStatic ?
                    "Static method " : "Method " + callInfo.Name + " does not exist");
            return methodInfo;
        }

        protected DynamicMethod CreateDynamicMethod()
        {
            return CreateDynamicMethod("invoke", callInfo.TargetType, ObjectType,
                                       callInfo.IsStatic
                                           ? new[] { ObjectType.MakeArrayType() }
                                           : new[] { ObjectType, ObjectType.MakeArrayType() });
        }
    }
}
