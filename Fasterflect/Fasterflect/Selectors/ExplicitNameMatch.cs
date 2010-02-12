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
using Fasterflect.Selectors.Interfaces;

namespace Fasterflect.Selectors
{
	internal class ExplicitNameMatch : IMemberSelector
	{
		#region Implementation of IMemberSelector
		public bool IsMatch( MemberInfo info, Flags flags, string name )
		{
			if( name == null )
				return false;
			bool ignoreCase = flags.IsSet( Flags.IgnoreCase );
			StringComparison comparison = ignoreCase
			                              	? StringComparison.InvariantCultureIgnoreCase
			                              	: StringComparison.InvariantCulture;

			bool reserved = name.Equals( ".ctor", comparison ) || name.Equals( ".cctor", comparison );
			int index = reserved ? -1 : info.Name.LastIndexOf( '.' ) + 1;
			string memberName = index > 0 ? info.Name.Substring( index ) : info.Name;
			// TODO design is broken when we have to do stuff like this
			if( flags.IsSet( Flags.PartialNameMatch ) )
				return ignoreCase ? memberName.ToUpperInvariant().Contains( name.ToUpperInvariant() ) : memberName.Contains( name );
			return memberName.Equals( name, comparison );
		}
		#endregion

		#region Implementation of ISelector
		public bool IsMatch( MemberInfo info, MemberTypes memberTypes, Flags flags, string name, Type[] paramTypes, Type returnType )
		{
			return IsMatch( info as MethodBase, flags, name );
		}
		#endregion
	}
}
