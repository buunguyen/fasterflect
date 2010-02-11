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
using Fasterflect.Selectors;

namespace Fasterflect
{
	/// <summary>
	/// Extension methods for locating and accessing fields or properties, for situations where
	/// you do not care which it is.
	/// </summary>
	public static class MemberExtensions
	{
		#region Member Lookup
		#region Single Member
		/// <summary>
        /// Find the member identified by <paramref name="name"/> on the given <paramref name="type"/>. This 
		/// method searches for public and non-public instance fields on both the type itself and all parent classes.
        /// Use the <see href="MemberDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
		public static MemberInfo Member( this Type type, string name )
		{
			return type.Member( name, Flags.InstanceCriteria );
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

		#region Multiple Members: FieldsAndProperties
		/// <summary>
        /// Find all public and non-public instance fields and properties on the given <paramref name="type"/>, 
		/// including members defined on base types.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> FieldsAndProperties( this Type type )
		{
			return type.Members( MemberTypes.Field | MemberTypes.Property, Flags.InstanceCriteria );
		}

		/// <summary>
        /// Find all public and non-public instance fields and properties on the given <paramref name="type"/> 
		/// that match the specified <paramref name="flags"/>, including members defined on base types.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> FieldsAndProperties( this Type type, Flags flags )
		{
			return type.Members( MemberTypes.Field | MemberTypes.Property, flags );
		}
		#endregion

		#region Multiple Members
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
        /// match the specified <paramref name="flags"/>. If a value is supplied for the <paramref name="name"/>
        /// parameter then filtering will be applied in accordance with the given <paramref name="flags"/>.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members( this Type type, MemberTypes memberTypes, Flags flags, string name )
		{
			if( type == null || type == typeof(object) ) { return new List<MemberInfo>(); }

			flags = flags ?? Flags.Default;
			bool recurse = flags.IsNotSet( Flags.DeclaredOnly);
        	flags |= Flags.DeclaredOnly;
            flags &= ~BindingFlags.FlattenHierarchy;
			flags = flags.SetIf( Flags.ExplicitNameMatch, !string.IsNullOrEmpty( name ) );

			var members = new List<MemberInfo>( type.FindMembers( memberTypes, flags, null, null ) );
            Type baseType = type.BaseType;
            while( recurse && baseType != null && baseType != typeof(object) )
            {
                members.AddRange( baseType.FindMembers( memberTypes, flags, null, null ) );
                baseType = baseType.BaseType;
            }
			var selectors = SelectorFactory.GetMemberSelectors( flags );
			return members.Where( m => selectors.All( s => s.IsMatch( m, flags, name ) ) ).ToList();
		}
		#endregion
		#endregion

		#region MemberInfo Helpers
		/// <summary>
		/// Get the system type of the field or property identified by the <paramref name="member"/>.
		/// </summary>
		/// <returns>The system type of the member.</returns>
		public static Type Type( this MemberInfo member )
		{
			var field = member as FieldInfo;
			if( field != null )
				return field.FieldType;
			var property = member as PropertyInfo;
			if( property != null )
				return property.PropertyType;
			throw new NotSupportedException( "Can only determine the type for fields and properties." );
		}

		/// <summary>
		/// Find out whether a value can be read from the field or property identified by
		/// the <paramref name="member"/>.
		/// </summary>
		/// <returns>True for fields and readable properties, false otherwise.</returns>
		public static bool CanRead( this MemberInfo member )
		{
			var property = member as PropertyInfo;
			return member is FieldInfo || (property != null && property.CanRead);
		}

		/// <summary>
		/// Find out whether a value can be assigned to the field or property identified by
		/// the <paramref name="member"/>.
		/// </summary>
		/// <returns>True for fields and writable properties, false otherwise.</returns>
		public static bool CanWrite( this MemberInfo member )
		{
			var property = member as PropertyInfo;
			return member is FieldInfo || (property != null && property.CanWrite);
		}
		#endregion
	}
}