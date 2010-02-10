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

using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
    public static class FieldInfoExtensions
    {
        /// <summary>
        /// Sets the static field identified by <paramref name="fieldInfo"/> with <paramref name="value" />.
        /// </summary>
        public static void Set(this FieldInfo fieldInfo, object value)
        {
            fieldInfo.DelegateForSetStaticFieldValue()(value);
        }

        /// <summary>
        /// Sets the instance field identified by <paramref name="fieldInfo"/> of <paramref name="target"/>
        /// with <paramref name="value" />.
        /// </summary>
        public static void Set(this FieldInfo fieldInfo, object target, object value)
        {
            fieldInfo.DelegateForSetFieldValue()(target, value);
        }

        /// <summary>
        /// Gets the value of the static field identified by <paramref name="fieldInfo"/>.
        /// </summary>
        public static object Get(this FieldInfo fieldInfo)
        {
            return fieldInfo.DelegateForGetStaticFieldValue()();
        }

        /// <summary>
        /// Gets the value of the instance field identified by <paramref name="fieldInfo"/> of <paramref name="target"/>.
        /// </summary>
        public static object Get(this FieldInfo fieldInfo, object target)
        {
            return fieldInfo.DelegateForGetFieldValue()( target );
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static field identified by <paramref name="fieldInfo"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticFieldValue(this FieldInfo fieldInfo)
        {
            return (StaticMemberSetter)new MemberSetEmitter(fieldInfo, true).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance field identified by <paramref name="fieldInfo"/>.
        /// </summary>
        public static MemberSetter DelegateForSetFieldValue(this FieldInfo fieldInfo)
        {
            return (MemberSetter)new MemberSetEmitter(fieldInfo, false).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static field identified by <paramref name="fieldInfo"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticFieldValue(this FieldInfo fieldInfo)
        {
            return (StaticMemberGetter)new MemberGetEmitter(fieldInfo, true).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static field identified by <paramref name="fieldInfo"/>.
        /// </summary>
        public static MemberGetter DelegateForGetFieldValue(this FieldInfo fieldInfo)
        {
            return (MemberGetter)new MemberGetEmitter(fieldInfo, false).GetDelegate();
        }
    }
}
