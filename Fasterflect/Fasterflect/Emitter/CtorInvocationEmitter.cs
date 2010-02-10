#region License

// Copyright 2010 Buu Nguyen, Morten Mertner
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
            : this(ctorInfo.DeclaringType, ctorInfo.GetParameters().GetTypeArray(), ctorInfo) {}

        public CtorInvocationEmitter(Type targetType, Type[] paramTypes)
            : this (targetType, paramTypes, null) {}

		private CtorInvocationEmitter(Type targetType, Type[] paramTypes, ConstructorInfo ctorInfo)
		{
            callInfo = new CallInfo(targetType, MemberTypes.Constructor, targetType.Name, paramTypes, false, ctorInfo);
		}
        
		protected internal override DynamicMethod CreateDynamicMethod()
		{
            return CreateDynamicMethod("ctor", callInfo.TargetType, Constants.ObjectType, new[] { Constants.ObjectType });
		}

		protected internal override Delegate CreateDelegate()
		{
			if (callInfo.IsTargetTypeStruct && callInfo.HasNoParam) // no-arg struct needs special initialization
			{
			    generator.DeclareLocal( callInfo.TargetType );      // TargetType tmp
                generator.ldloca_s(0)                               // &tmp
			             .initobj( callInfo.TargetType )            // init_obj(&tmp)
			             .ldloc_0.end();                            // load tmp
			}
			else if (callInfo.TargetType.IsArray)
			{
			    generator.ldarg_0                                           // load args[] (method arguments)
                         .ldc_i4_0                                          // load 0
                         .ldelem_ref                                        // load args[0] (length)
                         .unbox_any( typeof(int) )                          // unbox stack
                         .newarr( callInfo.TargetType.GetElementType() );   // new T[args[0]]
			}
			else
			{
                ConstructorInfo ctorInfo = LookupUtils.GetConstructor(callInfo);
                byte startUsableLocalIndex = 0;
				if (callInfo.HasRefParam)
				{
                    startUsableLocalIndex = CreateLocalsForByRefParams(0, ctorInfo); // create by_ref_locals from argument array
					generator.DeclareLocal(callInfo.TargetType);                     // TargetType tmp;
                }
                
                PushParamsOrLocalsToStack(0);               // push arguments and by_ref_locals
                generator.newobj(ctorInfo);                 // ctor (<stack>)

				if (callInfo.HasRefParam)
				{
                    generator.stloc(startUsableLocalIndex); // tmp = <stack>;
                    AssignByRefParamsToArray(0);            // store by_ref_locals back to argument array
                    generator.ldloc(startUsableLocalIndex); // tmp
				}
			}
            generator.boxIfValueType(callInfo.TargetType)
                     .ret();                                // return (box)<stack>;
			return method.CreateDelegate(typeof (ConstructorInvoker));
		}
	}
}