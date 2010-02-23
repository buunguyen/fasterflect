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
using System.Linq;
using System.Reflection;

namespace Fasterflect.Emitter
{
    /// <summary>
    /// Stores all necessary information to construct a dynamic method.
    /// </summary>
    internal class CallInfo
    {
        public Type TargetType { get; private set; }
        public Flags BindingFlags { get; private set; }
        public MemberTypes MemberTypes { get; set; }
        public Type[] ParamTypes { get; private set; }
        public string Name { get; private set; }
        public MemberInfo MemberInfo { get; private set; }

        public CallInfo( Type targetType, Flags bindingFlags, MemberTypes memberTypes, string name,
                         Type[] parameterTypes, MemberInfo memberInfo )
        {
            TargetType = targetType;
            BindingFlags = bindingFlags;
            MemberTypes = memberTypes;
            Name = name;
            ParamTypes = parameterTypes == null || parameterTypes.Length == 0
                             ? Type.EmptyTypes
                             : parameterTypes;
            MemberInfo = memberInfo;
        }

        /// <summary>
        /// The CIL should handle inner struct only when the target type is 
        /// a value type or the wrapper ValueTypeHolder type.  In addition, the call 
        /// must also be executed in the non-static context since static 
        /// context doesn't need to handle inner struct case.
        /// </summary>
        public bool ShouldHandleInnerStruct
        {
            get { return IsTargetTypeStruct && !IsStatic; }
        }

        public bool IsStatic
        {
            get { return BindingFlags.IsSet( Flags.Static ); }
        }

        public bool IsTargetTypeStruct
        {
            get { return TargetType.IsValueType; }
        }

        public bool HasNoParam
        {
            get { return ParamTypes == Type.EmptyTypes; }
        }

        public bool HasRefParam
        {
            get { return ParamTypes.Any( t => t.IsByRef ); }
        }

        /// <summary>
        /// Two <c>CallInfo</c> instances are considered equaled if the following properties
        /// are equaled: <c>TargetType</c>, <c>Flags</c>, <c>IsStatic</c>, <c>MemberTypes</c>, <c>Name</c>,
        /// and <c>ParamTypes</c>.
        /// </summary>
        public override bool Equals( object obj )
        {
            var other = obj as CallInfo;
            if( other == null )
            {
                return false;
            }
            if( other == this )
            {
                return true;
            }

            if( other.MemberInfo != MemberInfo ||
                other.TargetType != TargetType ||
                other.Name != Name ||
                other.MemberTypes != MemberTypes ||
                other.BindingFlags != BindingFlags ||
                other.ParamTypes.Length != ParamTypes.Length )
            {
                return false;
            }

            for( int i = 0; i < ParamTypes.Length; i++ )
            {
                if( ParamTypes[ i ] != other.ParamTypes[ i ] )
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = TargetType.GetHashCode() + (int) MemberTypes * Name.GetHashCode() + BindingFlags.GetHashCode();
            if( MemberInfo != null )
            {
                hash += MemberInfo.GetHashCode();
            }
            for( int i = 0; i < ParamTypes.Length; i++ )
            {
                hash += ParamTypes[ i ].GetHashCode();
            }
            return hash;
        }
    }
}