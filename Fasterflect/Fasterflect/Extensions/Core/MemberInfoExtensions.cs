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
    /// Extension methods for inspecting and working with members.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the static field or property identified by <paramref name="memberInfo"/>.
        /// </summary>
        public static object Get( this MemberInfo memberInfo )
        {
            var @delegate = (StaticMemberGetter) new MemberGetEmitter( memberInfo, Flags.StaticAnyVisibility ).GetDelegate();
            return @delegate();
        }

        /// <summary>
        /// Sets the static field or property identified by <paramref name="memberInfo"/> with <paramref name="value"/>.
        /// </summary>
        public static void Set( this MemberInfo memberInfo, object value )
        {
            var @delegate = (StaticMemberSetter) new MemberSetEmitter( memberInfo, Flags.StaticAnyVisibility ).GetDelegate();
            @delegate( value );
        }

        /// <summary>
        /// Gets the instance field or property identified by <paramref name="memberInfo"/> on
        /// the <paramref name="target"/>.
        /// </summary>
        public static object Get( this MemberInfo memberInfo, object target )
        {
            var @delegate = (MemberGetter) new MemberGetEmitter( memberInfo, Flags.InstanceAnyVisibility ).GetDelegate();
            return @delegate( target );
        }

        /// <summary>
        /// Sets the instance field or property identified by <paramref name="memberInfo"/> on
        /// the <paramref name="target"/> object with <paramref name="value"/>.
        /// </summary>
        public static void Set( this MemberInfo memberInfo, object target, object value )
        {
            var @delegate = (MemberSetter) new MemberSetEmitter( memberInfo, Flags.InstanceAnyVisibility ).GetDelegate();
            @delegate( target, value );
        }

        #region MemberInfo Helpers
        /// <summary>
        /// Get the system type of the field or property identified by the <paramref name="member"/>.
        /// </summary>
        /// <returns>The system type of the member.</returns>
        public static Type Type( this MemberInfo member )
        {
            var field = member as FieldInfo;
            if( field != null )
            {
                return field.FieldType;
            }
            var property = member as PropertyInfo;
            if( property != null )
            {
                return property.PropertyType;
            }
            throw new NotSupportedException( "Can only determine the type for fields and properties." );
        }

        /// <summary>
        /// Find out whether a value can be read from the field or property identified by
        /// the <paramref name="member"/>.
        /// </summary>
        /// <returns>True for fields and readable properties, false otherwise.</returns>
        public static bool CanRead( this MemberInfo member )
        {
            var property = member as PropertyInfo;
            return member is FieldInfo || (property != null && property.CanRead);
        }

        /// <summary>
        /// Find out whether a value can be assigned to the field or property identified by
        /// the <paramref name="member"/>.
        /// </summary>
        /// <returns>True for fields and writable properties, false otherwise.</returns>
        public static bool CanWrite( this MemberInfo member )
        {
            var property = member as PropertyInfo;
            return member is FieldInfo || (property != null && property.CanWrite);
        }

        /// <summary>
        /// Determines whether the given <paramref name="member"/> has the given <paramref name="name"/>.
        /// The comparison uses OrdinalIgnoreCase and allows for a leading underscore in either name
        /// to be ignored.
        /// </summary>
        /// <returns>True if the name is considered identical, false otherwise. If either parameter
        /// is null an exception will be thrown.</returns>
        public static bool HasName( this MemberInfo member, string name )
        {
            string memberName = member.Name.Length > 0 && member.Name[ 0 ] == '_'
                                    ? member.Name.Substring( 1 )
                                    : member.Name;
            name = name.Length > 0 && name[ 0 ] == '_' ? name.Substring( 1 ) : name;
            return memberName.Equals( name, StringComparison.OrdinalIgnoreCase );
        }
        #endregion
    }
}