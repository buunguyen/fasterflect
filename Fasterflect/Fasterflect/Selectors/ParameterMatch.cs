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
using System.Linq;
using System.Reflection;

namespace Fasterflect.Selectors
{
	internal class ParameterMatch : IMethodSelector
	{
		#region Implementation of IMethodSelector
		public bool IsMatch( MethodBase info, Flags flags, Type[] paramTypes, Type returnType )
		{
       		var methodParameterTypes = info.Parameters().Select( p => p.ParameterType ).ToList();
			if( methodParameterTypes.Count != paramTypes.Length )
				return false;
			return !methodParameterTypes.Where( ( t, i ) => ! t.IsAssignableFrom( paramTypes[ i ] ) ).Any();
		}
		#endregion

		#region Implementation of ISelector
		public bool IsMatch( MemberInfo info, MemberTypes memberTypes, Flags flags, string name, Type[] paramTypes, Type returnType )
		{
			return IsMatch( info as MethodBase, flags, paramTypes, returnType );
		}
		#endregion
	}
}
