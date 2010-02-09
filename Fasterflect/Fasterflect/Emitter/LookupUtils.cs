using System;
using System.Reflection;

namespace Fasterflect.Emitter
{
    internal class LookupUtils
    {
        public static ConstructorInfo GetConstructor(CallInfo callInfo)
        {
            ConstructorInfo ctorInfo = callInfo.TargetType.GetConstructor(
                Flags.InstanceCriteria,
                null, CallingConventions.HasThis, callInfo.ParamTypes, null);
            if (ctorInfo == null)
                throw new MissingMemberException("Constructor does not exist");
            return ctorInfo;
        }

        public static MethodInfo GetMethod(CallInfo callInfo)
        {
            MethodInfo methodInfo = callInfo.TargetType.GetMethod(callInfo.Name,
                callInfo.ScopeFlag | Flags.DefaultCriteria,
                null, callInfo.ParamTypes, null);
            if (methodInfo == null)
                throw new MissingMethodException(callInfo.IsStatic
                                                    ? "Static method "
                                                    : "Method " + callInfo.Name + " does not exist");
            return methodInfo;
        }

        public static MemberInfo GetMember(CallInfo callInfo)
        {
            if (callInfo.MemberTypes == MemberTypes.Property)
            {
                PropertyInfo member = callInfo.TargetType.Property(callInfo.Name, Flags.DefaultCriteria | callInfo.ScopeFlag);
                if (member == null)
                        throw new MissingMemberException((callInfo.IsStatic ? "Static property" : "Property") +
                            " '" + callInfo.Name + "' does not exist");
                return member;
            }
            if (callInfo.MemberTypes == MemberTypes.Field)
            {
                FieldInfo field = callInfo.TargetType.Field(callInfo.Name, Flags.DefaultCriteria | callInfo.ScopeFlag);
                if (field == null)
                    throw new MissingFieldException((callInfo.IsStatic ? "Static field" : "Field") +
                        " '" + callInfo.Name + "' does not exist");
                return field;
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
