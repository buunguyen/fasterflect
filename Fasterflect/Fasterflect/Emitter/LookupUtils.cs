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

namespace Fasterflect.Emitter
{
    internal class LookupUtils
    {
        public static ConstructorInfo GetConstructor(CallInfo callInfo)
        {
            ConstructorInfo ctorInfo = callInfo.MemberInfo as ConstructorInfo;
            if (ctorInfo != null)
                return ctorInfo;

            ctorInfo = callInfo.TargetType.GetConstructor(
                Flags.InstanceCriteria | Flags.ParameterMatch,
                null, CallingConventions.HasThis, callInfo.ParamTypes, null);
            if (ctorInfo == null)
                throw new MissingMemberException("Constructor does not exist");
            return ctorInfo;
        }

        public static MethodInfo GetMethod(CallInfo callInfo)
        {
            MethodInfo methodInfo = callInfo.MemberInfo as MethodInfo;
            if (methodInfo != null)
                return methodInfo;

            methodInfo = callInfo.TargetType.Method( callInfo.Name, callInfo.ScopeFlag | Flags.DefaultCriteria, callInfo.ParamTypes, null );
            if (methodInfo == null)
                throw new MissingMethodException(callInfo.IsStatic
                                                    ? "Static method "
                                                    : "Method " + callInfo.Name + " does not exist");
            return methodInfo;
        }

        public static MemberInfo GetMember(CallInfo callInfo)
        {
            MemberInfo memberInfo = callInfo.MemberInfo;
            if (memberInfo != null)
                return memberInfo;

            if (callInfo.MemberTypes == MemberTypes.Property)
            {
                memberInfo = callInfo.TargetType.Property(callInfo.Name, Flags.DefaultCriteria | callInfo.ScopeFlag);
                if (memberInfo == null)
                        throw new MissingMemberException((callInfo.IsStatic ? "Static property" : "Property") +
                            " '" + callInfo.Name + "' does not exist");
                return memberInfo;
            }
            if (callInfo.MemberTypes == MemberTypes.Field)
            {
                memberInfo = callInfo.TargetType.Field(callInfo.Name, Flags.DefaultCriteria | callInfo.ScopeFlag);
                if (memberInfo == null)
                    throw new MissingFieldException((callInfo.IsStatic ? "Static field" : "Field") +
                        " '" + callInfo.Name + "' does not exist");
                return memberInfo;
            }
            throw new ArgumentException(callInfo.MemberTypes + " is not supported");
        }

        public static MethodInfo GetPropertyGetMethod(PropertyInfo propInfo, CallInfo callInfo)
        {
            var methodInfo = propInfo.GetGetMethod();
            return methodInfo ?? GetPropertyMethod("get_", "Getter", callInfo);
        }

        public static MethodInfo GetPropertySetMethod(PropertyInfo propInfo, CallInfo callInfo)
        {
            var methodInfo = propInfo.GetSetMethod();
            return methodInfo ?? GetPropertyMethod("set_", "Setter", callInfo);
        }

        private static MethodInfo GetPropertyMethod(string infoPrefix, string errorPrefix, CallInfo callInfo)
        {
            MethodInfo setMethod = callInfo.TargetType.Method(infoPrefix + callInfo.Name, Flags.DefaultCriteria | callInfo.ScopeFlag);
            if (setMethod == null)
                throw new MissingMemberException(errorPrefix + " method for property " + callInfo.Name + " does not exist");
            return setMethod;
        }
    }
}
