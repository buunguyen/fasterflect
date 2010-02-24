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
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Sets the static property identified by <paramref name="propInfo"/> with <paramref name="value" />.
        /// </summary>
        public static void Set( this PropertyInfo propInfo, object value )
        {
            propInfo.DelegateForSetStaticPropertyValue()( value );
        }

        /// <summary>
        /// Sets the instance property identified by <paramref name="propInfo"/> of the given <paramref name="target"/>
        /// with <paramref name="value" />.
        /// </summary>
        public static void Set( this PropertyInfo propInfo, object target, object value )
        {
            propInfo.DelegateForSetPropertyValue()( target, value );
        }

        /// <summary>
        /// Gets the value of the static property identified by <paramref name="propInfo"/>.
        /// </summary>
        public static object Get( this PropertyInfo propInfo )
        {
            return propInfo.DelegateForGetStaticPropertyValue()();
        }

        /// <summary>
        /// Gets the value of the instance property identified by <paramref name="propInfo"/> of the given <paramref name="target"/>.
        /// </summary>
        public static object Get( this PropertyInfo propInfo, object target )
        {
            return propInfo.DelegateForGetPropertyValue()( target );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static property identified by <paramref name="propInfo"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticPropertyValue( this PropertyInfo propInfo )
        {
            return (StaticMemberSetter) new MemberSetEmitter( propInfo, Flags.StaticAnyVisibility ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance property <paramref name="propInfo"/>.
        /// </summary>
        public static MemberSetter DelegateForSetPropertyValue( this PropertyInfo propInfo )
        {
            return (MemberSetter) new MemberSetEmitter( propInfo, Flags.InstanceAnyVisibility ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static property <paramref name="propInfo"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticPropertyValue( this PropertyInfo propInfo )
        {
            return (StaticMemberGetter) new MemberGetEmitter( propInfo, Flags.StaticAnyVisibility ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static property <paramref name="propInfo"/>.
        /// </summary>
        public static MemberGetter DelegateForGetPropertyValue( this PropertyInfo propInfo )
        {
            return (MemberGetter) new MemberGetEmitter( propInfo, Flags.InstanceAnyVisibility ).GetDelegate();
        }
    }
}