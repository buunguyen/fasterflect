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
using Fasterflect.Selectors.Core;

namespace Fasterflect
{
	/// <summary>
	/// Extension methods for locating and accessing fields or properties, for situations where
	/// you do not care which it is.
	/// </summary>
	public static class MemberExtensions
	{
		#region Member Lookup (Single)
		/// <summary>
        /// Find the member identified by <paramref name="name"/> on the given <paramref name="type"/>. This 
		/// method searches for public and non-public instance fields on both the type itself and all parent classes.
        /// Use the <see href="MemberDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
		public static MemberInfo Member( this Type type, string name )
		{
			return type.Members( MemberTypes.All, Flags.InstanceCriteria, name ).FirstOrDefault();
		}

		/// <summary>
        /// Find the member identified by <paramref name="name"/> on the given <paramref name="type"/>. Use 
		/// the <paramref name="flags"/> parameter to define the scope of the search.
        /// Use the <see href="MemberDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
		public static MemberInfo Member( this Type type, string name, Flags flags )
		{
			return type.Members( MemberTypes.All, flags, name ).FirstOrDefault();
		}
		#endregion

		#region Member Lookup (FieldsAndProperties)
		/// <summary>
        /// Find all public and non-public instance fields and properties on the given <paramref name="type"/>, 
		/// including members defined on base types.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> FieldsAndProperties( this Type type )
		{
			return type.Members( MemberTypes.Field | MemberTypes.Property, Flags.InstanceCriteria, null );
		}

		/// <summary>
        /// Find all public and non-public instance fields and properties on the given <paramref name="type"/> 
		/// that match the specified <paramref name="flags"/>, including members defined on base types.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> FieldsAndProperties( this Type type, Flags flags )
		{
			return type.Members( MemberTypes.Field | MemberTypes.Property, flags, null );
		}
		#endregion

		#region Member Lookup (Multiple)
		/// <summary>
        /// Find all public and non-public instance members of the given <paramref name="memberTypes"/> on 
		/// the given <paramref name="type"/>.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members( this Type type, MemberTypes memberTypes )
		{
			return type.Members( memberTypes, Flags.InstanceCriteria, null );
		}

		/// <summary>
        /// Find all public and non-public instance members on the given <paramref name="type"/> that 
        /// match the specified <paramref name="flags"/>.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members( this Type type, Flags flags )
		{
			return type.Members( MemberTypes.All, flags, null );
		}

		/// <summary>
        /// Find all members of the given <paramref name="memberTypes"/> on the given <paramref name="type"/> that 
        /// match the specified <paramref name="flags"/>. If a value is supplied for the <paramref name="name"/>
        /// parameter then filtering will be applied in accordance with the given <paramref name="flags"/>.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members( this Type type, MemberTypes memberTypes, Flags flags )
		{
			return type.Members( memberTypes, flags, null );
		}

		/// <summary>
        /// Find all members of the given <paramref name="memberTypes"/> on the given <paramref name="type"/> that 
        /// match the specified <paramref name="flags"/>. If values are supplied for the <paramref name="names"/>
        /// parameter then filtering will be applied in accordance with the given <paramref name="flags"/>.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members( this Type type, MemberTypes memberTypes, Flags flags, params string[] names )
		{
			if( type == null || type == typeof(object) ) { return new List<MemberInfo>(); }

			flags = flags ?? Flags.Default;
			bool recurse = flags.IsNotSet( Flags.DeclaredOnly );
        	flags |= Flags.DeclaredOnly;
            flags &= ~BindingFlags.FlattenHierarchy;
			flags = flags.SetIf( Flags.ExplicitNameMatch, names != null && names.Length > 0 );

			var members = new List<MemberInfo>( type.FindMembers( memberTypes, flags, null, null ) );
            Type baseType = type.BaseType;
            while( recurse && baseType != null && baseType != typeof(object) )
            {
                members.AddRange( baseType.FindMembers( memberTypes, flags, null, null ) );
                baseType = baseType.BaseType;
            }
			var selectors = SelectorFactory.GetMemberSelectors( flags );
			return members.Where( m => selectors.All( s => names.Any( n => s.IsMatch( m, flags, n ) ) ) ).ToList();
		}
		#endregion
	}
}