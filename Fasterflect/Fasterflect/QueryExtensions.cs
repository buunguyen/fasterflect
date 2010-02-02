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
using System.Reflection;

namespace Fasterflect
{
	/// <summary>
	/// Helper class for performing various reflection tasks, such as gathering information
	/// on members and their custom attributes.
	/// </summary>
	public static class QueryExtensions
	{
		#region Member Helpers

		public static Type Type(this MemberInfo member)
		{
			var field = member as FieldInfo;
			if (field != null)
				return field.FieldType;
			var property = member as PropertyInfo;
			if (property != null)
				return property.PropertyType;
			throw new NotSupportedException("Can only determine the type for fields and properties.");
		}

		public static bool CanRead(this MemberInfo member)
		{
			var property = member as PropertyInfo;
			return property == null || property.CanRead;
		}

		public static bool CanWrite(this MemberInfo member)
		{
			var property = member as PropertyInfo;
			return property == null || property.CanWrite;
		}

		#endregion

		#region Member Lookup

		/// <summary>
		/// Find and return a list of members for the specified type.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="memberTypes">The member types to include in the result.</param>
		/// <param name="bindingFlags">The search criteria used to restrict the members included in the search.</param>
		/// <returns>A list of MemberInfo objects with information on the member.</returns>
		public static IList<MemberInfo> Members(this Type type, MemberTypes memberTypes, BindingFlags bindingFlags)
		{
			IList<MemberInfo> result = new List<MemberInfo>();
			foreach (MemberInfo memberInfo in type.FindMembers(memberTypes, bindingFlags, null, null))
			{
				result.Add(memberInfo);
			}
			return result;
		}

		/// <summary>
		/// Find a specific named member on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found</returns>
		public static MemberInfo Member(this Type type, string name)
		{
			MemberInfo[] mis = type.GetMember(name, Reflector.AllCriteria);
			return mis != null && mis.Length > 0 ? mis[0] : null;
		}

		/// <summary>
		/// Find a specific named member on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <param name="bindingFlags">The search criteria used to restrict the members included in the search.</param>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found</returns>
		public static MemberInfo Member(this Type type, string name, BindingFlags bindingFlags)
		{
			MemberInfo[] mis = type.GetMember(name, bindingFlags);
			return mis != null && mis.Length > 0 ? mis[0] : null;
		}

		/// <summary>
		/// Find a specific named field on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found</returns>
		public static FieldInfo Field<T>(this Type type, string name)
		{
			FieldInfo info = type.GetField(name, Reflector.AllCriteria);
			return info != null && info.FieldType == typeof (T) ? info : null;
		}

		/// <summary>
		/// Find a specific named field on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found</returns>
		public static FieldInfo Field(this Type type, string name)
		{
			return type.GetField(name, Reflector.AllCriteria);
		}

		/// <summary>
		/// Find a specific named property on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found</returns>
		public static PropertyInfo Property<T>(this Type type, string name)
		{
			PropertyInfo info = type.GetProperty(name, Reflector.AllCriteria);
			return info != null && info.PropertyType == typeof (T) ? info : null;
		}

		/// <summary>
		/// Find a specific named property on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found</returns>
		public static PropertyInfo Property(this Type type, string name)
		{
			return type.GetProperty(name, Reflector.AllCriteria);
		}

		#endregion

		#region Member Lookups

		public static FieldInfo[] Fields(this Type type)
		{
			return type.GetFields(Reflector.InstanceCriteria) ?? new FieldInfo[0];
		}

		public static FieldInfo[] FieldsIncludingBaseTypes(this Type type)
		{
			var fields = new List<FieldInfo>(type.Fields());
			Type baseType = type.BaseType;
			while (baseType != typeof (object))
			{
				fields.AddRange(baseType.Fields());
				baseType = baseType.BaseType;
			}
			return fields.ToArray();
		}

		public static PropertyInfo[] Properties(this Type type)
		{
			return type.GetProperties() ?? new PropertyInfo[0];
		}

		private static IEnumerable<MemberInfo> FieldsAndProperties(this Type type)
		{
			foreach (FieldInfo fi in type.GetFields())
				yield return fi;
			foreach (PropertyInfo pi in type.GetProperties())
				yield return pi;
		}

		private static IEnumerable<MemberInfo> MemberFieldsAndPropertiesAndMethodsAndEvents(this Type type)
		{
			foreach (FieldInfo fi in type.GetFields())
				yield return fi;
			foreach (PropertyInfo pi in type.GetProperties())
				yield return pi;
			foreach (MethodInfo mi in type.GetMethods())
				yield return mi;
			foreach (EventInfo ei in type.GetEvents())
				yield return ei;
		}

		#endregion

		#region Untyped Get/Set Methods

		#region Get/Set Field or Property

		public static object GetValue(this MemberInfo info, object target)
		{
			if (info.MemberType == MemberTypes.Field)
				return (info as FieldInfo).GetValue(target);
			if (info.MemberType == MemberTypes.Property)
				return (info as PropertyInfo).GetValue(target);
			return null;
		}

		public static void SetValue(this MemberInfo info, object target, object value)
		{
			if (info.MemberType == MemberTypes.Field)
				(info as FieldInfo).SetValue(target, value);
			else if (info.MemberType == MemberTypes.Property)
				(info as PropertyInfo).SetValue(target, value);
		}

		#endregion

		#region Get/Set Fields

		public static object GetValue(this FieldInfo info, object target)
		{
			return info.GetValue(target);
		}

		public static void SetValue(this FieldInfo info, object target, object value)
		{
			info.SetValue(target, value);
		}

		#endregion

		#region Get/Set Properties

		public static object GetValue(this PropertyInfo info, object target)
		{
			//Check.Assert( propertyInfo.CanRead, typeof(ReflectorExtensions), "Property {0}.{1} cannot be read from.", target.GetType(), propertyInfo.Name );
			return info.GetValue(target, null);
			//return info.GetGetMethod().Invoke( target, Constants.EmptyObjectArray );
		}

		public static void SetValue(this PropertyInfo info, object target, object value)
		{
			//Check.Assert( propertyInfo.CanWrite, typeof(ReflectorExtensions), "Property {0}.{1} is read-only.", target.GetType(), propertyInfo.Name );
			info.SetValue(target, value, null);
		}

		#endregion

		#endregion
	}
}