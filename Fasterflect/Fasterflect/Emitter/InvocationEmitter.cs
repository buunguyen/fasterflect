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
	internal abstract class InvocationEmitter : BaseEmitter
	{
		protected byte CreateLocalsForByRefParams(byte paramArrayIndex, MethodBase invocationInfo)
        {
            byte numberOfByRefParams = 0;
		    var parameters = invocationInfo.GetParameters();
			for (int i = 0; i < callInfo.ParamTypes.Length; i++)
			{
				Type paramType = callInfo.ParamTypes[i];
				if (paramType.IsByRef)
				{
					Type type = paramType.GetElementType();
					generator.DeclareLocal(type);
                    if (!parameters[i].IsOut) // no initialization necessary is 'out' parameter
                    {
                        generator.ldarg(paramArrayIndex)
                                 .ldc_i4(i)
                                 .ldelem_ref
                                 .CastFromObject(type)
                                 .stloc(numberOfByRefParams)
                                 .end();
                    }
				    numberOfByRefParams++;
				}
			}
			return numberOfByRefParams;
		}

		protected void AssignByRefParamsToArray(int paramArrayIndex)
		{
			byte currentByRefParam = 0;
			for (int i = 0; i < callInfo.ParamTypes.Length; i++)
			{
				Type paramType = callInfo.ParamTypes[i];
				if (paramType.IsByRef)
				{
				    generator.ldarg( paramArrayIndex )
    				         .ldc_i4( i )
                             .ldloc(currentByRefParam++)
                             .boxIfValueType(paramType.GetElementType())
                             .stelem_ref
                             .end();
				}
			}
		}

		protected void PushParamsOrLocalsToStack(int paramArrayIndex)
		{
			byte currentByRefParam = 0;
			for (int i = 0; i < callInfo.ParamTypes.Length; i++)
			{
				Type paramType = callInfo.ParamTypes[i];
				if (paramType.IsByRef)
				{
				    generator.ldloca_s( currentByRefParam++ );
				}
				else
				{
				    generator.ldarg( paramArrayIndex )
				             .ldc_i4( i )
				             .ldelem_ref
                             .CastFromObject(paramType);
				}
			}
		}
	}
}