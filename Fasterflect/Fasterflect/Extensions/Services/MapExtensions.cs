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
using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
    /// <summary>
    /// Extension methods for mapping (copying) members from one object instance to another.
    /// </summary>
    public static class MapExtensions
    {
		#region Map
		public static void Map( this object source, object target, MemberTypes sourceTypes, MemberTypes targetTypes, 
								Flags bindingFlags, params string[] names )
		{
            if( source == null || target == null )
            {
                throw new ArgumentException( "Unable to map members to or from a null instance." );
            }
			var emitter = new MapEmitter( source.GetType(), target.GetType(), sourceTypes, targetTypes, bindingFlags, names );
            var copier = emitter.GetDelegate();
            copier( source, target );
		}
    	#endregion

		#region Map Companions
		public static void Map( this object source, object target, params string[] names )
		{
			const MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property;
			source.Map( target, memberTypes, memberTypes, Flags.InstanceAnyVisibility, names );
		}

		public static void Map( this object source, object target, Flags bindingFlags, params string[] names )
		{
			const MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property;
			source.Map( target, memberTypes, memberTypes, bindingFlags, names );
		}

    	public static void MapFields( this object source, object target, params string[] names )
		{
			source.Map( target, MemberTypes.Field, MemberTypes.Field, Flags.InstanceAnyVisibility, names );
		}

		public static void MapProperties( this object source, object target, params string[] names )
		{
			source.Map( target, MemberTypes.Property, MemberTypes.Property, Flags.InstanceAnyVisibility, names );
		}

		public static void MapFieldsToProperties( this object source, object target, params string[] names )
		{
			source.Map( target, MemberTypes.Field, MemberTypes.Property, Flags.InstanceAnyVisibility, names );
		}

		public static void MapPropertiesToFields( this object source, object target, params string[] names )
		{
			source.Map( target, MemberTypes.Property, MemberTypes.Field, Flags.InstanceAnyVisibility, names );
		}
    	#endregion

		/*
		#region Batch Field Setters
        /// <summary>
        /// Sets the public and non-public static fields of the given <paramref name="type"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by 
        /// the optional list <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="type">The type whose static fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// static fields of the given <paramref name="type"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="type"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The type whose static fields are to be set.</returns>
        public static Type SetFields( this Type type, object sample, params string[] propertiesToInclude )
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach( prop => type.SetFieldValue( prop.Name, prop.Get( sample ) ) );
            return type;
        }

        /// <summary>
        /// Sets the static fields matching <paramref name="bindingFlags"/> of the given <paramref name="type"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by 
        /// the optional list <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="type">The type whose static fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// static fields of the given <paramref name="type"/>.</param>
        /// <param name="bindingFlags">The binding flag used to lookup the static fields of <paramref name="type"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="type"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The type whose static fields are to be set.</returns>
        public static Type SetFields(this Type type, object sample, Flags bindingFlags, params string[] propertiesToInclude)
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach(prop => type.SetFieldValue(prop.Name, prop.Get(sample), bindingFlags ));
            return type;
        }

        /// <summary>
        /// Sets the public and non-public instance fields of the given <paramref name="obj"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by the optional list 
        /// <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="obj">The object whose instance fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// instance fields of the given <paramref name="obj"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="obj"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The object whose instance fields are to be set.</returns>
        public static object SetFields( this object obj, object sample, params string[] propertiesToInclude )
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach( prop => obj.SetFieldValue( prop.Name, prop.Get( sample ) ) );
            return obj;
        }

        /// <summary>
        /// Sets the instance fields matching <paramref name="bindingFlags"/> of the given <paramref name="obj"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by the optional list 
        /// <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="obj">The object whose instance fields are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// instance fields of the given <paramref name="obj"/>.</param>
        /// <param name="bindingFlags">The binding flag used to lookup the instance fields of <paramref name="obj"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="obj"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The object whose instance fields are to be set.</returns>
        public static object SetFields(this object obj, object sample, Flags bindingFlags, params string[] propertiesToInclude)
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach(prop => obj.SetFieldValue(prop.Name, prop.Get(sample), bindingFlags ));
            return obj;
        }
        #endregion

		#region CopyFields
        /// <summary>
        /// Copies all public and non-public instance fields, including those defined on base classes,
        /// from the <paramref name="source"/> object to the <paramref name="target"/> object.
        /// </summary>
        public static void CopyFields( this object source, object target )
        {
            source.CopyFields( target, Flags.InstanceAnyVisibility );
        }

        /// <summary>
        /// Copies all fields matching the specified <paramref name="bindingFlags"/>, including those 
        /// defined on base classes, from the <paramref name="source"/> object to the <paramref name="target"/>
        /// object.
        /// </summary>
        public static void CopyFields( this object source, object target, Flags bindingFlags )
        {
            if( source == null || target == null )
            {
                throw new ArgumentException( "Unable to copy to or from null instance." );
            }
            var copier = (MemberCopier)
                new MapEmitter( source.GetType(), target.GetType(), bindingFlags, MemberTypes.Field ).GetDelegate();
            copier( source, target );
        }
        #endregion

		#region Batch Setters
        /// <summary>
        /// Sets the public and non-public static properties of the given <paramref name="type"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by 
        /// the optional list <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="type">The type whose static properties are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// static properties of the given <paramref name="type"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="type"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The type whose static properties are to be set.</returns>
        public static Type SetProperties( this Type type, object sample, params string[] propertiesToInclude )
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach(prop => type.SetPropertyValue(prop.Name, prop.Get(sample)));
            return type;
        }

        /// <summary>
        /// Sets the static properties matching <paramref name="bindingFlags"/> of the given <paramref name="type"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by 
        /// the optional list <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="type">The type whose static properties are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// static properties of the given <paramref name="type"/>.</param>
        /// <param name="bindingFlags">The binding flag used to lookup the static properties of <paramref name="type"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="type"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The type whose static properties are to be set.</returns>
        public static Type SetProperties(this Type type, object sample, Flags bindingFlags, params string[] propertiesToInclude)
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach(prop => type.SetPropertyValue(prop.Name, prop.Get(sample), bindingFlags ));
            return type;
        }

        /// <summary>
        /// Sets the public and non-public instance properties of the given <paramref name="obj"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by the optional list 
        /// <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="obj">The object whose instance properties are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// instance properties of the given <paramref name="obj"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="obj"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The object whose instance properties are to be set.</returns>
        public static object SetProperties(this object obj, object sample, params string[] propertiesToInclude)
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach(prop => obj.SetPropertyValue(prop.Name, prop.Get(sample)));
            return obj;
        }

        /// <summary>
        /// Sets the instance properties matching <paramref name="bindingFlags"/> of the given <paramref name="obj"/> based on
        /// the public properties available in <paramref name="sample"/> filtered by the optional list 
        /// <paramref name="propertiesToInclude"/>. 
        /// </summary>
        /// <param name="obj">The object whose instance properties are to be set.</param>
        /// <param name="sample">An object whose public properties will be used to set the 
        /// instance properties of the given <paramref name="obj"/>.</param>
        /// <param name="bindingFlags">The binding flag used to lookup the instance properties of <paramref name="obj"/>.</param>
        /// <param name="propertiesToInclude">An optional list of names of public properties to retrieve from 
        /// <paramref name="sample"/> to set <paramref name="obj"/>.  If this is <c>null</c> or left empty, 
        /// all public properties of <paramref name="sample"/> are used.</param>
        /// <returns>The object whose instance properties are to be set.</returns>
        public static object SetProperties(this object obj, object sample, Flags bindingFlags, params string[] propertiesToInclude)
        {
            var properties = sample.GetType().Properties(Flags.Instance | Flags.Public, propertiesToInclude);
            properties.ForEach(prop => obj.SetPropertyValue(prop.Name, prop.Get(sample), bindingFlags ));
            return obj;
        }
        #endregion

        #region CopyProperties
        /// <summary>
        /// Copies all public and non-public instance properties, including those defined on base classes,
        /// from the <paramref name="source"/> object to the <paramref name="target"/> object.
        /// </summary>
        public static void CopyProperties( this object source, object target )
        {
            source.CopyProperties( target, Flags.InstanceAnyVisibility );
        }

        /// <summary>
        /// Copies all properties matching the specified <paramref name="bindingFlags"/> from the 
        /// <paramref name="source"/> object to the <paramref name="target"/> object. 
        /// </summary>
        public static void CopyProperties( this object source, object target, Flags bindingFlags )
        {
            if( source == null || target == null )
            {
                throw new ArgumentException( "Unable to copy to or from null instance." );
            }
            var copier = (MemberCopier)
                new MapEmitter( source.GetType(), target.GetType(), bindingFlags, MemberTypes.Property ).GetDelegate();
            copier( source, target );
        }
        #endregion
		*/
    }
}