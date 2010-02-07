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
	/// Extension methods for locating and accessing properties.
	/// </summary>
	public static class PropertyExtensions
	{
		#region Property Access
		/// <summary>
		/// Sets the static property <paramref name="propertyName"/> of type <paramref name="targetType"/>
		/// with the specified <paramref name="value" />.
		/// </summary>
		/// <param name="targetType">The type whose static property is to be set.</param>
		/// <param name="propertyName">The name of the static property to be set.</param>
		/// <param name="value">The value used to set the static property.</param>
		/// <returns>The type whose static property is to be set.</returns>
		public static Type SetProperty( this Type targetType, string propertyName, object value )
		{
			return targetType.SetFieldOrProperty( MemberTypes.Property, propertyName, value );
		}

		/// <summary>
		/// Sets the property <paramref name="propertyName"/> of object <paramref name="target"/>
		/// with the specified <paramref name="value" />.
		/// </summary>
		/// <param name="target">The object whose property is to be set.</param>
		/// <param name="propertyName">The name of the property to be set.</param>
		/// <param name="value">The value used to set the property.</param>
		/// <returns>The object whose property is to be set.</returns>
		public static object SetProperty( this object target, string propertyName, object value )
		{
			return target.SetFieldOrProperty( MemberTypes.Property, propertyName, value );
		}

		/// <summary>
		/// Gets the value of the static property <paramref name="propertyName"/> of type 
		/// <paramref name="targetType"/>.
		/// </summary>
		/// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
		/// between this type and the actual type of the property.  Fasterflect simply inserts an explicit cast 
		/// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="targetType">The type whose static property is to be retrieved.</param>
		/// <param name="propertyName">The name of the static property whose value is to be retrieved.</param>
		/// <returns>The value of the static property.</returns>
		public static TReturn GetProperty<TReturn>( this Type targetType, string propertyName )
		{
			return targetType.GetFieldOrProperty<TReturn>( MemberTypes.Property, propertyName );
		}

		/// <summary>
		/// Retrieves the value of the property <paramref name="propertyName"/> of object
		/// <paramref name="target"/>.
		/// </summary>
		/// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
		/// between this type and the actual type of the property.  Fasterflect simply inserts an explicit cast 
		/// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="target">The object whose property is to be retrieved.</param>
		/// <param name="propertyName">The name of the property whose value is to be retrieved.</param>
		/// <returns>The value of the property.</returns>
		public static TReturn GetProperty<TReturn>( this object target, string propertyName )
		{
			return target.GetFieldOrProperty<TReturn>( MemberTypes.Property, propertyName );
		}

		/// <summary>
		/// Creates a delegate which can set the value of the specified static property.
		/// </summary>
		/// <param name="targetType">The type which the static property belongs to.</param>
		/// <param name="propertyName">The name of the static property to be set.</param>
		/// <returns>A delegate which can set the value of the specified static property.</returns>
		public static StaticMemberSetter DelegateForSetStaticProperty( this Type targetType, string propertyName )
		{
			return targetType.DelegateForSetStaticFieldOrProperty( MemberTypes.Property, propertyName );
		}

		/// <summary>
		/// Creates a delegate which can set the value of the specified property.
		/// </summary>
		/// <param name="targetType">The type which the property belongs to.</param>
		/// <param name="propertyName">The name of the property to be set.</param>
		/// <returns>A delegate which can set the value of the specified property.</returns>
		public static MemberSetter DelegateForSetProperty( this Type targetType, string propertyName )
		{
			return targetType.DelegateForSetFieldOrProperty( MemberTypes.Property, propertyName );
		}

		/// <summary>
		/// Creates a delegate which can get the value of the specified static property.
		/// </summary>
		/// <param name="targetType">The type which the static property belongs to.</param>
		/// <param name="propertyName">The name of the static property to be retrieved.</param>
		/// <returns>A delegate which can get the value of the specified static property.</returns>
		public static StaticMemberGetter DelegateForGetStaticProperty( this Type targetType, string propertyName )
		{
			return targetType.DelegateForGetStaticFieldOrProperty( MemberTypes.Property, propertyName );
		}

		/// <summary>
		/// Creates a delegate which can get the value of the specified property.
		/// </summary>
		/// <param name="targetType">The type which the property belongs to.</param>
		/// <param name="propertyName">The name of the property to be retrieved.</param>
		/// <returns>A delegate which can get the value of the specified property.</returns>
		public static MemberGetter DelegateForGetProperty( this Type targetType, string propertyName )
		{
			return targetType.DelegateForGetFieldOrProperty( MemberTypes.Property, propertyName );
		}
		#endregion

		#region PropertyInfo Access
		/// <summary>
		/// Gets the value of the instance property <paramref name="info"/> from the <paramref name="target"/>.
		/// </summary>
		/// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
		/// between this type and the actual type of the property.  Fasterflect simply inserts an explicit cast 
		/// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="info">The property to read.</param>
		/// <param name="target">The object whose property should be read.</param>
		/// <returns>The value of the specified property.</returns>
		public static TReturn GetValue<TReturn>( this PropertyInfo info, object target )
		{
			return target.GetProperty<TReturn>( info.Name );
		}

		/// <summary>
		/// Sets the value of the instance property <paramref name="info"/> on the <paramref name="target"/>.
		/// </summary>
		/// <param name="info">The property to write.</param>
		/// <param name="target">The object on which to set the property value.</param>
		/// <param name="value">The value to assign to the specified property.</param>
		public static void SetValue( this PropertyInfo info, object target, object value )
		{
			target.SetProperty( info.Name, value );
		}
		#endregion

		#region Batch Setters
		/// <summary>
		/// Sets the static properties of <paramref name="targetType"/> based on
		/// the public properties available in <paramref name="sample"/>. 
		/// </summary>
		/// <param name="targetType">The type whose static properties are to be set.</param>
		/// <param name="sample">An object whose public properties will be used to set the static properties of
		/// <paramref name="targetType"/>.</param>
		/// <returns>The type whose static properties are to be set.</returns>
		public static Type SetProperties( this Type targetType, object sample )
		{
			return targetType.SetProperties( sample, null );
		}

		/// <summary>
		/// Sets the static properties of <paramref name="targetType"/> based on
		/// the public properties available in <paramref name="sample"/>, filtered by <paramref name="properties"/>. 
		/// </summary>
		/// <param name="targetType">The type whose static properties are to be set.</param>
		/// <param name="sample">An object whose public properties will be used to set the static properties of
		/// <paramref name="targetType"/>.</param>
		/// <param name="properties">A comma delimited list of names of properties to be retrieved.  If
		/// this is <c>null</c>, all public properties are used.</param>
		/// <returns>The type whose static properties are to be set.</returns>
		public static Type SetProperties( this Type targetType, object sample, params string[] properties )
		{
			IList<PropertyInfo> propertyInfos = sample.GetType().Properties( properties );
			propertyInfos.ForEach( prop => targetType.SetProperty( prop.Name, prop.GetValue<object>( sample ) ) );
			return targetType;
		}

		/// <summary>
		/// Sets the properties of <paramref name="target"/> based on
		/// the public properties available in <paramref name="sample"/>. 
		/// </summary>
		/// <param name="target">The object whose properties are to be set.</param>
		/// <param name="sample">An object whose public properties will be used to set the properties of
		/// <paramref name="target"/>.</param>
		/// <returns>The object whose properties are to be set.</returns>
		public static object SetProperties( this object target, object sample )
		{
			return target.SetProperties( sample, null );
		}

		/// <summary>
		/// Sets the properties of <paramref name="target"/> based on
		/// the public properties available in <paramref name="sample"/>, filtered by <paramref name="properties"/>. 
		/// </summary>
		/// <param name="target">The object whose properties are to be set.</param>
		/// <param name="sample">An object whose public properties will be used to set the properties of
		/// <paramref name="target"/>.</param>
		/// <param name="properties">A list of names of properties to be retrieved. If
		/// this is <c>null</c>, all public properties are used.</param>
		/// <returns>The object whose properties are to be set.</returns>
		public static object SetProperties( this object target, object sample, params string[] properties )
		{
			IList<PropertyInfo> propertyInfos = sample.GetType().Properties( properties );
			propertyInfos.ForEach( prop => target.SetProperty( prop.Name, prop.GetValue<object>( sample ) ) );
			return target;
		}
		#endregion

		#region Indexers
		/// <summary>
		/// Sets the value of the indexer of object <paramref name="target"/>
		/// </summary>
		/// <param name="target">The object whose indexer is to be set.</param>
		/// <param name="parameters">The list of the indexer parameters plus the value to be set to the indexer.
		/// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
		/// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
		/// use a different overload of this method.</param>
		/// <returns>The object whose indexer is to be set.</returns>
		/// <example>
		/// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
		/// method should be invoked as follow:
		/// <code>
		/// target.SetIndexer(new Type[]{typeof(int), typeof(string)}, new object[]{1, "a"});
		/// </code>
		/// </example>
		public static object SetIndexer( this object target, params object[] parameters )
		{
			DelegateForSetIndexer( target.GetTypeAdjusted(), parameters.GetTypeArray() )( target, parameters );
			return target;
		}

		/// <summary>
		/// Sets the value of the indexer of object <paramref name="target"/>
		/// </summary>
		/// <param name="target">The object whose indexer is to be set.</param>
		/// <param name="paramTypes">The types of the indexer parameters (must be in the right order), plus
		/// the type of the indexer.</param>
		/// <param name="parameters">The list of the indexer parameters plus the value to be set to the indexer.
		/// This list must match with the <paramref name="paramTypes"/> list.</param>
		/// <returns>The object whose indexer is to be set.</returns>
		/// <example>
		/// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
		/// method should be invoked as follow:
		/// <code>
		/// target.SetIndexer(new Type[]{typeof(int), typeof(string)}, new object[]{1, "a"});
		/// </code>
		/// </example>
		public static object SetIndexer( this object target, Type[] paramTypes, params object[] parameters )
		{
			DelegateForSetIndexer( target.GetTypeAdjusted(), paramTypes )( target, parameters );
			return target;
		}

		/// <summary>
		/// Gets the value of the indexer of object <paramref name="target"/>
		/// </summary>
		/// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
		/// between this type and the actual type of the indexer.  Fasterflect simply inserts an explicit cast 
		/// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="target">The object whose indexer is to be retrieved.</param>
		/// <param name="parameters">The list of the indexer parameters.
		/// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
		/// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
		/// use a different overload of this method.</param>
		/// <returns>The value returned by the indexer.</returns>
		public static TReturn GetIndexer<TReturn>( this object target, params object[] parameters )
		{
			return (TReturn) DelegateForGetIndexer( target.GetTypeAdjusted(), parameters.GetTypeArray() )( target, parameters );
		}

		/// <summary>
		/// Gets the value of the indexer of object <paramref name="target"/>
		/// </summary>
		/// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
		/// between this type and the actual type of the indexer.  Fasterflect simply inserts an explicit cast 
		/// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="target">The object whose indexer is to be retrieved.</param>
		/// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
		/// <param name="parameters">The list of the indexer parameters.</param>
		/// <returns>The value returned by the indexer.</returns>
		public static TReturn GetIndexer<TReturn>( this object target, Type[] paramTypes, params object[] parameters )
		{
			return (TReturn) DelegateForGetIndexer( target.GetTypeAdjusted(), paramTypes )( target, parameters );
		}

		/// <summary>
		/// Creates a delegate which can set an indexer
		/// </summary>
		/// <param name="targetType">The type which the indexer belongs to.</param>
		/// <param name="paramTypes">The types of the indexer parameters (must be in the right order), plus
		/// the type of the indexer.</param>
		/// <returns>A delegate which can set an indexer.</returns>
		/// <example>
		/// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
		/// method should be invoked as follow:
		/// <code>
		/// MethodInvoker invoker = type.DelegateForSetIndexer(new Type[]{typeof(int), typeof(string)});
		/// </code>
		/// </example>
		public static MethodInvoker DelegateForSetIndexer( this Type targetType, params Type[] paramTypes )
		{
			return (MethodInvoker) new MethodInvocationEmitter(
			                       	Constants.IndexerSetterName, targetType, paramTypes, false ).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can get the value of an indexer.
		/// </summary>
		/// <param name="targetType">The type which the indexer belongs to.</param>
		/// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
		/// <returns>The delegate which can get the value of an indexer.</returns>
		public static MethodInvoker DelegateForGetIndexer( this Type targetType, params Type[] paramTypes )
		{
			return (MethodInvoker) new MethodInvocationEmitter(
			                       	Constants.IndexerGetterName, targetType, paramTypes, false ).GetDelegate();
		}
		#endregion

		#region Property Lookup
		#region Single Property
		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
		/// if supplied for the <typeparamref name="T"/> parameter then the properties type must be assignment
		/// compatible with this type. This method searches for public and non-public instance properties on both 
		/// the type itself and all parent classes.
		/// Use the <seealso href="PropertyDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property<T>( this Type type, string name )
		{
			return type.Property( name, Flags.InstanceCriteria, typeof(T) );
		}

		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
		/// searches for public and non-public instance properties on both the type itself and all parent classes.
		/// Use the <seealso href="PropertyDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property( this Type type, string name )
		{
			return type.Property( name, Flags.InstanceCriteria, null );
		}

		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
		/// if supplied for the <typeparamref name="T"/> parameter then the properties type must be assignment
		/// compatible with this type. Use the <paramref name="flags"/> parameter to define the scope of the search.
		/// Use the <seealso href="PropertyDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property<T>( this Type type, string name, BindingFlags flags )
		{
			return type.Property( name, flags, typeof(T) );
		}

		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. 
		/// Use the <paramref name="flags"/> parameter to define the scope of the search.
		/// Use the <seealso href="PropertyDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property( this Type type, string name, BindingFlags flags )
		{
			return type.Property( name, flags, null );
		}

		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
		/// if supplied for the <paramref name="propertyType"/> parameter then the properties type must be assignment
		/// compatible with this type. Use the <paramref name="flags"/> parameter to define the scope of the search.
		/// Use the <seealso href="PropertyDeclared"/> method if you do not wish to search base types.  
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property( this Type type, string name, BindingFlags flags, Type propertyType )
		{
			PropertyInfo info =
				type.Properties( flags ).FirstOrDefault( p => p.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
			return info != null && (propertyType == null || propertyType.IsAssignableFrom( info.PropertyType )) ? info : null;
		}

		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
		/// if supplied for the <typeparamref name="T"/> parameter then the properties type must be assignment
		/// compatible with this type. 
		/// This method searches for public and non-public instance properties on the specified type only.
		/// Use the <seealso href="Property"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo PropertyDeclared<T>( this Type type, string name )
		{
			return type.PropertyDeclared( name, Flags.InstanceCriteria, typeof(T) );
		}

		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>.
		/// This method searches for public and non-public instance properties on the specified type only.
		/// Use the <seealso href="Property"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo PropertyDeclared( this Type type, string name )
		{
			return type.PropertyDeclared( name, Flags.InstanceCriteria, null );
		}

		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
		/// if supplied for the <typeparamref name="T"/> parameter then the properties type must be assignment
		/// compatible with this type. Use the <paramref name="flags"/> parameter to define the scope of the search.
		/// Use the <seealso href="Property"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo PropertyDeclared<T>( this Type type, string name, BindingFlags flags )
		{
			return type.PropertyDeclared( name, flags, typeof(T) );
		}

		/// <summary>
		/// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. If a value
		/// if supplied for the <paramref name="propertyType"/> parameter then the properties type must be assignment
		/// compatible with this type. Use the <paramref name="flags"/> parameter to define the scope of the search.
		/// Use the <seealso href="Property"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo PropertyDeclared( this Type type, string name, BindingFlags flags, Type propertyType )
		{
			PropertyInfo info = type.GetProperty( name, flags );
			return info != null && (propertyType == null || propertyType.IsAssignableFrom( info.PropertyType )) ? info : null;
		}
		#endregion

		#region Multiple Properties
		/// <summary>
		/// Find all public and non-public instance properties on the given <paramref name="type"/>,
		/// including properties defined on base types.
		/// </summary>
		/// <returns>A list of all instance properties on the type. This value will never be null.</returns>
		public static IList<PropertyInfo> Properties( this Type type )
		{
			return type.Properties( Flags.InstanceCriteria );
		}

		/// <summary>
		/// Find all public and non-public instance properties on the given <paramref name="type"/>,
		/// including properties defined on base types. The result can optionally be filtered by specifying
		/// a case-insensitive list of property names to include using the <paramref name="properties"/>
		/// parameter.
		/// </summary>
		/// <returns>A list of matching instance properties on the type.</returns>
		/// <param name="type">The type whose public properties are to be retrieved.</param>
		/// <param name="properties">A comma delimited list of names of properties to be retrieved. If
		/// this is <c>null</c>, all public properties are returned. Names are compared case-insensitively
		/// using OrdinalIgnoreCase.</param>
		/// <returns>A list of all public properties on the type filted by <paramref name="properties"/>.
		/// This value will never be null.</returns>
		public static IList<PropertyInfo> Properties( this Type type, params string[] properties )
		{
			IList<PropertyInfo> result = type.Properties( Flags.InstanceCriteria );
			bool filter = properties != null && properties.Length > 0;
			return filter
			       	? result.Where( p => properties.Contains( p.Name, StringComparer.OrdinalIgnoreCase ) ).ToList()
			       	: result;
		}

		/// <summary>
		/// Find all properties on the given <paramref name="type"/> that match the specified <paramref name="flags"/>,
		/// including properties defined on base types.
		/// </summary>
		/// <returns>A list of all matching properties on the type. This value will never be null.</returns>
		public static IList<PropertyInfo> Properties( this Type type, BindingFlags flags )
		{
			// as we recurse below, reset flags to only include declared properties (avoid duplicates in result)
			flags |= BindingFlags.DeclaredOnly;
			flags -= BindingFlags.FlattenHierarchy;
			var properties = new List<PropertyInfo>( type.PropertiesDeclared( flags ) );
			Type baseType = type.BaseType;
			while( baseType != null && baseType != typeof(object) )
			{
				properties.AddRange( baseType.PropertiesDeclared( flags ) );
				baseType = baseType.BaseType;
			}
			return properties;
		}

		/// <summary>
		/// Find all public and non-public instance properties declared on the given <paramref name="type"/>.
		/// Use the <seealso href="Properties"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A list of all instance properties declared on the type. This value will never be null.</returns>
		public static IList<PropertyInfo> PropertiesDeclared( this Type type )
		{
			return type.PropertiesDeclared( Flags.InstanceCriteria );
		}

		/// <summary>
		/// Find all properties declared on the given <paramref name="type"/> that match the specified <paramref name="flags"/>.
		/// Use the <seealso href="Properties"/> method if you wish to include base types in the search.
		/// </summary>
		/// <returns>A list of all instance properties declared on the type. This value will never be null.</returns>
		public static IList<PropertyInfo> PropertiesDeclared( this Type type, BindingFlags flags )
		{
			flags |= BindingFlags.DeclaredOnly;
			return type.GetProperties( flags ).ToList();
		}
		#endregion
		#endregion
	}
}