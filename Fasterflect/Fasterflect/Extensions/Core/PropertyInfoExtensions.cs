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
        public static void Set(this PropertyInfo propInfo, object value)
        {
            propInfo.DelegateForSetStaticPropertyValue()( value );
        }

        /// <summary>
        /// Sets the instance property identified by <paramref name="propInfo"/> of <paramref name="target"/>
        /// with <paramref name="value" />.
        /// </summary>
        public static void Set(this PropertyInfo propInfo, object target, object value)
        {
            propInfo.DelegateForSetPropertyValue()( target, value );
        }

        /// <summary>
        /// Gets the value of the static property identified by <paramref name="propInfo"/>.
        /// </summary>
        public static object Get(this PropertyInfo propInfo)
        {
            return propInfo.DelegateForGetStaticPropertyValue()();
        }

        /// <summary>
        /// Gets the value of the instance property identified by <paramref name="propInfo"/> of <paramref name="target"/>.
        /// </summary>
        public static object Get(this PropertyInfo propInfo, object target)
        {
            return propInfo.DelegateForGetPropertyValue()(target);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static property identified by <paramref name="propInfo"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticPropertyValue(this PropertyInfo propInfo)
        {
            return (StaticMemberSetter) new MemberSetEmitter(propInfo, true).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance property <paramref name="propInfo"/>.
        /// </summary>
        public static MemberSetter DelegateForSetPropertyValue(this PropertyInfo propInfo)
        {
            return (MemberSetter) new MemberSetEmitter(propInfo, false).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static property <paramref name="propInfo"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticPropertyValue(this PropertyInfo propInfo)
        {
            return (StaticMemberGetter) new MemberGetEmitter(propInfo, true).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static property <paramref name="propInfo"/>.
        /// </summary>
        public static MemberGetter DelegateForGetPropertyValue(this PropertyInfo propInfo)
        {
            return (MemberGetter) new MemberGetEmitter(propInfo, false).GetDelegate();
        }
    }
}
