#region License
// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
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
    /// Stores all necessary information to construct a dynamic method and optionally
    /// parameters used to invoke that method.
    /// </summary>
    internal class CallInfo
    {
        private static readonly Type[] EmptyTypeArray = new Type[0];

        public CallInfo(Type targetType, MemberTypes memberTypes, string name)
            : this(targetType, memberTypes, name, EmptyTypeArray)
        {
        }

        public CallInfo(Type targetType, MemberTypes memberTypes, string name, Type[] paramTypes)
        {
            TargetType = targetType;
            MemberTypes = memberTypes;
            Name = name;
            ParamTypes = paramTypes.Length == 0 ? EmptyTypeArray : paramTypes;
        }

        public Type TargetType { get; private set; }
        public MemberTypes MemberTypes { get; set; }
        public Type[] ParamTypes { get; private set; }
        public bool HasNoParam { get { return ParamTypes == EmptyTypeArray; } }
        public string Name { get; private set; }
        public bool IsStatic { get; set; }
        public object Target { get; set; }
        public object[] Parameters { get; set; }

        public bool IsTargetTypeStruct
        {
            get { return TargetType.IsValueType; }
        }

        /// <summary>
        /// Returns the actual type that is to be used for the CIL invocation.
        /// If there's an inner struct, the inner struct type is returned.
        /// </summary>
        public Type ActualTargetType 
        { 
            get
            {
                if (ShouldHandleInnerStruct && Target is ValueTypeHolder)
                {
                    return ((ValueTypeHolder)Target).Value.GetType();
                }
                return TargetType;
            }
        }

        /// <summary>
        /// The CIL should handle inner struct only when the target type is 
        /// a value type or the wrapper ValueTypeHolder type.  In addition, the call 
        /// must also be executed in the non-static context since static 
        /// context doesn't need to handle inner struct case (cos' it has no arg).
        /// </summary>
        public bool ShouldHandleInnerStruct
        {
            get
            {
                return ((TargetType == typeof(ValueTypeHolder) && Target != null) || 
                        TargetType.IsValueType) && 
                       !IsStatic;
            }
        }

        public bool HasRefParam
        {
            get { return ParamTypes.Any(t => t.IsByRef); }
        }

        /// <summary>
        /// Two <c>CallInfo</c> instances are considered equaled if the following properties
        /// are equaled: <c>TargetType</c>, <c>MemberTypes</c>, <c>Name</c>,
        /// and <c>ParamTypes</c>.
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as CallInfo;
            if (other == null) return false;
            if (other == this) return true;
            if (other.TargetType != TargetType || other.MemberTypes != MemberTypes || 
                other.Name != Name || other.ParamTypes.Length != ParamTypes.Length)
                return false;
            for (int i = 0; i < ParamTypes.Length; i++)
                if (ParamTypes[i] != other.ParamTypes[i])
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 7;
            hashCode = 31 * hashCode + TargetType.GetHashCode();
            hashCode = 31 * hashCode + Name.GetHashCode();
            for (int i = 0; i < ParamTypes.Length; i++)
                hashCode = 31 * hashCode + ParamTypes[i].GetHashCode();
            return hashCode;
        }
    }
}
