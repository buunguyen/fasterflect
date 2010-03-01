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
        /// Gets the member identified by <paramref name="name"/> on the given <paramref name="targetType"/>. This 
        /// method searches for public and non-public instance fields on both the type itself and all parent classes.
        /// </summary>
        /// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
        public static MemberInfo Member( this Type targetType, string name )
        {
            return targetType.Members( MemberTypes.All, Flags.InstanceAnyVisibility, name ).FirstOrDefault();
        }

        /// <summary>
        /// Gets the member identified by <paramref name="name"/> on the given <paramref name="targetType"/>. Use 
        /// the <paramref name="bindingFlags"/> parameter to define the scope of the search.
        /// </summary>
        /// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
        public static MemberInfo Member( this Type targetType, string name, Flags bindingFlags )
        {
            // we need to check all members to do partial name matches
            if( bindingFlags.IsAnySet( Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented ) )
            {
                return targetType.Members( MemberTypes.All, bindingFlags, name ).FirstOrDefault();
            }

            IList<MemberInfo> result = targetType.GetMember( name, bindingFlags );
            bool hasSpecialFlags =
                bindingFlags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
            result = hasSpecialFlags && result.Count > 0 ? result.Filter( bindingFlags ) : result;
            bool found = result.Count > 0;

            if( !found && bindingFlags.IsNotSet( Flags.DeclaredOnly ) )
            {
                if( targetType.BaseType != typeof(object) && targetType.BaseType != null )
                {
                    return targetType.BaseType.Member( name, bindingFlags );
                }
            }
            return found ? result[ 0 ] : null;
        }
        #endregion

        #region Member Lookup (FieldsAndProperties)
        /// <summary>
        /// Gets all public and non-public instance fields and properties on the given <paramref name="targetType"/>, 
        /// including members defined on base types.
        /// </summary>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> FieldsAndProperties( this Type targetType )
        {
            return targetType.Members( MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility, null );
        }

        /// <summary>
        /// Gets all public and non-public instance fields and properties on the given <paramref name="targetType"/> 
        /// that match the specified <paramref name="bindingFlags"/>, including members defined on base types.
        /// </summary>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> FieldsAndProperties( this Type targetType, Flags bindingFlags )
        {
            return targetType.Members( MemberTypes.Field | MemberTypes.Property, bindingFlags, null );
        }
        #endregion

        #region Member Lookup (Multiple)
        /// <summary>
        /// Gets all public and non-public instance members on the given <paramref name="targetType"/>.
        /// </summary>
        /// <returns>A list of all members on the type. This value will never be null.</returns>
		/// <param name="targetType">The type to reflect on.</param>
        /// <returns>A list of all members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> Members( this Type targetType )
        {
            return targetType.Members( MemberTypes.All, Flags.InstanceAnyVisibility, null );
        }

        /// <summary>
        /// Gets all public and non-public instance members on the given <paramref name="targetType"/> that 
        /// match the specified <paramref name="bindingFlags"/>.
        /// </summary>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
		/// <param name="targetType">The type to reflect on.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> Members( this Type targetType, Flags bindingFlags )
        {
            return targetType.Members( MemberTypes.All, bindingFlags, null );
        }

        /// <summary>
        /// Gets all public and non-public instance members of the given <paramref name="memberTypes"/> on the 
        /// given <paramref name="targetType"/>, optionally filtered by the supplied <paramref name="names"/> list.
        /// </summary>
		/// <param name="memberTypes">The <see href="MemberTypes"/> to include in the result.</param>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
		/// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> Members( this Type targetType, MemberTypes memberTypes, params string[] names )
        {
        	return targetType.Members( memberTypes, Flags.InstanceAnyVisibility, names );
        }

    	/// <summary>
        /// Gets all members of the given <paramref name="memberTypes"/> on the given <paramref name="targetType"/> that 
        /// match the specified <paramref name="bindingFlags"/>, optionally filtered by the supplied <paramref name="names"/>
        /// list (in accordance with the given <paramref name="bindingFlags"/>).
        /// </summary>
		/// <param name="targetType">The type to reflect on.</param>
		/// <param name="memberTypes">The <see href="MemberTypes"/> to include in the result.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
		/// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        public static IList<MemberInfo> Members( this Type targetType, MemberTypes memberTypes, Flags bindingFlags,
                                                 params string[] names )
        {
            if( targetType == null || targetType == typeof(object) )
            {
                return new MemberInfo[0];
            }

            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );
            bool hasNames = names != null && names.Length > 0;
            bool hasSpecialFlags =
                bindingFlags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );

            if( ! recurse && ! hasNames && ! hasSpecialFlags )
            {
                return targetType.FindMembers( memberTypes, bindingFlags, null, null );
            }

            var members = GetMembers( targetType, memberTypes, bindingFlags );
            members = hasSpecialFlags ? members.Filter( bindingFlags ) : members;
            members = hasNames ? members.Filter( bindingFlags, names ) : members;
            return members;
        }

        private static IList<MemberInfo> GetMembers( Type targetType, MemberTypes memberTypes, Flags bindingFlags )
        {
            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );

            if( ! recurse )
            {
                return targetType.FindMembers( memberTypes, bindingFlags, null, null );
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var members = new List<MemberInfo>();
            members.AddRange( targetType.FindMembers( memberTypes, bindingFlags, null, null ) );
            Type baseType = targetType.BaseType;
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
        /// Gets the first (public or non-public) instance member with the given <paramref name="name"/> on the given
        /// <paramref name="target"/> object. Returns the value of the member if a match was found and null otherwise.
		/// </summary>
		/// <remarks>
        /// When using this method it is not possible to distinguish between a missing member and a member whose value is null.
		/// </remarks>
		/// <param name="target">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <returns>The value of the member or null if no member was found</returns>
        public static object TryGetValue( this object target, string name )
        {
            return TryGetValue( target, name, Flags.InstanceAnyVisibility );
        }

		/// <summary>
        /// Gets the first member with the given <paramref name="name"/> on the given <paramref name="target"/> object.
        /// Returns the value of the member if a match was found and null otherwise.
        /// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <remarks>
        /// When using this method it is not possible to distinguish between a missing member and a member whose value is null.
		/// </remarks>
		/// <param name="target">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>The value of the member or null if no member was found</returns>
        public static object TryGetValue( this object target, string name, Flags bindingFlags )
        {
            Type type = target.GetType();
            var info = type.Member( name, bindingFlags );
            return info != null ? info.Get( target ) : null;
        }
        #endregion

        #region TrySetValue
		/// <summary>
        /// Sets the first (public or non-public) instance member with the given <paramref name="name"/> on the 
        /// given <paramref name="target"/> object to the supplied <paramref name="value"/>. Returns true 
        /// if a value was assigned to a member and false otherwise.
		/// </summary>
		/// <param name="target">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the member</param>
		/// <returns>True if the value was assigned to a member and false otherwise</returns>
        public static bool TrySetValue( this object target, string name, object value )
        {
            return TrySetValue( target, name, value, Flags.InstanceAnyVisibility );
        }

		/// <summary>
        /// Sets the first member with the given <paramref name="name"/> on the given <paramref name="target"/> object
        /// to the supplied <paramref name="value"/>. Returns true if a value was assigned to a member and false otherwise.
        /// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <param name="target">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the member</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>True if the value was assigned to a member and false otherwise</returns>
        public static bool TrySetValue( this object target, string name, object value, Flags bindingFlags )
        {
            Type type = target.GetType();
            var property = type.Property( name, bindingFlags );
            if( property != null && property.CanWrite )
            {
                property.Set( target, value );
                return true;
            }
            var field = type.Field( name, bindingFlags );
            if( field != null )
            {
                field.Set( target, value );
                return true;
            }
            return false;
        }
        #endregion

        #endregion
    }
}