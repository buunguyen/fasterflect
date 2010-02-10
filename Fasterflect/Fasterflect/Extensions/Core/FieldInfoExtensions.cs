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
