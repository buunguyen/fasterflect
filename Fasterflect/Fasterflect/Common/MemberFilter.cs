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
using System.Collections.Generic;
using System.Reflection;

namespace Fasterflect
{
    internal static class MemberFilter
    {
        public static bool IsReservedName( this string name )
        {
            name = name.ToLowerInvariant();
            return name == ".ctor" || name == ".cctor";
        }

        public static string TrimExplicitlyImplementedName( this string name )
        {
            int index = name.IsReservedName() ? -1 : name.LastIndexOf( '.' ) + 1;
            return index > 0 ? name.Substring( index ) : name;
        }

        /// <summary>
        /// This method applies name filtering to a set of members.
        /// </summary>
        public static IList<T> Filter<T>( this IList<T> members, Flags bindingFlags, string[] names )
            where T : MemberInfo
        {
            var result = new List<T>( members.Count );
            bool ignoreCase = bindingFlags.IsSet( Flags.IgnoreCase );
            bool isPartial = bindingFlags.IsSet( Flags.PartialNameMatch );
            bool trimExplicit = bindingFlags.IsSet( Flags.TrimExplicitlyImplemented );

            for( int i = 0; i < members.Count; i++ )
            {
                var member = members[ i ];
                var memberName = trimExplicit ? member.Name.TrimExplicitlyImplementedName() : member.Name;
                for( int j = 0; j < names.Length; j++ )
                {
                    var name = names[ j ];
                	var comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
                    bool match = (isPartial && memberName.Contains( name )) || memberName.Equals( name, comparison );
                    if( match )
                    {
                        result.Add( member );
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// This method applies method parameter type filtering to a set of methods.
        /// </summary>
        public static IList<T> Filter<T>( this IList<T> methods, Flags bindingFlags, Type[] paramTypes )
            where T : MethodBase
        {
            var result = new List<T>( methods.Count );

            bool exact = bindingFlags.IsSet( Flags.ExactBinding );

            for( int i = 0; i < methods.Count; i++ )
            {
                var method = methods[ i ];
                var parameters = method.GetParameters();
                if( parameters.Length != paramTypes.Length )
                {
                    break;
                }
                bool match = true;
                for( int j = 0; j < paramTypes.Length; j++ )
                {
                    var type = paramTypes[ j ];
                    var parameter = parameters[ j ];
                    match &= exact ? type == parameter.ParameterType : parameter.ParameterType.IsAssignableFrom( type );
                    if( ! match )
                    {
                        break;
                    }
                }
                if( match )
                {
                    result.Add( method );
                }
            }
            return result;
        }

        /// <summary>
        /// This method applies member type filtering to a set of members.
        /// </summary>
        public static IList<T> Filter<T>( this IList<T> members, Flags bindingFlags, MemberTypes memberTypes )
            where T : MemberInfo
        {
            var result = new List<T>( members.Count );

            for( int i = 0; i < members.Count; i++ )
            {
                var member = members[ i ];
                bool match = (member.MemberType & memberTypes) == member.MemberType;
                if( match )
                {
                    result.Add( member );
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// This method applies flags-based filtering to a set of members.
        /// </summary>
        public static IList<T> Filter<T>( this IList<T> members, Flags bindingFlags ) where T : MemberInfo
        {
            var result = new List<T>( members.Count );

            for( int i = 0; i < members.Count; i++ )
            {
                var member = members[ i ];

                bool excludeBacking = bindingFlags.IsSet( Flags.ExcludeBackingMembers );
                bool excludeExplicit = bindingFlags.IsSet( Flags.ExcludeExplicitlyImplemented );

                bool match =
                    ! (excludeBacking && (member.MemberType & MemberTypes.Field) == MemberTypes.Field &&
                       member.Name[ 0 ] == '<');
                match &=
                    ! (excludeBacking && (member.MemberType & MemberTypes.Method) == MemberTypes.Method &&
                       member.Name.Substring( 1, 3 ) == "et_");
                match &= ! (excludeExplicit && member.Name.Contains( "." ));
                if( match )
                {
                    result.Add( member );
                    break;
                }
            }
            return result;
        }
    }
}