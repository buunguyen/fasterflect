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
    /// Extension methods for locating and retrieving attributes.
    /// </summary>
    public static class AttributeExtensions
    {
        #region Attribute Lookup (Single)
        /// <summary>
        /// Gets the first <see href="Attribute"/> associated with the <paramref name="source"/>.
        /// </summary>
        /// <returns>The first attribute found on the source element.</returns>
        public static Attribute Attribute( this ICustomAttributeProvider source )
        {
            return source.Attributes().FirstOrDefault();
        }

        /// <summary>
        /// Gets the first <see href="Attribute"/> of type <paramref name="attributeType"/> associated with the <paramref name="source"/>.
        /// </summary>
        /// <returns>The first attribute found on the source element.</returns>
        public static Attribute Attribute( this ICustomAttributeProvider source, Type attributeType )
        {
            return source.Attributes( attributeType ).FirstOrDefault();
        }

        /// <summary>
        /// Gets the first <see href="Attribute"/> of type <typeparamref name="T"/> associated with the <paramref name="source"/>.
        /// </summary>
        /// <returns>The first attribute found on the source element.</returns>
        public static T Attribute<T>( this ICustomAttributeProvider source ) where T : Attribute
        {
            return source.Attributes<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the first <see href="Attribute"/> of type <typeparamref name="T"/> associated with the 
        /// enumeration value given in the <paramref name="source"/> parameter.
        /// </summary>
        /// <typeparam name="T">The attribute type to search for.</typeparam>
        /// <param name="source">An enumeration value on which to search for the attribute.</param>
        /// <returns>The first attribute found on the source.</returns>
        public static T Attribute<T>( this Enum source ) where T : Attribute
        {
            return source.Attribute( typeof(T) ) as T;
        }

        /// <summary>
        /// Gets the first <see href="Attribute"/> of type <paramref name="attributeType"/> associated with the 
        /// enumeration value given in the <paramref name="source"/> parameter.
        /// </summary>
        /// <param name="source">An enumeration value on which to search for the attribute.</param>
        /// <param name="attributeType">The attribute type to search for.</param>
        /// <returns>The first attribute found on the source.</returns>
        public static Attribute Attribute( this Enum source, Type attributeType )
        {
            Type type = source.GetType();
            MemberInfo info = type.Member( source.ToString(), Flags.StaticAnyVisibility | Flags.DeclaredOnly );
            return info.Attribute( attributeType );
        }
        #endregion

        #region Attribute Lookup (Multiple)
        /// <summary>
        /// Gets the <see href="Attribute"/>s associated with the <paramref name="source"/>. The resulting
        /// list of attributes can optionally be filtered by suppliying a list of <paramref name="attributeTypes"/>
        /// to include.
        /// </summary>
        /// <returns>A list of the attributes found on the source element. This value will never be null.</returns>
        public static IList<Attribute> Attributes( this ICustomAttributeProvider source, params Type[] attributeTypes )
        {
            return source.GetCustomAttributes( true ).Cast<Attribute>().Where( attr => attributeTypes.Length == 0 ||
			                                                                         attributeTypes.Any( at => { Type type = attr.GetType();
			                                                                                                     return at == type || at.IsSubclassOf(type); } ) ).ToList();
		}

        /// <summary>
        /// Gets all <see href="Attribute"/>s of type <typeparamref name="T"/> associated with the <paramref name="source"/>.
        /// </summary>
        /// <returns>A list of the attributes found on the source element. This value will never be null.</returns>
        public static IList<T> Attributes<T>( this ICustomAttributeProvider source ) where T : Attribute
        {
            return source.GetCustomAttributes( typeof(T), true ).Cast<T>().ToList();
        }

        /// <summary>
        /// Gets the <see href="Attribute"/>s associated with the enumeration given in <paramref name="source"/>. 
        /// </summary>
        /// <typeparam name="T">The attribute type to search for.</typeparam>
        /// <param name="source">An enumeration on which to search for attributes of the given type.</param>
        /// <returns>A list of the attributes found on the supplied source. This value will never be null.</returns>
        public static IList<T> Attributes<T>( this Enum source ) where T : Attribute
        {
            return source.Attributes( typeof(T) ).Cast<T>().ToList();
        }

        /// <summary>
        /// Gets the <see href="Attribute"/>s associated with the enumeration given in <paramref name="source"/>. 
        /// The resulting list of attributes can optionally be filtered by suppliying a list of <paramref name="attributeTypes"/>
        /// to include.
        /// </summary>
        /// <returns>A list of the attributes found on the supplied source. This value will never be null.</returns>
        public static IList<Attribute> Attributes( this Enum source, params Type[] attributeTypes )
        {
            Type type = source.GetType();
            MemberInfo info = type.Member( source.ToString(), Flags.StaticAnyVisibility | Flags.DeclaredOnly );
            return info.Attributes( attributeTypes );
        }
        #endregion

        #region HasAttribute Lookup (Presence Detection)
        /// <summary>
        /// Find out whether the <paramref name="source"/> element has an associated <see href="Attribute"/>
        /// of type <paramref name="attributeType"/>.
        /// </summary>
        /// <returns>True if the source element has the associated attribute, false otherwise.</returns>
        public static bool HasAttribute( this ICustomAttributeProvider source, Type attributeType )
        {
            return source.Attribute( attributeType ) != null;
        }

        /// <summary>
        /// Find out whether the <paramref name="source"/> element has an associated <see href="Attribute"/>
        /// of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>True if the source element has the associated attribute, false otherwise.</returns>
        public static bool HasAttribute<T>( this ICustomAttributeProvider source ) where T : Attribute
        {
            return source.HasAttribute( typeof(T) );
        }

        /// <summary>
        /// Find out whether the <paramref name="source"/> element has an associated <see href="Attribute"/>
        /// of any of the types given in <paramref name="attributeTypes"/>.
        /// </summary>
        /// <returns>True if the source element has at least one of the specified attribute types, false otherwise.</returns>
        public static bool HasAnyAttribute( this ICustomAttributeProvider source, params Type[] attributeTypes )
        {
            return source.Attributes( attributeTypes ).Count() > 0;
        }

        /// <summary>
        /// Find out whether the <paramref name="source"/> element has an associated <see href="Attribute"/>
        /// of all of the types given in <paramref name="attributeTypes"/>.
        /// </summary>
        /// <returns>True if the source element has all of the specified attribute types, false otherwise.</returns>
        public static bool HasAllAttributes( this ICustomAttributeProvider source, params Type[] attributeTypes )
        {
            return attributeTypes.Length == 0 || attributeTypes.All( at => source.HasAttribute( at ) );
        }
        #endregion

        #region TypesWith Lookup
        /// <summary>
        /// Gets all types in the given <paramref name="assembly"/> that are decorated with an
        /// <see href="Attribute"/> of the given <paramref name="attributeType"/>.
        /// </summary>
        /// <returns>A list of all matching types. This value will never be null.</returns>
        public static IList<Type> TypesWith( this Assembly assembly, Type attributeType )
        {
            var query = from t in assembly.GetTypes()
                        where t.HasAttribute( attributeType )
                        select t;
            return query.ToList();
        }

        /// <summary>
        /// Gets all types in the given <paramref name="assembly"/> that are decorated with an
        /// <see href="Attribute"/> of the given type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A list of all matching types. This value will never be null.</returns>
        public static IList<Type> TypesWith<T>( this Assembly assembly ) where T : Attribute
        {
            return assembly.TypesWith( typeof(T) );
        }
        #endregion

        #region MembersWith Lookup
        /// <summary>
        /// Gets all public and non-public instance members on the given <paramref name="type"/> that are 
        /// decorated with an <see href="Attribute"/> of the given type <typeparamref name="T"/>. Only members
        /// of the given <paramref name="memberTypes"/> will be included in the result.
        /// </summary>
        /// <returns>A list of all matching members. This value will never be null.</returns>
        public static IList<MemberInfo> MembersWith<T>( this Type type, MemberTypes memberTypes ) where T : Attribute
        {
            return type.MembersWith( memberTypes, Flags.InstanceAnyVisibility, typeof(T) );
        }

        /// <summary>
        /// Gets all members on the given <paramref name="type"/> that are decorated with an
        /// <see href="Attribute"/> of the given type <typeparamref name="T"/>. Only members
        /// of the given <paramref name="memberTypes"/> and matching <paramref name="bindingFlags"/>
        /// will be included in the result. This method will include both
        /// public and non-public, instance and static members in the result.
        /// </summary>
        /// <returns>A list of all matching members. This value will never be null.</returns>
        public static IList<MemberInfo> MembersWith<T>( this Type type, MemberTypes memberTypes, Flags bindingFlags )
        {
            return type.MembersWith( memberTypes, bindingFlags, typeof(T) );
        }

        /// <summary>
        /// Gets all public and non-public instance members on the given <paramref name="type"/>. Only members
        /// of the given <paramref name="memberTypes"/> will be included in the result.
        /// The resulting list of attributes can optionally be filtered by supplying a list
        /// of <paramref name="attributeTypes"/>, in which case only members decorated with
        /// at least one of these will be included.
        /// </summary>
        /// <returns>A list of all matching members. This value will never be null.</returns>
        public static IList<MemberInfo> MembersWith( this Type type, MemberTypes memberTypes,
                                                     params Type[] attributeTypes )
        {
            return type.MembersWith( memberTypes, Flags.InstanceAnyVisibility, attributeTypes );
        }

        /// <summary>
        /// Gets all members on the given <paramref name="type"/>. Only members of the given 
        /// <paramref name="memberTypes"/> and matching <paramref name="bindingFlags"/> will be 
        /// included in the result.
        /// The resulting list of attributes can optionally be filtered by supplying a list
        /// of <paramref name="attributeTypes"/>, in which case only members decorated with
        /// at least one of these will be included.
        /// </summary>
        /// <returns>A list of all matching members. This value will never be null.</returns>
        public static IList<MemberInfo> MembersWith( this Type type, MemberTypes memberTypes, Flags bindingFlags,
                                                     params Type[] attributeTypes )
        {
            var query = from m in type.Members( memberTypes, bindingFlags )
                        where attributeTypes.Length == 0 || m.HasAnyAttribute( attributeTypes )
                        select m;
            return query.ToList();
        }

        /// <summary>
        /// Gets all fields and properties on the given <paramref name="type"/>.
        /// The resulting list of members can optionally be filtered by supplying a list
        /// of <paramref name="attributeTypes"/>, in which case only members decorated with
        /// at least one of these will be included. This method will include both
        /// public and non-public, instance and static members in the result.
        /// </summary>
        /// <returns>A list of all matching members. This value will never be null.</returns>
        public static IList<MemberInfo> FieldsAndPropertiesWith( this Type type, params Type[] attributeTypes )
        {
            return type.MembersWith( MemberTypes.Field | MemberTypes.Property, attributeTypes );
        }
        #endregion

        #region MembersAndAttributes Lookup
        /// <summary>
        /// Gets a dictionary with all members on the given <paramref name="type"/> and their associated attributes.
        /// Only members of the given <paramref name="memberTypes"/> and matching <paramref name="bindingFlags"/> will
        /// be included in the result.
        /// The list of attributes associated with each member can optionally be filtered by supplying a list of
        /// <paramref name="attributeTypes"/>, in which case only members with at least one of these will be
        /// included in the result.
        /// </summary>
        /// <returns>An dictionary mapping all matching members to their associated attributes. This value
        /// will never be null.</returns>
        public static IDictionary<MemberInfo, List<Attribute>> MembersAndAttributes( this Type type,
                                                                                     MemberTypes memberTypes,
                                                                                     Flags flags,
                                                                                     params Type[] attributeTypes )
        {
            var members = from m in type.Members( memberTypes, flags )
                          let a = m.Attributes( attributeTypes )
                          where a.Count() > 0
                          select new { Member = m, Attributes = a.ToList() };
            return members.ToDictionary( m => m.Member, m => m.Attributes );
        }
        #endregion
    }
}