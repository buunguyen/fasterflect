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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
    internal static class ReflectorUtils
    {
        #region Fields
        public static readonly Type[] ArrayOfObjectType = new[] { typeof(object) };
        public static readonly Type[] EmptyTypeArray = new Type[0];
        public static readonly object[] EmptyObjectArray = new object[0];
        public const string IndexerSetterName = "set_Item";
        public const string IndexerGetterName = "get_Item";
        #endregion

        #region Reflection
        public static List<PropertyInfo> GetProperties(this object sample)
        {
            return sample.GetType().GetProperties().ToList();
        }

        public static object GetValue(this PropertyInfo prop, object sample)
        {
            return prop.GetGetMethod().Invoke(sample, EmptyObjectArray);
        }
        #endregion

        #region CallInfo Construction
        public static CallInfo CreateStaticSetCallInfo(Type targetType, MemberTypes memberTypes, string fieldOrProperty, object value)
        {
            return new CallInfo(targetType, memberTypes, fieldOrProperty, ArrayOfObjectType)
                        {
                            IsStatic = true, 
                            Parameters = new[] { value }
                        };
        }

        public static CallInfo CreateStaticGetCallInfo(Type targetType, MemberTypes memberTypes,
            string fieldOrPropertyName)
        {
            return new CallInfo(targetType, memberTypes, fieldOrPropertyName)
                        {
                           IsStatic = true
                        };
        }

        public static CallInfo CreateSetCallInfo(Type targetType, object target, MemberTypes memberTypes, string fieldOrProperty, object value)
        {
            return new CallInfo(targetType, memberTypes, fieldOrProperty, ArrayOfObjectType)
                        {
                            Target = target,
                            Parameters = new[] { value }
                        };
        }

        public static CallInfo CreateGetCallInfo(Type targetType, object target, MemberTypes memberTypes, string fieldOrPropertyName)
        {
            return new CallInfo(targetType, memberTypes, fieldOrPropertyName)
                        {
                           Target = target
                        };
        }

        public static CallInfo GetConstructorCallInfo(Type targetType, Type[] paramTypes,
            object[] parameters)
        {
            return new CallInfo(targetType, MemberTypes.Constructor, targetType.Name, paramTypes)
                        {
                            Parameters = parameters
                        };
        }

        public static CallInfo CreateIndexerSetCallInfo(Type targetType, object target, Type[] paramTypes, object[] parameters)
        {
            return new CallInfo(targetType, MemberTypes.Method, IndexerSetterName, paramTypes)
            {
                Parameters = parameters,
                Target = target
            };
        }

        public static CallInfo CreateIndexerGetCallInfo(Type targetType, object target, Type[] paramTypes, object[] parameters)
        {
            return new CallInfo(targetType, MemberTypes.Method, IndexerGetterName, paramTypes)
            {
                Parameters = parameters,
                Target = target
            };
        }

        public static CallInfo CreateStaticMethodCallInfo(Type targetType, string methodName, Type[] paramTypes, object[] parameters)
        {
            return new CallInfo(targetType, MemberTypes.Method, methodName, paramTypes)
            {
                IsStatic = true,
                Parameters = parameters
            };
        }

        public static CallInfo CreateMethodCallInfo(Type targetType, object target, string methodName, Type[] paramTypes, object[] parameters)
        {
            return new CallInfo(targetType, MemberTypes.Method, methodName, paramTypes)
            {
                Parameters = parameters,
                Target = target
            };
        }
        #endregion
    }
}
