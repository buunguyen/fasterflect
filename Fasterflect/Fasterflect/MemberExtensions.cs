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
using Fasterflect.Emitter;

namespace Fasterflect
{
	/// <summary>
	/// Extension methods for locating and accessing fields or properties, for situations where
	/// you do not care which it is.
	/// </summary>
	public static class MemberExtensions
	{
		#region Member Access
		#region Instance Getters
		public static object GetValue( this MemberInfo info, object target )
		{
			return target.GetFieldOrProperty<object>( info.MemberType, info.Name );
		}

		public static TReturn GetFieldOrProperty<TReturn>(this object target, MemberTypes memberTypes,
		                                                  string fieldOrPropertyName)
		{
			return (TReturn)
				DelegateForGetFieldOrProperty(target.GetTypeAdjusted(), memberTypes, fieldOrPropertyName)(target);
		}

		public static AttributeGetter DelegateForGetFieldOrProperty(this Type targetType, MemberTypes memberTypes,
		                                                            string fieldOrPropertyName)
		{
			return (AttributeGetter)
				new MemberGetEmitter(targetType, memberTypes, fieldOrPropertyName, false).GetDelegate();
		}
		#endregion

		#region Instance Setters
		public static void SetValue( this MemberInfo info, object target, object value )
		{
			target.SetFieldOrProperty( info.MemberType, info.Name, value );
		}

		public static object SetFieldOrProperty( this object target, MemberTypes memberTypes,
		                                        string fieldOrProperty, object value)
		{
			DelegateForSetFieldOrProperty(target.GetTypeAdjusted(), memberTypes, fieldOrProperty)(target, value);
			return target;
		}

		public static AttributeSetter DelegateForSetFieldOrProperty(this Type targetType, MemberTypes memberTypes,
		                                                            string fieldOrProperty)
		{
			return (AttributeSetter) new MemberSetEmitter(targetType, memberTypes, fieldOrProperty, false).GetDelegate();
		}
		#endregion

		#region Static Getters

		public static TReturn GetFieldOrProperty<TReturn>(this Type targetType, MemberTypes memberTypes,
		                                                  string fieldOrPropertyName)
		{
			return (TReturn) DelegateForGetStaticFieldOrProperty(targetType, memberTypes, fieldOrPropertyName)();
		}

		public static StaticAttributeGetter DelegateForGetStaticFieldOrProperty(this Type targetType, MemberTypes memberTypes,
		                                                                        string fieldOrPropertyName)
		{
			return
				(StaticAttributeGetter) new MemberGetEmitter(targetType, memberTypes, fieldOrPropertyName, true).GetDelegate();
		}

		#endregion

		#region Static Setters

		public static Type SetFieldOrProperty(this Type targetType, MemberTypes memberTypes,
		                                      string fieldOrProperty, object value)
		{
			DelegateForSetStaticFieldOrProperty(targetType, memberTypes, fieldOrProperty)(value);
			return targetType;
		}

		public static StaticAttributeSetter DelegateForSetStaticFieldOrProperty(this Type targetType,
		                                                                        MemberTypes memberTypes,
		                                                                        string fieldOrProperty)
		{
			return (StaticAttributeSetter)
			       new MemberSetEmitter(targetType, memberTypes, fieldOrProperty, true).GetDelegate();
		}

		#endregion
		#endregion

		#region Member Lookup
		/// <summary>
		/// Find and return a list of fields and properties for the specified type. This method returns
		/// both public and non-public, instance and static members.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <returns>A list of MemberInfo objects with information on the member.</returns>
		public static IList<MemberInfo> FieldsAndProperties( this Type type )
		{
			return type.Members( MemberTypes.Field | MemberTypes.Property, Reflector.AllCriteria );
		}

		/// <summary>
		/// Find and return a list of fields and properties for the specified type. This method returns
		/// both public and non-public, instance and static members.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="bindingFlags">The search criteria used to restrict the members included in the search.</param>
		/// <returns>A list of MemberInfo objects with information on the member.</returns>
		public static IList<MemberInfo> FieldsAndProperties( this Type type, BindingFlags bindingFlags )
		{
			return type.Members( MemberTypes.Field | MemberTypes.Property, bindingFlags );
		}

		/// <summary>
		/// Find and return a list of members for the specified type.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="memberTypes">The member types to include in the result.</param>
		/// <param name="bindingFlags">The search criteria used to restrict the members included in the search.</param>
		/// <returns>A list of MemberInfo objects with information on the member.</returns>
		public static IList<MemberInfo> Members( this Type type, MemberTypes memberTypes, BindingFlags bindingFlags )
		{
			return type.FindMembers(memberTypes, bindingFlags, null, null).ToList();
		}

		/// <summary>
		/// Find a specific named member on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found</returns>
		public static MemberInfo Member( this Type type, string name )
		{
			MemberInfo[] mis = type.GetMember( name, Reflector.AllCriteria );
			return mis != null && mis.Length > 0 ? mis[ 0 ] : null;
		}

		/// <summary>
		/// Find a specific named member on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <param name="bindingFlags">The search criteria used to restrict the members included in the search.</param>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found</returns>
		public static MemberInfo Member( this Type type, string name, BindingFlags bindingFlags )
		{
			MemberInfo[] mis = type.GetMember( name, bindingFlags );
			return mis != null && mis.Length > 0 ? mis[ 0 ] : null;
		}
		#endregion

		#region MemberInfo Helpers
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

		public static bool CanRead( this MemberInfo member )
		{
			var property = member as PropertyInfo;
			return property == null || property.CanRead;
		}

		public static bool CanWrite( this MemberInfo member )
		{
			var property = member as PropertyInfo;
			return property == null || property.CanWrite;
		}
		#endregion
	}
}