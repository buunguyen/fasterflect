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
        /// Sets the static field specified by <paramref name="fieldName"/> of the given <paramref name="targetType"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="targetType"/>.</returns>
        public static Type SetFieldValue( this Type targetType, string fieldName, object value )
        {
            DelegateForSetStaticFieldValue( targetType, fieldName )( value );
            return targetType;
        }

        /// <summary>
        /// Sets the instance field specified by <paramref name="fieldName"/> of the given <paramref name="target"/>
        /// with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="target"/>.</returns>
        public static object SetFieldValue( this object target, string fieldName, object value )
        {
            DelegateForSetFieldValue( target.GetTypeAdjusted(), fieldName )( target, value );
            return target;
        }

        /// <summary>
        /// Gets the value of the static field specified by <paramref name="fieldName"/> of the given <paramref name="targetType"/>.
        /// </summary>
        public static object GetFieldValue( this Type targetType, string fieldName )
        {
            return DelegateForGetStaticFieldValue( targetType, fieldName )();
        }

        /// <summary>
        /// Gets the value of the instance field specified by <paramref name="fieldName"/> of the given <paramref name="target"/>.
        /// </summary>
        public static object GetFieldValue( this object target, string fieldName )
        {
            return DelegateForGetFieldValue( target.GetTypeAdjusted(), fieldName )( target );
        }

        /// <summary>
        /// Sets the static field specified by <paramref name="fieldName"/> and matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="targetType"/> with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="targetType"/>.</returns>
        public static Type SetFieldValue( this Type targetType, string fieldName, Flags bindingFlags, object value )
        {
            DelegateForSetStaticFieldValue( targetType, fieldName, bindingFlags )( value );
            return targetType;
        }

        /// <summary>
        /// Sets the instance field specified by <paramref name="fieldName"/> and matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="target"/> with the specified <paramref name="value" />.
        /// </summary>
        /// <returns><paramref name="target"/>.</returns>
        public static object SetFieldValue( this object target, string fieldName, Flags bindingFlags, object value )
        {
            DelegateForSetFieldValue( target.GetTypeAdjusted(), fieldName, bindingFlags )( target, value );
            return target;
        }

        /// <summary>
        /// Gets the value of the static field specified by <paramref name="fieldName"/> and matching <paramref name="bindingFlags"/> 
        /// of the given <paramref name="targetType"/>.
        /// </summary>
        public static object GetFieldValue( this Type targetType, string fieldName, Flags bindingFlags )
        {
            return DelegateForGetStaticFieldValue( targetType, fieldName, bindingFlags )();
        }

        /// <summary>
        /// Gets the value of the instance field specified by <paramref name="fieldName"/> and matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="target"/>.
        /// </summary>
        public static object GetFieldValue( this object target, string fieldName, Flags bindingFlags )
        {
            return DelegateForGetFieldValue( target.GetTypeAdjusted(), fieldName, bindingFlags )( target );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static field specified by <paramref name="fieldName"/> of 
        /// the given <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticFieldValue( this Type targetType, string fieldName )
        {
            return DelegateForSetStaticFieldValue( targetType, fieldName, Flags.StaticAnyVisibility );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance field specified by <paramref name="fieldName"/> of 
        /// the given <paramref name="targetType"/>.
        /// </summary>
        public static MemberSetter DelegateForSetFieldValue( this Type targetType, string fieldName )
        {
            return DelegateForSetFieldValue( targetType, fieldName, Flags.InstanceAnyVisibility );
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static field specified by <paramref name="fieldName"/> of 
        /// the given <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticFieldValue( this Type targetType, string fieldName )
        {
            return DelegateForGetStaticFieldValue( targetType, fieldName, Flags.StaticAnyVisibility );
        }

        /// <summary>
        /// Creates a delegate which can get the value of the instance field specified by <paramref name="fieldName"/> of 
        /// the given <paramref name="targetType"/>.
        /// </summary>
        public static MemberGetter DelegateForGetFieldValue( this Type targetType, string fieldName )
        {
            return DelegateForGetFieldValue( targetType, fieldName, Flags.InstanceAnyVisibility );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static field specified by <paramref name="fieldName"/> and 
        /// matching <paramref name="bindingFlags"/> of the given <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticFieldValue( this Type targetType, string fieldName,
                                                                         Flags bindingFlags )
        {
            return (StaticMemberSetter)
                new MemberSetEmitter( targetType, bindingFlags, MemberTypes.Field, fieldName ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance field specified by <paramref name="fieldName"/> and
        /// matching <paramref name="bindingFlags"/> of the given <paramref name="targetType"/>.
        /// </summary>
        public static MemberSetter DelegateForSetFieldValue( this Type targetType, string fieldName, Flags bindingFlags )
        {
            return (MemberSetter)
                new MemberSetEmitter( targetType, bindingFlags, MemberTypes.Field, fieldName ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static field specified by <paramref name="fieldName"/> and 
        /// matching <paramref name="bindingFlags"/> of the given <paramref name="targetType"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticFieldValue( this Type targetType, string fieldName,
                                                                         Flags bindingFlags )
        {
            return
                (StaticMemberGetter)
                new MemberGetEmitter( targetType, bindingFlags, MemberTypes.Field, fieldName ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the instance field specified by <paramref name="fieldName"/> and
        /// matching <paramref name="bindingFlags"/> of the given <paramref name="targetType"/>.
        /// </summary>
        public static MemberGetter DelegateForGetFieldValue( this Type targetType, string fieldName, Flags bindingFlags )
        {
            return (MemberGetter)
                new MemberGetEmitter( targetType, bindingFlags, MemberTypes.Field, fieldName ).GetDelegate();
        }
        #endregion

        #region Batch Setters
        /// <summary>
        /// Sets the public and non-public static fields of the given <paramref name="targetType"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by 
        /// the optional list <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="targetType">The type whose static fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// static fields of the given <paramref name="targetType"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="targetType"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The type whose static fields are to be set.</returns>
        public static Type SetFields( this Type targetType, object sample, params string[] propertiesToInclude )
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach( prop => targetType.SetFieldValue( prop.Name, prop.Get( sample ) ) );
            return targetType;
        }

        /// <summary>
        /// Sets the static fields matching <paramref name="bindingFlags"/> of the given <paramref name="targetType"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by 
        /// the optional list <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="targetType">The type whose static fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// static fields of the given <paramref name="targetType"/>.</param>
        /// <param name="bindingFlags">The binding flag used to lookup the static fields of <paramref name="targetType"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="targetType"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The type whose static fields are to be set.</returns>
        public static Type SetFields(this Type targetType, object sample, Flags bindingFlags, params string[] propertiesToInclude)
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach(prop => targetType.SetFieldValue(prop.Name, bindingFlags, prop.Get(sample)));
            return targetType;
        }

        /// <summary>
        /// Sets the public and non-public instance fields of the given <paramref name="target"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by the optional list 
        /// <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="target">The object whose instance fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// instance fields of the given <paramref name="target"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="target"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The object whose instance fields are to be set.</returns>
        public static object SetFields( this object target, object sample, params string[] propertiesToInclude )
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach( prop => target.SetFieldValue( prop.Name, prop.Get( sample ) ) );
            return target;
        }

        /// <summary>
        /// Sets the instance fields matching <paramref name="bindingFlags"/> of the given <paramref name="target"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by the optional list 
        /// <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="target">The object whose instance fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// instance fields of the given <paramref name="target"/>.</param>
        /// <param name="bindingFlags">The binding flag used to lookup the instance fields of <paramref name="target"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="target"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The object whose instance fields are to be set.</returns>
        public static object SetFields(this object target, object sample, Flags bindingFlags, params string[] propertiesToInclude)
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach(prop => target.SetFieldValue(prop.Name, bindingFlags, prop.Get(sample)));
            return target;
        }
        #endregion

        #endregion

        #region Field Lookup (Single)
        /// <summary>
        /// Gets the field identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
        /// searches for public and non-public instance fields on both the type itself and all parent classes.
        /// </summary>
        /// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
        public static FieldInfo Field( this Type type, string name )
        {
            return type.Field( name, Flags.InstanceAnyVisibility );
        }

        /// <summary>
        /// Gets the field identified by <paramref name="name"/> on the given <paramref name="type"/>. 
        /// Use the <paramref name="bindingFlags"/> parameter to define the scope of the search.
        /// </summary>
        /// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
        public static FieldInfo Field( this Type type, string name, Flags bindingFlags )
        {
            // we need to check all fields to do partial name matches
            if( bindingFlags.IsAnySet( Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented ) )
            {
                return type.Fields( bindingFlags, name ).FirstOrDefault();
            }

            var result = type.GetField( name, bindingFlags );
            if( result == null && bindingFlags.IsNotSet( Flags.DeclaredOnly ) )
            {
                if( type.BaseType != typeof(object) && type.BaseType != null )
                {
                    return type.BaseType.Field( name, bindingFlags );
                }
            }
            bool hasSpecialFlags =
                bindingFlags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
            if( hasSpecialFlags )
            {
                IList<FieldInfo> fields = new List<FieldInfo> { result };
                fields = fields.Filter( bindingFlags );
                return fields.Count > 0 ? fields[ 0 ] : null;
            }
            return result;
        }
        #endregion

        #region Field Lookup (Multiple)
        /// <summary>
        /// Gets all public and non-public instance fields on the given <paramref name="type"/>,
        /// including fields defined on base types.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> to include explicitly implemented 
		/// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
		/// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all instance fields on the type. This value will never be null.</returns>
        public static IList<FieldInfo> Fields( this Type type, params string[] names )
        {
            return type.Fields( Flags.InstanceAnyVisibility, names );
        }

        /// <summary>
        /// Gets all fields on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> to include explicitly implemented 
		/// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
		/// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching fields on the type. This value will never be null.</returns>
        public static IList<FieldInfo> Fields( this Type type, Flags bindingFlags, params string[] names )
        {
            if( type == null || type == typeof(object) )
            {
                return new FieldInfo[0];
            }

            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );
            bool hasNames = names != null && names.Length > 0;
            bool hasSpecialFlags =
                bindingFlags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );

            if( ! recurse && ! hasNames && ! hasSpecialFlags )
            {
                return type.GetFields( bindingFlags ) ?? new FieldInfo[0];
            }

            var fields = GetFields( type, bindingFlags );
            fields = hasSpecialFlags ? fields.Filter( bindingFlags ) : fields;
            fields = hasNames ? fields.Filter( bindingFlags, names ) : fields;
            return fields;
        }

        private static IList<FieldInfo> GetFields( Type type, Flags bindingFlags )
        {
            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );

            if( ! recurse )
            {
                return type.GetFields( bindingFlags ) ?? new FieldInfo[0];
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var fields = new List<FieldInfo>();
            fields.AddRange( type.GetFields( bindingFlags ) );
            Type baseType = type.BaseType;
            while( baseType != null && baseType != typeof(object) )
            {
                fields.AddRange( baseType.GetFields( bindingFlags ) );
                baseType = baseType.BaseType;
            }
            return fields;
        }
        #endregion

        #region Field Combined

        #region TryGetValue
		/// <summary>
        /// Gets the first (public or non-public) instance field with the given <paramref name="name"/> on the given
        /// <paramref name="source"/> object. Returns the value of the field if a match was found and null otherwise.
		/// </summary>
		/// <remarks>
        /// When using this method it is not possible to distinguish between a missing field and a field whose value is null.
		/// </remarks>
		/// <param name="source">The source object on which to find the field</param>
		/// <param name="name">The name of the field whose value should be retrieved</param>
		/// <returns>The value of the field or null if no field was found</returns>
		public static object TryGetFieldValue( this object source, string name )
        {
            return TryGetFieldValue( source, name, Flags.InstanceAnyVisibility );
        }

		/// <summary>
        /// Gets the first field with the given <paramref name="name"/> on the given <paramref name="source"/> object.
        /// Returns the value of the field if a match was found and null otherwise.
        /// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <remarks>
        /// When using this method it is not possible to distinguish between a missing field and a field whose value is null.
		/// </remarks>
		/// <param name="source">The source object on which to find the field</param>
		/// <param name="name">The name of the field whose value should be retrieved</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>The value of the field or null if no field was found</returns>
        public static object TryGetFieldValue( this object source, string name, Flags bindingFlags )
        {
            try
            {
                return source.GetFieldValue( name, bindingFlags );
            }
            catch( MissingFieldException )
            {
                return null;
            }
        }
        #endregion

        #region TrySetValue
		/// <summary>
        /// Sets the first (public or non-public) instance field with the given <paramref name="name"/> on the 
        /// given <paramref name="source"/> object to supplied <paramref name="value"/>. Returns true if a value
        /// was assigned to a field and false otherwise.
		/// </summary>
		/// <param name="source">The source object on which to find the field</param>
		/// <param name="name">The name of the field whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the field</param>
		/// <returns>True if the value was assigned to a field and false otherwise</returns>
        public static bool TrySetFieldValue( this object source, string name, object value )
        {
            return TrySetFieldValue( source, name, value, Flags.InstanceAnyVisibility );
        }

		/// <summary>
        /// Sets the first field with the given <paramref name="name"/> on the given <paramref name="source"/> object
        /// to the supplied <paramref name="value"/>. Returns true if a value was assigned to a field and false otherwise.
        /// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <param name="source">The source object on which to find the field</param>
		/// <param name="name">The name of the field whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the field</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>True if the value was assigned to a field and false otherwise</returns>
        public static bool TrySetFieldValue( this object source, string name, object value, Flags bindingFlags )
        {
            try
            {
                source.SetFieldValue(name, bindingFlags, value );
                return true;
            }
            catch( MissingFieldException )
            {
                return false;
            }
        }
        #endregion

        #endregion
    }
}