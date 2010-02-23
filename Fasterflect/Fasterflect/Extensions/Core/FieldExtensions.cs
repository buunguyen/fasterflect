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
    /// Extension methods for locating and accessing fields.
    /// </summary>
    public static class FieldExtensions
    {
        #region Field Access
        #region Single Access
        /// <summary>
        /// Sets the static field <paramref name="fieldName"/> of type <paramref name="targetType"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="targetType"/>.</returns>
        public static Type SetFieldValue( this Type targetType, string fieldName, object value )
        {
            DelegateForSetStaticFieldValue( targetType, fieldName )( value );
            return targetType;
        }

        /// <summary>
        /// Sets the instance field <paramref name="fieldName"/> of object <paramref name="target"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="target"/>.</returns>
        public static object SetFieldValue( this object target, string fieldName, object value )
        {
            DelegateForSetFieldValue( target.GetTypeAdjusted(), fieldName )( target, value );
            return target;
        }

        /// <summary>
        /// Gets the value of the static field <paramref name="fieldName"/> of type 
        /// <paramref name="targetType"/>.
        /// </summary>
        public static object GetFieldValue( this Type targetType, string fieldName )
        {
            return DelegateForGetStaticFieldValue( targetType, fieldName )();
        }

        /// <summary>
        /// Gets the value of the instance field <paramref name="fieldName"/> of object
        /// <paramref name="target"/>.
        /// </summary>
        public static object GetFieldValue( this object target, string fieldName )
        {
            return DelegateForGetFieldValue( target.GetTypeAdjusted(), fieldName )( target );
        }

        /// <summary>
        /// Sets the static field <paramref name="fieldName"/> of type <paramref name="targetType"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="targetType"/>.</returns>
        public static Type SetFieldValue(this Type targetType, Flags flags, string fieldName, object value)
        {
            DelegateForSetStaticFieldValue(targetType, flags, fieldName)(value);
            return targetType;
        }

        /// <summary>
        /// Sets the instance field <paramref name="fieldName"/> of object <paramref name="target"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="target"/>.</returns>
        public static object SetFieldValue(this object target, Flags flags, string fieldName, object value)
        {
            DelegateForSetFieldValue(target.GetTypeAdjusted(), flags, fieldName)(target, value);
            return target;
        }

        /// <summary>
        /// Gets the value of the static field <paramref name="fieldName"/> of type 
        /// <paramref name="targetType"/>.
        /// </summary>
        public static object GetFieldValue(this Type targetType, Flags flags, string fieldName)
        {
            return DelegateForGetStaticFieldValue(targetType, flags, fieldName)();
        }

        /// <summary>
        /// Gets the value of the instance field <paramref name="fieldName"/> of object
        /// <paramref name="target"/>.
        /// </summary>
        public static object GetFieldValue(this object target, Flags flags, string fieldName)
        {
            return DelegateForGetFieldValue(target.GetTypeAdjusted(), flags, fieldName)(target);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static field <paramref name="fieldName"/> of 
        /// type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticFieldValue(this Type targetType, string fieldName)
        {
            return DelegateForSetStaticFieldValue( targetType, Flags.DefaultCriteria, fieldName );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance field <paramref name="fieldName"/> of 
        /// type <paramref name="targetType"/>.
        /// </summary>
        public static MemberSetter DelegateForSetFieldValue(this Type targetType, string fieldName)
        {
            return DelegateForSetFieldValue( targetType, Flags.DefaultCriteria, fieldName );
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static field <paramref name="fieldName"/> of 
        /// type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticFieldValue(this Type targetType, string fieldName)
        {
            return DelegateForGetStaticFieldValue( targetType, Flags.DefaultCriteria, fieldName );
        }

        /// <summary>
        /// Creates a delegate which can get the value of the instance field <paramref name="fieldName"/> of 
        /// type <paramref name="targetType"/>.
        /// </summary>
        public static MemberGetter DelegateForGetFieldValue(this Type targetType, string fieldName)
        {
            return DelegateForGetFieldValue( targetType, Flags.DefaultCriteria, fieldName );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static field <paramref name="fieldName"/> of 
        /// type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticFieldValue(this Type targetType, Flags flags, string fieldName)
        {
            return
                (StaticMemberSetter)
                new MemberSetEmitter(targetType, flags, MemberTypes.Field, fieldName, true).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance field <paramref name="fieldName"/> of 
        /// type <paramref name="targetType"/>.
        /// </summary>
        public static MemberSetter DelegateForSetFieldValue(this Type targetType, Flags flags, string fieldName)
        {
            return (MemberSetter)new MemberSetEmitter(targetType, flags, MemberTypes.Field, fieldName, false).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static field <paramref name="fieldName"/> of 
        /// type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticFieldValue(this Type targetType, Flags flags, string fieldName)
        {
            return
                (StaticMemberGetter)
                new MemberGetEmitter(targetType, flags, MemberTypes.Field, fieldName, true).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the instance field <paramref name="fieldName"/> of 
        /// type <paramref name="targetType"/>.
        /// </summary>
        public static MemberGetter DelegateForGetFieldValue(this Type targetType, Flags flags, string fieldName)
        {
            return (MemberGetter)new MemberGetEmitter(targetType, flags, MemberTypes.Field, fieldName, false).GetDelegate();
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
            properties.ForEach( prop => targetType.SetFieldValue( prop.Name, prop.Get( sample ) ) );
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
            properties.ForEach( prop => target.SetFieldValue( prop.Name, prop.Get( sample ) ) );
            return target;
        }
        #endregion
        #endregion

        #region Field Lookup (Single)
        /// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
        /// searches for public and non-public instance fields on both the type itself and all parent classes.
        /// Use the <seealso href="FieldDeclared"/> method if you do not wish to search base types.  
        /// </summary>
        /// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
        public static FieldInfo Field( this Type type, string name )
        {
            return type.Field( name, Flags.InstanceCriteria );
        }

        /// <summary>
        /// Find the field identified by <paramref name="name"/> on the given <paramref name="type"/>. 
        /// Use the <paramref name="flags"/> parameter to define the scope of the search.
        /// </summary>
        /// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
        public static FieldInfo Field( this Type type, string name, Flags flags )
        {
			// we need to check all fields to do partial name matches
			if( flags.IsAnySet( Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented ) )
				return type.Fields( flags, name ).FirstOrDefault();

			var result = type.GetField( name, flags );
			if( result == null && flags.IsNotSet( Flags.DeclaredOnly ) )
			{ 
				if( type.BaseType != typeof(object) && type.BaseType != null )
        			return type.BaseType.Field( name, flags );
			}
			bool hasSpecialFlags = flags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
			if( hasSpecialFlags )
			{
				IList<FieldInfo> fields = new List<FieldInfo> { result };
				fields = fields.Filter( flags );
				return fields.Count > 0 ? fields[ 0 ] : null;
			}
			return result;
        }
        #endregion

        #region Field Lookup (Multiple)
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
        public static IList<FieldInfo> Fields( this Type type, Flags flags, params string[] names )
        {
			if( type == null || type == typeof(object) ) { return new FieldInfo[ 0 ]; }

			bool recurse = flags.IsNotSet( Flags.DeclaredOnly );
			bool hasNames = names != null && names.Length > 0;
			bool hasSpecialFlags = flags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
        	
			if( ! recurse && ! hasNames && ! hasSpecialFlags )
				return type.GetFields( flags ) ?? new FieldInfo[ 0 ];

			var fields = GetFields( type, flags );
			fields = hasSpecialFlags ? fields.Filter( flags ) : fields;
			fields = hasNames ? fields.Filter( flags, names ) : fields;
			return fields;
		}

        private static IList<FieldInfo> GetFields( Type type, Flags flags )
        {
			bool recurse = flags.IsNotSet( Flags.DeclaredOnly );

			if( ! recurse )
				return type.GetFields( flags ) ?? new FieldInfo[ 0 ];

			flags |= Flags.DeclaredOnly;
			flags &= ~BindingFlags.FlattenHierarchy;

        	var fields = new List<FieldInfo>();
			fields.AddRange( type.GetFields( flags ) );
			Type baseType = type.BaseType;
			while( baseType != null && baseType != typeof(object) )
			{
				fields.AddRange( baseType.GetFields( flags ) );
			    baseType = baseType.BaseType;
			}
			return fields;
		}
        #endregion

		#region Field Combined
		#region TryGetValue
		public static object TryGetFieldValue( this object source, string name )
		{
			return TryGetFieldValue( source, name, Flags.InstanceCriteria );
		}

		public static object TryGetFieldValue( this object source, string name, Flags flags )
		{
			Type type = source.GetType();
			var info = type.Field( name, flags );
			return info != null ? info.Get( source ) : null;
		}
		#endregion

		#region TrySetValue
		public static bool TrySetFieldValue( this object source, string name, object value )
		{
			return TrySetFieldValue( source, name, value, Flags.InstanceCriteria );
		}

		public static bool TrySetFieldValue( this object source, string name, object value, Flags flags )
		{
			Type type = source.GetType();
			var info = type.Field( name, flags );
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