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

        protected override Delegate CreateDelegate()
        {
            MethodInfo methodInfo = GetMethodInfo();
            DynamicMethod method = CreateDynamicMethod();
            ILGenerator generator = method.GetILGenerator();
            LoadLocalsFromArguments(generator, callInfo.IsStatic ? 0 : 1);
            if (!callInfo.IsStatic)
                LoadTarget(generator);
            PushLocalsToStack(generator);
            generator.Emit(callInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo);
            ReturnValue(generator, methodInfo.ReturnType);
            return method.CreateDelegate(callInfo.IsStatic
                ? typeof(StaticMethodInvoker)
                : typeof(MethodInvoker));
        }

        protected MethodInfo GetMethodInfo()
        {
            var methodInfo = callInfo.TargetType.GetMethod(callInfo.Name,
                BindingFlags.ExactBinding | ScopeFlag |
                BindingFlags.Public | BindingFlags.NonPublic,
                null, CallingConventions.HasThis, callInfo.ParamTypes, null);
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
