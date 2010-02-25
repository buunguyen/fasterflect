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
using Fasterflect.Internal;

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
            return type.Members( MemberTypes.All, Flags.InstanceAnyVisibility, name ).FirstOrDefault();
        }

        /// <summary>
        /// Find the member identified by <paramref name="name"/> on the given <paramref name="type"/>. Use 
        /// the <paramref name="bindingFlags"/> parameter to define the scope of the search.
        /// Use the <see href="MemberDeclared"/> method if you do not wish to search base types.  
        /// </summary>
        /// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
        public static MemberInfo Member( this Type type, string name, Flags bindingFlags )
        {
            // we need to check all members to do partial name matches
            if( bindingFlags.IsAnySet( Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented ) )
            {
                return type.Members( MemberTypes.All, bindingFlags, name ).FirstOrDefault();
            }

            IList<MemberInfo> result = type.GetMember( name, bindingFlags );
            bool hasSpecialFlags =
                bindingFlags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
            result = hasSpecialFlags && result.Count > 0 ? result.Filter( bindingFlags ) : result;
            bool found = result.Count > 0;

            if( !found && bindingFlags.IsNotSet( Flags.DeclaredOnly ) )
            {
                if( type.BaseType != typeof(object) && type.BaseType != null )
                {
                    return type.BaseType.Member( name, bindingFlags );
                }
            }
            return found ? result[ 0 ] : null;
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
            return type.Members( MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility, null );
        }

        /// <summary>
        /// Find all public and non-public instance fields and properties on the given <paramref name="type"/> 
        /// that match the specified <paramref name="bindingFlags"/>, including members defined on base types.
        /// </summary>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> FieldsAndProperties( this Type type, Flags bindingFlags )
        {
            return type.Members( MemberTypes.Field | MemberTypes.Property, bindingFlags, null );
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
            return type.Members( memberTypes, Flags.InstanceAnyVisibility, null );
        }

        /// <summary>
        /// Find all public and non-public instance members on the given <paramref name="type"/> that 
        /// match the specified <paramref name="bindingFlags"/>.
        /// </summary>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> Members( this Type type, Flags bindingFlags )
        {
            return type.Members( MemberTypes.All, bindingFlags, null );
        }

        /// <summary>
        /// Find all members of the given <paramref name="memberTypes"/> on the given <paramref name="type"/> that 
        /// match the specified <paramref name="bindingFlags"/>.
        /// </summary>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> Members( this Type type, MemberTypes memberTypes, Flags bindingFlags )
        {
            return type.Members( memberTypes, bindingFlags, null );
        }

        /// <summary>
        /// Find all members of the given <paramref name="memberTypes"/> on the given <paramref name="type"/> that 
        /// match the specified <paramref name="bindingFlags"/>. If values are supplied for the <paramref name="names"/>
        /// parameter then filtering will be applied in accordance with the given <paramref name="bindingFlags"/>.
        /// </summary>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> Members( this Type type, MemberTypes memberTypes, Flags bindingFlags,
                                                 params string[] names )
        {
            if( type == null || type == typeof(object) )
            {
                return new MemberInfo[0];
            }

            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );
            bool hasNames = names != null && names.Length > 0;
            bool hasSpecialFlags =
                bindingFlags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );

            if( ! recurse && ! hasNames && ! hasSpecialFlags )
            {
                return type.FindMembers( memberTypes, bindingFlags, null, null );
            }

            var members = GetMembers( type, memberTypes, bindingFlags );
            members = hasSpecialFlags ? members.Filter( bindingFlags ) : members;
            members = hasNames ? members.Filter( bindingFlags, names ) : members;
            return members;
        }

        private static IList<MemberInfo> GetMembers( Type type, MemberTypes memberTypes, Flags bindingFlags )
        {
            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );

            if( ! recurse )
            {
                return type.FindMembers( memberTypes, bindingFlags, null, null );
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var members = new List<MemberInfo>();
            members.AddRange( type.FindMembers( memberTypes, bindingFlags, null, null ) );
            Type baseType = type.BaseType;
            while( baseType != null && baseType != typeof(object) )
            {
                members.AddRange( baseType.FindMembers( memberTypes, bindingFlags, null, null ) );
                baseType = baseType.BaseType;
            }
            return members;
        }
        #endregion

        #region Member Combined

        #region TryGetValue
		/// <summary>
        /// Finds the first (public or non-public) instance member with the given <paramref name="name"/> on the given
        /// <paramref name="source"/> object. Returns the value of the member if a match was found and null otherwise.
		/// </summary>
		/// <remarks>
        /// When using this method it is not possible to distinguish between a missing member and a member whose value is null.
		/// </remarks>
		/// <param name="source">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <returns>The value of the member or null if no member was found</returns>
        public static object TryGetValue( this object source, string name )
        {
            return TryGetValue( source, name, Flags.InstanceAnyVisibility );
        }

		/// <summary>
        /// Find the first member with the given <paramref name="name"/> on the given <paramref name="source"/> object.
        /// Returns the value of the member if a match was found and null otherwise.
        /// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <remarks>
        /// When using this method it is not possible to distinguish between a missing member and a member whose value is null.
		/// </remarks>
		/// <param name="source">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>The value of the member or null if no member was found</returns>
        public static object TryGetValue( this object source, string name, Flags bindingFlags )
        {
            Type type = source.GetType();
            var info = type.Member( name, bindingFlags );
            return info != null ? info.Get( source ) : null;
        }
        #endregion

        #region TrySetValue
		/// <summary>
        /// Find the first (public or non-public) instance member with the given <paramref name="name"/> on the 
        /// given <paramref name="source"/> object and assign it the given <paramref name="value"/>. Returns true 
        /// if a value was assigned to a member and false otherwise.
		/// </summary>
		/// <param name="source">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the member</param>
		/// <returns>True if the value was assigned to a member and false otherwise</returns>
        public static bool TrySetValue( this object source, string name, object value )
        {
            return TrySetValue( source, name, value, Flags.InstanceAnyVisibility );
        }

		/// <summary>
        /// Find the first member with the given <paramref name="name"/> on the given <paramref name="source"/> object
        /// and assign it the given <paramref name="value"/>. Returns true if a value was assigned to a member and false otherwise.
        /// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <param name="source">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the member</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>True if the value was assigned to a member and false otherwise</returns>
        public static bool TrySetValue( this object source, string name, object value, Flags bindingFlags )
        {
            Type type = source.GetType();
            var property = type.Property( name, bindingFlags );
            if( property != null && property.CanWrite )
            {
                property.Set( source, value );
                return true;
            }
            var field = type.Field( name, bindingFlags );
            if( field != null )
            {
                field.Set( source, value );
                return true;
            }
            return false;
        }
        #endregion

        #endregion
    }
}