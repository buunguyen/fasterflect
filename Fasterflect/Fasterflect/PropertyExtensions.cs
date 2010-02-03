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
		public static Type SetProperty(this Type targetType, string propertyName, object value)
		{
			return targetType.SetFieldOrProperty(MemberTypes.Property, propertyName, value);
		}

		/// <summary>
		/// Sets the property <paramref name="propertyName"/> of object <paramref name="target"/>
		/// with the specified <paramref name="value" />.
		/// </summary>
		/// <param name="target">The object whose property is to be set.</param>
		/// <param name="propertyName">The name of the property to be set.</param>
		/// <param name="value">The value used to set the property.</param>
		/// <returns>The object whose property is to be set.</returns>
		public static object SetProperty(this object target, string propertyName, object value)
		{
			return target.SetFieldOrProperty(MemberTypes.Property, propertyName, value);
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
		public static TReturn GetProperty<TReturn>(this Type targetType, string propertyName)
		{
			return targetType.GetFieldOrProperty<TReturn>(MemberTypes.Property, propertyName);
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
		public static TReturn GetProperty<TReturn>(this object target, string propertyName)
		{
			return target.GetFieldOrProperty<TReturn>(MemberTypes.Property, propertyName);
		}

		/// <summary>
		/// Creates a delegate which can set the value of the specified static property.
		/// </summary>
		/// <param name="targetType">The type which the static property belongs to.</param>
		/// <param name="propertyName">The name of the static property to be set.</param>
		/// <returns>A delegate which can set the value of the specified static property.</returns>
		public static StaticMemberSetter DelegateForSetStaticProperty(this Type targetType, string propertyName)
		{
			return targetType.DelegateForSetStaticFieldOrProperty(MemberTypes.Property, propertyName);
		}

		/// <summary>
		/// Creates a delegate which can set the value of the specified property.
		/// </summary>
		/// <param name="targetType">The type which the property belongs to.</param>
		/// <param name="propertyName">The name of the property to be set.</param>
		/// <returns>A delegate which can set the value of the specified property.</returns>
		public static MemberSetter DelegateForSetProperty(this Type targetType, string propertyName)
		{
			return targetType.DelegateForSetFieldOrProperty(MemberTypes.Property, propertyName);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the specified static property.
		/// </summary>
		/// <param name="targetType">The type which the static property belongs to.</param>
		/// <param name="propertyName">The name of the static property to be retrieved.</param>
		/// <returns>A delegate which can get the value of the specified static property.</returns>
		public static StaticMemberGetter DelegateForGetStaticProperty(this Type targetType, string propertyName)
		{
			return targetType.DelegateForGetStaticFieldOrProperty(MemberTypes.Property, propertyName);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the specified property.
		/// </summary>
		/// <param name="targetType">The type which the property belongs to.</param>
		/// <param name="propertyName">The name of the property to be retrieved.</param>
		/// <returns>A delegate which can get the value of the specified property.</returns>
		public static MemberGetter DelegateForGetProperty(this Type targetType, string propertyName)
		{
			return targetType.DelegateForGetFieldOrProperty(MemberTypes.Property, propertyName);
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
			target.SetProperty(info.Name, value);
		}
		#endregion

		#region Batch Setters
		/// <summary>
		/// Sets the static properties of <paramref name="targetType"/> based on
		/// the properties available in <paramref name="sample"/>. 
		/// </summary>
		/// <param name="targetType">The type whose static properties are to be set.</param>
		/// <param name="sample">An object whose properties will be used to set the static properties of
		/// <paramref name="targetType"/>.</param>
		/// <returns>The type whose static properties are to be set.</returns>
		public static Type SetProperties(this Type targetType, object sample)
		{
			sample.GetType().Properties().ForEach( prop => SetProperty( targetType, prop.Name, prop.GetValue<object>( sample ) ) );
			return targetType;
		}

		/// <summary>
		/// Sets the properties of <paramref name="target"/> based on
		/// the properties available in <paramref name="sample"/>. 
		/// </summary>
		/// <param name="target">The object whose properties are to be set.</param>
		/// <param name="sample">An object whose properties will be used to set the properties of
		/// <paramref name="target"/>.</param>
		/// <returns>The object whose properties are to be set.</returns>
		public static object SetProperties(this object target, object sample)
		{
            sample.GetType().Properties().ForEach(prop => SetProperty(target, prop.Name, prop.GetValue<object>(sample)));
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
		public static object SetIndexer(this object target, params object[] parameters)
		{
			DelegateForSetIndexer(target.GetTypeAdjusted(), parameters.GetTypeArray())(target, parameters);
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
		public static object SetIndexer(this object target, Type[] paramTypes, params object[] parameters)
		{
			DelegateForSetIndexer(target.GetTypeAdjusted(), paramTypes)(target, parameters);
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
		public static TReturn GetIndexer<TReturn>(this object target, params object[] parameters)
		{
			return (TReturn) DelegateForGetIndexer(target.GetTypeAdjusted(), parameters.GetTypeArray())(target, parameters);
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
		public static TReturn GetIndexer<TReturn>(this object target, Type[] paramTypes, params object[] parameters)
		{
			return (TReturn) DelegateForGetIndexer(target.GetTypeAdjusted(), paramTypes)(target, parameters);
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
		public static MethodInvoker DelegateForSetIndexer(this Type targetType, params Type[] paramTypes)
		{
			return (MethodInvoker) new MethodInvocationEmitter(
			                       	Constants.IndexerSetterName, targetType, paramTypes, false).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can get the value of an indexer.
		/// </summary>
		/// <param name="targetType">The type which the indexer belongs to.</param>
		/// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
		/// <returns>The delegate which can get the value of an indexer.</returns>
		public static MethodInvoker DelegateForGetIndexer(this Type targetType, params Type[] paramTypes)
		{
			return (MethodInvoker) new MethodInvocationEmitter(
			                       	Constants.IndexerGetterName, targetType, paramTypes, false).GetDelegate();
		}
		#endregion

		#region Property Lookup
		/// <summary>
		/// Find a specific named property on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
        /// <param name="name">The name of the member to find</param>
        /// <typeparam name="T">The type of the specified property</typeparam>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found</returns>
		public static PropertyInfo Property<T>( this Type type, string name )
		{
            PropertyInfo info = type.GetProperty(name, Flags.AllCriteria);
			return info != null && info.PropertyType == typeof( T ) ? info : null;
		}

		/// <summary>
		/// Find a specific named property on the given type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="name">The name of the member to find</param>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found</returns>
		public static PropertyInfo Property( this Type type, string name )
		{
            return type.GetProperty(name, Flags.AllCriteria);
		}

		/// <summary>
		/// Find all public properties on the given <paramref name="type"/>.
		/// </summary>
		/// <returns>A list of all public properties on the type.</returns>
		public static List<PropertyInfo> Properties( this Type type )
		{
            return type.GetProperties().ToList();
		}
		#endregion
	}
}