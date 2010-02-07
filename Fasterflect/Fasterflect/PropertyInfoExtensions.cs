using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Fasterflect
{
    public static class PropertyInfoExtensions
    {
        #region Setters
        /// <summary>
        /// Sets the static property <paramref name="propInfo"/> with <paramref name="value" />.
        /// </summary>
        public static void Set(this PropertyInfo propInfo, object value)
        {
            propInfo.DeclaringType.SetProperty(propInfo.Name, value);
        }

        /// <summary>
        /// Sets the instance property <paramref name="propInfo"/> of <paramref name="target"/>
        /// with <paramref name="value" />.
        /// </summary>
        public static void Set(this PropertyInfo propInfo, object target, object value)
        {
            target.SetProperty(propInfo.Name, value);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static property <paramref name="propInfo"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticProperty(this PropertyInfo propInfo)
        {
            return propInfo.DeclaringType.DelegateForSetStaticProperty(propInfo.Name);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance property <paramref name="propInfo"/>.
        /// </summary>
        public static MemberSetter DelegateForSetProperty(this PropertyInfo propInfo)
        {
            return propInfo.DeclaringType.DelegateForSetProperty(propInfo.Name);
        }
        #endregion

        #region Getters
        /// <summary>
        /// Gets the value of the static property <paramref name="propInfo"/>.
        /// </summary>
        public static TReturn Get<TReturn>(this PropertyInfo propInfo)
        {
            return propInfo.DeclaringType.GetProperty<TReturn>(propInfo.Name);
        }

        /// <summary>
        /// Gets the value of the instance property <paramref name="propInfo"/> of <paramref name="target"/>.
        /// </summary>
        public static TReturn Get<TReturn>(this PropertyInfo propInfo, object target)
        {
            return target.GetProperty<TReturn>(propInfo.Name);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static property <paramref name="propInfo"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticProperty(this PropertyInfo propInfo)
        {
            return propInfo.DeclaringType.DelegateForGetStaticProperty(propInfo.Name);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static property <paramref name="propInfo"/>.
        /// </summary>
        public static MemberGetter DelegateForGetProperty(this PropertyInfo propInfo)
        {
            return propInfo.DeclaringType.DelegateForGetProperty(propInfo.Name);
        }
        #endregion
    }
}
