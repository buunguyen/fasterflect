#region License

// Copyright 2010 Morten Mertner, Buu Nguyen (http://www.buunguyen.net/blog)
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
		#region Attribute (Multiple) Lookup

		public static IEnumerable<Attribute> Attributes(this ICustomAttributeProvider source, params Type[] attributeTypes)
		{
			foreach (Attribute attr in source.GetCustomAttributes(true))
			{
				if (attributeTypes.Length == 0 || attributeTypes.Any(at =>
				                                                     	{
				                                                     		Type type = attr.GetType();
				                                                     		return at == type || at.IsSubclassOf(type);
				                                                     	}))
					yield return attr;
			}
		}

		public static IEnumerable<T> Attributes<T>(this ICustomAttributeProvider source) where T : Attribute
		{
			foreach (T attr in source.GetCustomAttributes(typeof (T), true))
			{
				yield return attr;
			}
		}

		#endregion

		#region Attribute (Single) Lookup

		public static Attribute Attribute(this ICustomAttributeProvider source)
		{
			return source.Attributes().FirstOrDefault();
		}

		public static Attribute Attribute(this ICustomAttributeProvider source, Type attributeType)
		{
			return source.Attributes(attributeType).FirstOrDefault();
		}

		public static T Attribute<T>(this ICustomAttributeProvider source) where T : Attribute
		{
			return source.Attributes<T>().FirstOrDefault();
		}

		/// <summary>
		/// Find a specific attribute type on the given enumeration instance.
		/// </summary>
		/// <typeparam name="T">The attribute type to search for.</typeparam>
		/// <param name="instance">An enumeration value on which to search for the attribute.</param>
		/// <returns>An instance of the attribute type specified if it was found on the instance.</returns>
		public static T Attribute<T>(this Enum instance) where T : Attribute
		{
			Type type = instance.GetType();
			MemberInfo info = type.Member(instance.ToString());
			return info.Attribute<T>();
		}

		#endregion

		#region Attribute Presence Lookup

		public static bool HasAttribute(this ICustomAttributeProvider source, Type attributeType)
		{
			return source.Attribute(attributeType) != null;
		}

		public static bool HasAttribute<T>(this ICustomAttributeProvider source) where T : Attribute
		{
			return source.HasAttribute(typeof (T));
		}

		public static bool HasAnyAttribute(this ICustomAttributeProvider source, params Type[] attributeTypes)
		{
			return source.Attributes(attributeTypes).Count() > 0;
		}

		public static bool HasAllAttributes(this ICustomAttributeProvider source, params Type[] attributeTypes)
		{
			if (attributeTypes.Length == 0)
				return true;
			return attributeTypes.All(at => source.HasAttribute(at));
		}

		#endregion

		#region Types With Lookup

		public static IEnumerable<Type> TypesWith(this Assembly assembly, Type attributeType)
		{
			return from t in assembly.GetTypes()
			       where t.HasAttribute(attributeType)
			       select t;
		}

		public static IEnumerable<Type> TypesWith<T>(this Assembly assembly) where T : Attribute
		{
			return assembly.TypesWith(typeof (T));
		}

		#endregion

		#region Members With Lookup

		public static IEnumerable<MemberInfo> MembersWith<T>(this Type type, MemberTypes memberTypes) where T : Attribute
		{
			return type.MembersWith(memberTypes, Reflector.AllCriteria, typeof (T));
		}

		public static IEnumerable<MemberInfo> MembersWith<T>(this Type type, MemberTypes memberTypes,
		                                                     BindingFlags bindingFlags)
		{
			return type.MembersWith(memberTypes, Reflector.AllCriteria, typeof (T));
		}

		public static IEnumerable<MemberInfo> MembersWith(this Type type, MemberTypes memberTypes,
		                                                  params Type[] attributeTypes)
		{
			return type.MembersWith(memberTypes, Reflector.AllCriteria, attributeTypes);
		}

		public static IEnumerable<MemberInfo> MembersWith(this Type type, MemberTypes memberTypes, BindingFlags bindingFlags,
		                                                  params Type[] attributeTypes)
		{
			return from m in type.Members(memberTypes, bindingFlags)
			       where attributeTypes.Length == 0 || m.HasAnyAttribute(attributeTypes)
			       select m;
		}

		public static IEnumerable<MemberInfo> FieldsAndPropertiesWith(this Type type, params Type[] attributeTypes)
		{
			return type.MembersWith(MemberTypes.Field | MemberTypes.Property, attributeTypes);
		}

		#endregion

		#region Members And Attributes Lookup 

		//public static IEnumerable<MetaMember> MembersAndAttributes<T>( this Type type, MemberTypes memberTypes, BindingFlags bindingFlags ) where T : Attribute
		//{
		//    return type.MembersAndAttributes( memberTypes, bindingFlags, typeof(T) );
		//}
		//public static IEnumerable<MetaMember> MembersAndAttributes( this Type type, MemberTypes memberTypes, BindingFlags bindingFlags, params Type[] attributeTypes )
		//{
		//    var members = from m in type.Members( memberTypes, bindingFlags )
		//                  let a = m.Attributes( attributeTypes )
		//                  where a.Count() > 0
		//                  select MetaMember.Create( m, a.ToList() );
		//    return members;
		//}
		public static Dictionary<MemberInfo, List<Attribute>> MembersAndAttributes(this Type type, MemberTypes memberTypes,
		                                                                           BindingFlags bindingFlags,
		                                                                           params Type[] attributeTypes)
		{
			var members = from m in type.Members(memberTypes, bindingFlags)
			              let a = m.Attributes(attributeTypes)
			              where a.Count() > 0
			              select new {Member = m, Attributes = a.ToList()};
			return members.ToDictionary(m => m.Member, m => m.Attributes);
		}

		#endregion
	}
}