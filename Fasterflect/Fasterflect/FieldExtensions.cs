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
using Fasterflect.Common;

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
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual type of the field.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="targetType">The type whose static field is to be retrieved.</param>
		/// <param name="fieldName">The name of the static field whose value is to be retrieved.</param>
		/// <returns>The value of the static field.</returns>
		public static TReturn GetField<TReturn>( this Type targetType, string fieldName )
		{
			return targetType.GetFieldOrProperty<TReturn>( MemberTypes.Field, fieldName );
		}

		/// <summary>
		/// Retrieves the value of the field <paramref name="fieldName"/> of object
		/// <paramref name="target"/>.
        /// </summary>
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual type of the field.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
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
        /// the public properties available in <paramref name="sample"/>. 
		/// </summary>
		/// <param name="targetType">The type whose static fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
		/// static fields of <paramref name="targetType"/>.</param>
		/// <returns>The type whose static fields are to be set.</returns>
		public static Type SetFields( this Type targetType, object sample )
		{
		    return targetType.SetFields( sample, null );
		}

        /// <summary>
        /// Sets the static fields of <paramref name="targetType"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by 
        /// <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="targetType">The type whose static fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// static fields of <paramref name="targetType"/>.</param>
        /// <param name="propertiesToInclude">A list of names of properties to be retrieved. If
        /// this is <c>null</c>, all public properties are used.</param>
        /// <returns>The type whose static fields are to be set.</returns>
        public static Type SetFields( this Type targetType, object sample, params string[] propertiesToInclude )
        {
            var properties = sample.GetType().Properties( propertiesToInclude );
            properties.ForEach( prop => targetType.SetField( prop.Name, prop.GetValue<object>( sample ) ) );
            return targetType;
        }

	    /// <summary>
		/// Sets the fields of <paramref name="target"/> based on
        /// the public properties available in <paramref name="sample"/>. 
		/// </summary>
		/// <param name="target">The object whose fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the fields of
		/// <paramref name="target"/>.</param>
		/// <returns>The object whose fields are to be set.</returns>
		public static object SetFields( this object target, object sample )
		{
	        return target.SetFields( sample, null );
		}

        /// <summary>
        /// Sets the fields of <paramref name="target"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by 
        /// <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="target">The object whose fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the fields of
        /// <paramref name="target"/>.</param>
        /// <param name="propertiesToInclude">A comma delimited list of names of properties to be retrieved.  If
        /// this is <c>null</c>, all public properties are used.</param>
        /// <returns>The object whose fields are to be set.</returns>
        public static object SetFields( this object target, object sample, params string[] propertiesToInclude )
        {
            var properties = sample.GetType().Properties( propertiesToInclude );
            properties.ForEach( prop => target.SetField( prop.Name, prop.GetValue<object>( sample ) ) );
            return target;
        }
		#endregion
		#endregion

		#region Field Lookup
		#region Single Field
		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
        /// if supplied for the <typeparamref name="T"/> parameter then the fields type must be assignment
        /// compatible with this type. This method searches for public and non-public instance fields on both 
        /// the type itself and all parent classes.
        /// Use the <seealso href="FieldDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field<T>( this Type type, string name )
		{
			return type.Field( name, Flags.InstanceCriteria, typeof(T) );
		}

		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
        /// searches for public and non-public instance fields on both the type itself and all parent classes.
        /// Use the <seealso href="FieldDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field( this Type type, string name )
		{
			return type.Field( name, Flags.InstanceCriteria, null );
		}

		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
        /// if supplied for the <typeparamref name="T"/> parameter then the fields type must be assignment
        /// compatible with this type. Use the <paramref name="flags"/> parameter to define the scope of the search.
        /// Use the <seealso href="FieldDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field<T>( this Type type, string name, BindingFlags flags )
		{
			return type.Field( name, flags, typeof(T) );
		}

		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. 
        /// Use the <paramref name="flags"/> parameter to define the scope of the search.
        /// Use the <seealso href="FieldDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field( this Type type, string name, BindingFlags flags )
		{
			return type.Field( name, flags, null );
		}

		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
        /// if supplied for the <paramref name="fieldType"/> parameter then the fields type must be assignment
        /// compatible with this type. Use the <paramref name="flags"/> parameter to define the scope of the search.
        /// Use the <seealso href="FieldDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field( this Type type, string name, BindingFlags flags, Type fieldType )
		{
            FieldInfo info = type.Fields( flags ).FirstOrDefault( f => f.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
			return info != null && (fieldType == null || fieldType.IsAssignableFrom( info.FieldType )) ? info : null;
		}

		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
        /// if supplied for the <typeparamref name="T"/> parameter then the fields type must be assignment
        /// compatible with this type. 
        /// This method searches for public and non-public instance fields on the specified type only.
        /// Use the <seealso href="Field"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo FieldDeclared<T>( this Type type, string name )
		{
			return type.FieldDeclared( name, Flags.InstanceCriteria, typeof(T) );
		}

		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>.
        /// This method searches for public and non-public instance fields on the specified type only.
        /// Use the <seealso href="Field"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo FieldDeclared( this Type type, string name )
		{
			return type.FieldDeclared( name, Flags.InstanceCriteria, null );
		}

		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
        /// if supplied for the <typeparamref name="T"/> parameter then the fields type must be assignment
        /// compatible with this type. Use the <paramref name="flags"/> parameter to define the scope of the search.
        /// Use the <seealso href="Field"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo FieldDeclared<T>( this Type type, string name, BindingFlags flags )
		{
			return type.FieldDeclared( name, flags, typeof(T) );
		}

		/// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
        /// if supplied for the <paramref name="fieldType"/> parameter then the fields type must be assignment
        /// compatible with this type. Use the <paramref name="flags"/> parameter to define the scope of the search.
        /// Use the <seealso href="Field"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo FieldDeclared( this Type type, string name, BindingFlags flags, Type fieldType )
		{
            FieldInfo info = type.GetField( name, flags );
			return info != null && (fieldType == null || fieldType.IsAssignableFrom( info.FieldType )) ? info : null;
		}
		#endregion

		#region Multiple Fields
		/// <summary>
        /// Find all public and non-public instance fields on the given <paramref name="type"/>,
		/// including fields defined on base types.
		/// </summary>
		/// <returns>A list of all instance fields on the type. This value will never be null.</returns>
		public static IList<FieldInfo> Fields( this Type type )
		{
			return type.Fields( Flags.InstanceCriteria );
		}

		/// <summary>
        /// Find all fields on the given <paramref name="type"/> that match the specified <paramref name="flags"/>,
		/// including fields defined on base types.
		/// </summary>
		/// <returns>A list of all matching fields on the type. This value will never be null.</returns>
		public static IList<FieldInfo> Fields( this Type type, BindingFlags flags )
		{
			// as we recurse below, reset flags to only include declared fields (avoid duplicates in result)
			flags |= BindingFlags.DeclaredOnly;
			flags -= BindingFlags.FlattenHierarchy;
			var fields = new List<FieldInfo>( type.FieldsDeclared( flags ) );
			Type baseType = type.BaseType;
			while( baseType != null && baseType != typeof(object) )
			{
				fields.AddRange( baseType.FieldsDeclared( flags ) );
				baseType = baseType.BaseType;
			}
			return fields;
		}

		/// <summary>
		/// Find all public and non-public instance fields declared on the given <paramref name="type"/>.
		/// Use the <seealso href="Fields"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A list of all instance fields declared on the type. This value will never be null.</returns>
		public static IList<FieldInfo> FieldsDeclared( this Type type )
		{
			return type.FieldsDeclared( Flags.InstanceCriteria );
		}

		/// <summary>
		/// Find all fields declared on the given <paramref name="type"/> that match the specified <paramref name="flags"/>.
		/// Use the <seealso href="Fields"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A list of all instance fields declared on the type. This value will never be null.</returns>
		public static IList<FieldInfo> FieldsDeclared( this Type type, BindingFlags flags )
		{
			flags |= BindingFlags.DeclaredOnly;
			return type.GetFields( flags ).ToList();
		}
		#endregion
		#endregion
	}
}