using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the static field or property identified by <paramref name="memberInfo"/>.
        /// </summary>
        public static object GetValue(this MemberInfo memberInfo)
        {
            var @delegate = (StaticMemberGetter)
                new MemberGetEmitter(
                    memberInfo.DeclaringType,
                    memberInfo.MemberType,
                    memberInfo.Name, 
                    true).GetDelegate();
            return @delegate();
        }

        /// <summary>
        /// Sets the static field or property identified by <paramref name="memberInfo"/> with <paramref name="value"/>.
        /// </summary>
        public static void SetValue(this MemberInfo memberInfo, object value)
        {
            var @delegate = (StaticMemberSetter)
                new MemberSetEmitter(
                    memberInfo.DeclaringType, 
                    memberInfo.MemberType, 
                    memberInfo.Name, 
                    true).GetDelegate();
            @delegate(value);
        }

        /// <summary>
        /// Gets the instance field or property identified by <paramref name="memberInfo"/> on
        /// the <paramref name="target"/>.
        /// </summary>
        public static object GetValue(this MemberInfo memberInfo, object target)
        {
            var @delegate = (MemberGetter)
                new MemberGetEmitter(
                    memberInfo.DeclaringType,
                    memberInfo.MemberType,
                    memberInfo.Name, 
                    false).GetDelegate();
            return @delegate(target);
        }

        /// <summary>
        /// Sets the instance field or property identified by <paramref name="memberInfo"/> on
        /// the <paramref name="target"/> object with <paramref name="value"/>.
        /// </summary>
        public static void SetValue(this MemberInfo memberInfo, object target, object value)
        {
            var @delegate = (MemberSetter)
                new MemberSetEmitter(
                    memberInfo.DeclaringType, 
                    memberInfo.MemberType, 
                    memberInfo.Name, 
                    false).GetDelegate();
            @delegate(target, value);
        }
    }
}
