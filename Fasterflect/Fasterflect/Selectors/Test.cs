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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fasterflect.Selectors
{
	internal static class Test
	{
		public static IList<MemberInfo> Members( this Type type, MemberTypes memberTypes, Flags flags, string name )
		{
			// as we recurse below, reset flags to only include declared fields (to avoid duplicates in result)
			bool recurse = flags.AreAllSet( Flags.DeclaredOnly );
			flags |= recurse ? Flags.DeclaredOnly : Flags.None;
			//flags -= BindingFlags.FlattenHierarchy;
			
			var members = new List<MemberInfo>( type.FindMembers( memberTypes, flags, null, null ) );
			Type baseType = type.BaseType;
			while( recurse && baseType != null && baseType != typeof(object) )
			{
				members.AddRange( baseType.FindMembers( memberTypes, flags, null, null ) );
				baseType = baseType.BaseType;
			}
			// apply fasterflect selectors
			var selectors = SelectorFactory.GetMemberSelectors( flags );
			return members.Where( m => selectors.All( s => s.IsMatch( m, flags, name ) ) ).ToList();
		}
		
		public static IList<MethodInfo> Methods( this Type type, Flags flags, string name, Type[] paramTypes, Type returnType )
        {
			// as we recurse below, reset flags to only include declared fields (to avoid duplicates in result)
			bool recurse = flags.AreAllSet( Flags.DeclaredOnly );
			flags |= recurse ? Flags.DeclaredOnly : Flags.None;

  			var methods = new List<MethodInfo>( type.GetMethods( flags ) );
            Type baseType = type.BaseType;
            while( baseType != null && baseType != typeof(object) )
            {
                methods.AddRange( baseType.GetMethods( flags ) );
                baseType = baseType.BaseType;
            }
 			// apply fasterflect selectors
			var selectors = SelectorFactory.GetSelectors( flags );
			return methods.Where( m => selectors.All( s => s.IsMatch( m, MemberTypes.Method, flags, name, paramTypes, returnType ) ) ).ToList();
        }
	}
}
