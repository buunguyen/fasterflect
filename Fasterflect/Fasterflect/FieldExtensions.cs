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
	/// Extension methods for locating and accessing fields.
	/// </summary>
	public static class FieldExtensions
	{
		#region Field Access
		#region Single Setters
		/// <summary>
		/// Sets the static field <paramref name="fieldName"/> of type <paramref name="targetType"/>
		/// with the specified <paramref name="value" />.
		/// </summary>
		/// <param name="targetType">The type whose sttic field is to be set.</param>
		/// <param name="fieldName">The name of the static field to be set.</param>
		/// <param name="value">The value used to set the static field.</param>
		/// <returns>The type whose sttic field is to be set.</returns>
		public static Type SetField(this Type targetType, string fieldName, object value)
		{
			return targetType.SetFieldOrProperty(MemberTypes.Field, fieldName, value);
		}

		/// <summary>
		/// Sets the field <paramref name="fieldName"/> of object <paramref name="target"/>
		/// with the specified <paramref name="value" />.
		/// </summary>
		/// <param name="target">The object whose field is to be set.</param>
		/// <param name="fieldName">The name of the field to be set.</param>
		/// <param name="value">The value used to set the field.</param>
		/// <returns>The object whose field is to be set.</returns>
		public static object SetField(this object target, string fieldName, object value)
		{
			return target.SetFieldOrProperty(MemberTypes.Field, fieldName, value);
		}

		/// <summary>
		/// Creates a delegate which can set the value of the specified static field.
		/// </summary>
		/// <param name="targetType">The type which the static field belongs to.</param>
		/// <param name="fieldName">The name of the static field to be set.</param>
		/// <returns>A delegate which can set the value of the specified static field.</returns>
		public static StaticMemberSetter DelegateForSetStaticField(this Type targetType, string fieldName)
		{
			return targetType.DelegateForSetStaticFieldOrProperty(MemberTypes.Field, fieldName);
		}

		/// <summary>
		/// Creates a delegate which can set the value of the specified field.
		/// </summary>
		/// <param name="targetType">The type which the field belongs to.</param>
		/// <param name="fieldName">The name of the field to be set.</param>
		/// <returns>A delegate which can set the value of the specified field.</returns>
		public static MemberSetter DelegateForSetField(this Type targetType, string fieldName)
		{
			return targetType.DelegateForSetFieldOrProperty(MemberTypes.Field, fieldName);
		}
		#endregion

		#region Single Getters
		/// <summary>
		/// Gets the value of the static field <paramref name="fieldName"/> of type 
		/// <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The type whose static field is to be retrieved.</param>
		/// <param name="fieldName">The name of the static field whose value is to be retrieved.</param>
		/// <returns>The value of the static field.</returns>
		public static TReturn GetField<TReturn>(this Type targetType, string fieldName)
		{
			return targetType.GetFieldOrProperty<TReturn>(MemberTypes.Field, fieldName);
		}

		/// <summary>
		/// Retrieves the value of the field <paramref name="fieldName"/> of object
		/// <paramref name="target"/>.
		/// </summary>
		/// <param name="target">The object whose field is to be retrieved.</param>
		/// <param name="fieldName">The name of the field whose value is to be retrieved.</param>
		/// <returns>The value of the field.</returns>
		public static TReturn GetField<TReturn>(this object target, string fieldName)
		{
			return target.GetFieldOrProperty<TReturn>(MemberTypes.Field, fieldName);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the specified static field.
		/// </summary>
		/// <param name="targetType">The type which the static field belongs to.</param>
		/// <param name="fieldName">The name of the static field to be retrieved.</param>
		/// <returns>A delegate which can get the value of the specified static field.</returns>
		public static StaticMemberGetter DelegateForGetStaticField(this Type targetType, string fieldName)
		{
			return targetType.DelegateForGetStaticFieldOrProperty(MemberTypes.Field, fieldName);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the specified field.
		/// </summary>
		/// <param name="targetType">The type which the field belongs to.</param>
		/// <param name="fieldName">The name of the field to be retrieved.</param>
		/// <returns>A delegate which can get the value of the specified field.</returns>
		public static MemberGetter DelegateForGetField(this Type targetType, string fieldName)
		{
			return targetType.DelegateForGetFieldOrProperty(MemberTypes.Field, fieldName);
		}
		#endregion

		#region Batch Setters
		/// <summary>
		/// Sets the static fields of <paramref name="targetType"/> based on
		/// the properties available in <paramref name="sample"/>. 
		/// </summary>
		/// <param name="targetType">The type whose static fields are to be set.</param>
		/// <param name="sample">An object whose properties will be used to set the static fields of
		/// <paramref name="targetType"/>.</param>
		/// <returns>The type whose static fields are to be set.</returns>
		public static Type SetFields(this Type targetType, object sample)
		{
			sample.Properties().ForEach( prop => SetField( targetType, prop.Name, prop.GetValue( sample ) ) );
			return targetType;
		}

		/// <summary>
		/// Sets the fields of <paramref name="target"/> based on
		/// the properties available in <paramref name="sample"/>. 
		/// </summary>
		/// <param name="target">The object whose fields are to be set.</param>
		/// <param name="sample">An object whose fields will be used to set the properties of
		/// <paramref name="target"/>.</param>
		/// <returns>The object whose fields are to be set.</returns>
		public static object SetFields(this object target, object sample)
		{
			sample.Properties().ForEach(prop => SetField(target, prop.Name, prop.GetValue(sample)));
			return target;
		}
		#endregion
		#endregion

		#region FieldInfo Access
		/// <summary>
		/// Gets the value of the instance field <paramref name="info"/> from the <paramref name="target"/>.
		/// </summary>
		/// <param name="info">The field to read.</param>
		/// <param name="target">The object whose field should be read.</param>
		/// <returns>The value of the specified field.</returns>
		public static object GetValue( this FieldInfo info, object target )
		{
			return target.GetField<object>( info.Name );
		}

		/// <summary>
		/// Sets the value of the instance field <paramref name="info"/> on the <paramref name="target"/>.
		/// </summary>
		/// <param name="info">The field to write.</param>
		/// <param name="target">The object on which to set the field value.</param>
		/// <param name="value">The value to assign to the specified field.</param>
		public static void SetValue( this FieldInfo info, object target, object value )
		{
			target.SetField( info.Name, value );
		}
		#endregion

		#region Field Lookup
		/// <summary>
		/// Find a specific named field on the given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found</returns>
		public static FieldInfo Field<T>( this Type type, string name )
		{
			FieldInfo info = type.GetField( name, Reflector.AllCriteria );
			return info != null && info.FieldType == typeof( T ) ? info : null;
		}

		/// <summary>
		/// Find a specific named field on the given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found</returns>
		public static FieldInfo Field( this Type type, string name )
		{
			return type.GetField( name, Reflector.AllCriteria );
		}

		/// <summary>
		/// Find all instance fields on the given <paramref name="type"/>.
		/// </summary>
		/// <returns>A list of all instance fields on the type.</returns>
		public static List<FieldInfo> Fields( this Type type )
		{
			return type.GetFields( Reflector.InstanceCriteria ).ToList();
		}

		/// <summary>
		/// Find all instance fields on the given <paramref name="type"/>, including fields from
		/// all base types.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <returns>A list of all instance fields on the type.</returns>
		public static List<FieldInfo> FieldsIncludingBaseTypes( this Type type )
		{
			var fields = new List<FieldInfo>( type.Fields() );
			Type baseType = type.BaseType;
			while( baseType != typeof( object ) )
			{
				fields.AddRange( baseType.Fields() );
				baseType = baseType.BaseType;
			}
			return fields;
		}
		#endregion
	}
}