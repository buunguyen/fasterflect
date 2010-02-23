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
using Fasterflect.Emitter;
using Fasterflect.Internal;

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
        /// <returns><paramref name="targetType"/>.</returns>
        public static Type SetPropertyValue( this Type targetType, string propertyName, object value )
        {
            DelegateForSetStaticPropertyValue( targetType, propertyName )( value );
            return targetType;
        }

        /// <summary>
        /// Sets the instance property <paramref name="propertyName"/> of object <paramref name="target"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="target"/>.</returns>
        public static object SetPropertyValue( this object target, string propertyName, object value )
        {
            DelegateForSetPropertyValue( target.GetTypeAdjusted(), propertyName )( target, value );
            return target;
        }

        /// <summary>
        /// Gets the value of the static property <paramref name="propertyName"/> of type 
        /// <paramref name="targetType"/>.
        /// </summary>
        public static object GetPropertyValue( this Type targetType, string propertyName )
        {
            return DelegateForGetStaticPropertyValue( targetType, propertyName )();
        }

        /// <summary>
        /// Gets the value of the property <paramref name="propertyName"/> of object
        /// <paramref name="target"/>.
        /// </summary>
        public static object GetPropertyValue( this object target, string propertyName )
        {
            return DelegateForGetPropertyValue( target.GetTypeAdjusted(), propertyName )( target );
        }

        /// <summary>
        /// Sets the static property <paramref name="propertyName"/> of type <paramref name="targetType"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="targetType"/>.</returns>
        public static Type SetPropertyValue(this Type targetType, Flags flags, string propertyName, object value)
        {
            DelegateForSetStaticPropertyValue(targetType, flags, propertyName)(value);
            return targetType;
        }

        /// <summary>
        /// Sets the instance property <paramref name="propertyName"/> of object <paramref name="target"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="target"/>.</returns>
        public static object SetPropertyValue(this object target, Flags flags, string propertyName, object value)
        {
            DelegateForSetPropertyValue(target.GetTypeAdjusted(), flags, propertyName)(target, value);
            return target;
        }

        /// <summary>
        /// Gets the value of the static property <paramref name="propertyName"/> of type 
        /// <paramref name="targetType"/>.
        /// </summary>
        public static object GetPropertyValue(this Type targetType, Flags flags, string propertyName)
        {
            return DelegateForGetStaticPropertyValue(targetType, flags, propertyName)();
        }

        /// <summary>
        /// Gets the value of the property <paramref name="propertyName"/> of object
        /// <paramref name="target"/>.
        /// </summary>
        public static object GetPropertyValue(this object target, Flags flags, string propertyName)
        {
            return DelegateForGetPropertyValue(target.GetTypeAdjusted(), flags, propertyName)(target);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static property <paramref name="propertyName"/>
        /// of type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticPropertyValue(this Type targetType, string propertyName)
        {
            return DelegateForSetStaticPropertyValue( targetType, Flags.DefaultCriteria, propertyName );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance property <paramref name="propertyName"/>
        /// of type <paramref name="targetType"/>.
        /// </summary>
        public static MemberSetter DelegateForSetPropertyValue(this Type targetType, string propertyName)
        {
            return DelegateForSetPropertyValue(targetType, Flags.DefaultCriteria, propertyName);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static property <paramref name="propertyName"/>
        /// of type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticPropertyValue(this Type targetType, string propertyName)
        {
            return DelegateForGetStaticPropertyValue( targetType, Flags.DefaultCriteria, propertyName );
        }

        /// <summary>
        /// Creates a delegate which can get the value of the instance property <paramref name="propertyName"/>
        /// of type <paramref name="targetType"/>.
        /// </summary>
        public static MemberGetter DelegateForGetPropertyValue(this Type targetType, string propertyName)
        {
            return DelegateForGetPropertyValue( targetType, Flags.DefaultCriteria, propertyName );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static property <paramref name="propertyName"/>
        /// of type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticPropertyValue(this Type targetType, Flags flags, string propertyName)
        {
            return
                (StaticMemberSetter)
                new MemberSetEmitter(targetType, flags, MemberTypes.Property, propertyName, true).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance property <paramref name="propertyName"/>
        /// of type <paramref name="targetType"/>.
        /// </summary>
        public static MemberSetter DelegateForSetPropertyValue(this Type targetType, Flags flags, string propertyName)
        {
            return
                (MemberSetter)
                new MemberSetEmitter(targetType, flags, MemberTypes.Property, propertyName, false).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static property <paramref name="propertyName"/>
        /// of type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticPropertyValue(this Type targetType, Flags flags, string propertyName)
        {
            return
                (StaticMemberGetter)
                new MemberGetEmitter(targetType, flags, MemberTypes.Property, propertyName, true).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the instance property <paramref name="propertyName"/>
        /// of type <paramref name="targetType"/>.
        /// </summary>
        public static MemberGetter DelegateForGetPropertyValue(this Type targetType, Flags flags, string propertyName)
        {
            return
                (MemberGetter)
                new MemberGetEmitter( targetType, flags, MemberTypes.Property, propertyName, false).GetDelegate();
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
            propertyInfos.ForEach( prop => targetType.SetPropertyValue( prop.Name, prop.Get( sample ) ) );
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
            propertyInfos.ForEach( prop => target.SetPropertyValue( prop.Name, prop.Get( sample ) ) );
            return target;
        }
        #endregion

        #region Indexers
        /// <summary>
        /// Sets the value of the indexer of object <paramref name="target"/>
        /// </summary>
        /// <param name="target">The object whose indexer is to be set.</param>
        /// <param name="parameters">The list of the indexer parameters plus the value to be set to the indexer.
        /// The parameter types are determined from these parameters, therefore no parameter can be <c>null</c>.
        /// If any parameter is <c>null</c> (or you can't be sure of that, i.e. receive from a variable), 
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
        /// <param name="target">The object whose indexer is to be retrieved.</param>
        /// <param name="parameters">The list of the indexer parameters.
        /// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
        /// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
        /// use a different overload of this method.</param>
        /// <returns>The value returned by the indexer.</returns>
        public static object GetIndexer( this object target, params object[] parameters )
        {
            return DelegateForGetIndexer( target.GetTypeAdjusted(), parameters.GetTypeArray() )( target, parameters );
        }

        /// <summary>
        /// Gets the value of the indexer of object <paramref name="target"/>
        /// </summary>
        /// <param name="target">The object whose indexer is to be retrieved.</param>
        /// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
        /// <param name="parameters">The list of the indexer parameters.</param>
        /// <returns>The value returned by the indexer.</returns>
        public static object GetIndexer( this object target, Type[] paramTypes, params object[] parameters )
        {
            return DelegateForGetIndexer( target.GetTypeAdjusted(), paramTypes )( target, parameters );
        }

        /// <summary>
        /// Sets the value of the indexer of object <paramref name="target"/>
        /// </summary>
        /// <param name="target">The object whose indexer is to be set.</param>
        /// <param name="parameters">The list of the indexer parameters plus the value to be set to the indexer.
        /// The parameter types are determined from these parameters, therefore no parameter can be <c>null</c>.
        /// If any parameter is <c>null</c> (or you can't be sure of that, i.e. receive from a variable), 
        /// use a different overload of this method.</param>
        /// <returns>The object whose indexer is to be set.</returns>
        /// <example>
        /// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
        /// method should be invoked as follow:
        /// <code>
        /// target.SetIndexer(new Type[]{typeof(int), typeof(string)}, new object[]{1, "a"});
        /// </code>
        /// </example>
        public static object SetIndexer(this object target, Flags flags, params object[] parameters)
        {
            DelegateForSetIndexer(target.GetTypeAdjusted(), flags, parameters.GetTypeArray())(target, parameters);
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
        public static object SetIndexer(this object target, Flags flags, Type[] paramTypes, params object[] parameters)
        {
            DelegateForSetIndexer(target.GetTypeAdjusted(), flags, paramTypes)(target, parameters);
            return target;
        }

        /// <summary>
        /// Gets the value of the indexer of object <paramref name="target"/>
        /// </summary>
        /// <param name="target">The object whose indexer is to be retrieved.</param>
        /// <param name="parameters">The list of the indexer parameters.
        /// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
        /// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
        /// use a different overload of this method.</param>
        /// <returns>The value returned by the indexer.</returns>
        public static object GetIndexer(this object target, Flags flags, params object[] parameters)
        {
            return DelegateForGetIndexer(target.GetTypeAdjusted(), flags, parameters.GetTypeArray())(target, parameters);
        }

        /// <summary>
        /// Gets the value of the indexer of object <paramref name="target"/>
        /// </summary>
        /// <param name="target">The object whose indexer is to be retrieved.</param>
        /// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
        /// <param name="parameters">The list of the indexer parameters.</param>
        /// <returns>The value returned by the indexer.</returns>
        public static object GetIndexer(this object target, Flags flags, Type[] paramTypes, params object[] parameters)
        {
            return DelegateForGetIndexer(target.GetTypeAdjusted(), flags, paramTypes)(target, parameters);
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
            return DelegateForSetIndexer( targetType, Flags.DefaultCriteria, paramTypes );
        }

        /// <summary>
        /// Creates a delegate which can get the value of an indexer.
        /// </summary>
        /// <param name="targetType">The type which the indexer belongs to.</param>
        /// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
        /// <returns>The delegate which can get the value of an indexer.</returns>
        public static MethodInvoker DelegateForGetIndexer(this Type targetType, params Type[] paramTypes)
        {
            return DelegateForGetIndexer(targetType, Flags.DefaultCriteria, paramTypes);
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
        public static MethodInvoker DelegateForSetIndexer(this Type targetType, Flags flags, params Type[] paramTypes)
        {
            return (MethodInvoker)new MethodInvocationEmitter(targetType, flags, Constants.IndexerSetterName, paramTypes, false).
                GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of an indexer.
        /// </summary>
        /// <param name="targetType">The type which the indexer belongs to.</param>
        /// <param name="paramTypes">The types of the indexer parameters (must be in the right order).</param>
        /// <returns>The delegate which can get the value of an indexer.</returns>
        public static MethodInvoker DelegateForGetIndexer(this Type targetType, Flags flags, params Type[] paramTypes)
        {
            return (MethodInvoker)new MethodInvocationEmitter( targetType, flags, Constants.IndexerGetterName, paramTypes, false ).
                    GetDelegate();
        }
        #endregion

        #region Property Lookup (Single)
        /// <summary>
        /// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
        /// searches for public and non-public instance properties on both the type itself and all parent classes.
        /// Use the <seealso href="PropertyDeclared"/> method if you do not wish to search base types.  
        /// </summary>
        /// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
        public static PropertyInfo Property( this Type type, string name )
        {
        	return type.Property( name, Flags.InstanceCriteria );
        }

        /// <summary>
        /// Find the property identified by <paramref name="name"/> on the given <paramref name="type"/>. 
        /// Use the <paramref name="flags"/> parameter to define the scope of the search.
        /// </summary>
        /// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
        public static PropertyInfo Property( this Type type, string name, Flags flags )
        {
			// we need to check all properties to do partial name matches
			if( flags.IsAnySet( Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented ) )
				return type.Properties( flags, name ).FirstOrDefault();

        	var result = type.GetProperty( name, flags );
        	if( result == null && flags.IsNotSet( Flags.DeclaredOnly ) )
			{ 
				if( type.BaseType != typeof(object) && type.BaseType != null )
        			return type.BaseType.Property( name, flags );
			}
			bool hasSpecialFlags = flags.IsSet( Flags.ExcludeExplicitlyImplemented );
			if( hasSpecialFlags )
			{
				IList<PropertyInfo> properties = new List<PropertyInfo> { result };
				properties = properties.Filter( flags );
				return properties.Count > 0 ? properties[ 0 ] : null;
			}
			return result;
        }
        #endregion

        #region Property Lookup (Multiple)
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
        /// a list of property names to include using the <paramref name="names"/> parameter.
        /// </summary>
        /// <returns>A list of matching instance properties on the type.</returns>
        /// <param name="type">The type whose public properties are to be retrieved.</param>
        /// <param name="names">A list of names of properties to be retrieved. If this is <c>null</c>, 
        /// all properties are returned.</param>
        /// <returns>A list of all public properties on the type filted by <paramref name="names"/>.
        /// This value will never be null.</returns>
        public static IList<PropertyInfo> Properties( this Type type, params string[] names )
        {
        	return type.Properties( Flags.InstanceCriteria, names );
        }

        /// <summary>
        /// Find all properties on the given <paramref name="type"/> that match the specified <paramref name="flags"/>,
        /// including properties defined on base types.
        /// </summary>
        /// <returns>A list of all matching properties on the type. This value will never be null.</returns>
        public static IList<PropertyInfo> Properties( this Type type, Flags flags, params string[] names )
        {
			if( type == null || type == typeof(object) ) { return new PropertyInfo[ 0 ]; }

			bool recurse = flags.IsNotSet( Flags.DeclaredOnly );
			bool hasNames = names != null && names.Length > 0;
			bool hasSpecialFlags = flags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
        	
			if( ! recurse && ! hasNames && ! hasSpecialFlags )
				return type.GetProperties( flags ) ?? new PropertyInfo[ 0 ];

			var properties = GetProperties( type, flags );
			properties = hasSpecialFlags ? properties.Filter( flags ) : properties;
			properties = hasNames ? properties.Filter( flags, names ) : properties;
			return properties;
		}

        private static IList<PropertyInfo> GetProperties( Type type, Flags flags )
        {
			bool recurse = flags.IsNotSet( Flags.DeclaredOnly );

			if( ! recurse )
				return type.GetProperties( flags ) ?? new PropertyInfo[ 0 ];

			flags |= Flags.DeclaredOnly;
			flags &= ~BindingFlags.FlattenHierarchy;

        	var properties = new List<PropertyInfo>();
			properties.AddRange( type.GetProperties( flags ) );
			Type baseType = type.BaseType;
			while( baseType != null && baseType != typeof(object) )
			{
				properties.AddRange( baseType.GetProperties( flags ) );
			    baseType = baseType.BaseType;
			}
			return properties;
		}

		/// <summary>
		/// This method applies name filtering to a set of properties.
		/// </summary>
		private static IList<PropertyInfo> ApplyFilters( IList<PropertyInfo> members, Flags flags, string[] names )
		{
			var result = new List<PropertyInfo>( members.Count );
			bool isPartial = flags.IsSet( Flags.PartialNameMatch );
			bool trimExplicit = flags.IsSet( Flags.TrimExplicitlyImplemented );

			for( int i = 0; i < members.Count; i++ )
			{
				var member = members[ i ];
				for( int j = 0; j < names.Length; j++ )
				{
					var name = names[ j ];
					var memberName = trimExplicit ? member.Name.TrimExplicitlyImplementedName() : member.Name;
					bool match = (isPartial && memberName.Contains( name )) || memberName == name;
					if( match )
					{
						result.Add( member );
						break;
					}
				}
			}
			return members;
		}
    	#endregion

		#region Property Combined
		#region TryGetValue
		public static object TryGetPropertyValue( this object source, string name )
		{
			return TryGetPropertyValue( source, name, Flags.InstanceCriteria );
		}

		public static object TryGetPropertyValue( this object source, string name, Flags flags )
		{
			Type type = source.GetType();
			var info = type.Property( name, flags );
			return info != null ? info.Get( source ) : null;
		}
		#endregion

		#region TrySetValue
		public static bool TrySetPropertyValue( this object source, string name, object value )
		{
			return TrySetPropertyValue( source, name, value, Flags.InstanceCriteria );
		}

		public static bool TrySetPropertyValue( this object source, string name, object value, Flags flags )
		{
			Type type = source.GetType();
			var info = type.Property( name, flags );
			if( info != null )
			{
				info.Set( source, value );
				return true;
			}
			return false;
		}
		#endregion
    	#endregion
	}
}