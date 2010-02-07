using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Fasterflect
{
    public static class FieldInfoExtensions
    {
        #region Setters
        /// <summary>
        /// Sets the static field <paramref name="fieldInfo"/> with <paramref name="value" />.
        /// </summary>
        public static void Set(this FieldInfo fieldInfo, object value)
        {
            fieldInfo.DeclaringType.SetField(fieldInfo.Name, value);
        }

        /// <summary>
        /// Sets the instance field <paramref name="fieldInfo"/> of <paramref name="target"/>
        /// with <paramref name="value" />.
        /// </summary>
        public static void Set(this FieldInfo fieldInfo, object target, object value)
        {
            target.SetField(fieldInfo.Name, value);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static field <paramref name="fieldInfo"/>.
        /// </summary>
        public static StaticMemberSetter DelegateForSetStaticField(this FieldInfo fieldInfo)
        {
            return fieldInfo.DeclaringType.DelegateForSetStaticField(fieldInfo.Name);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the instance field <paramref name="fieldInfo"/>.
        /// </summary>
        public static MemberSetter DelegateForSetField(this FieldInfo fieldInfo)
        {
            return fieldInfo.DeclaringType.DelegateForSetField(fieldInfo.Name);
        }
        #endregion

        #region Getters
        /// <summary>
        /// Gets the value of the static field <paramref name="fieldInfo"/>.
        /// </summary>
        public static TReturn Get<TReturn>(this FieldInfo fieldInfo)
        {
            return fieldInfo.DeclaringType.GetField<TReturn>(fieldInfo.Name);
        }

        /// <summary>
        /// Gets the value of the instance field <paramref name="fieldInfo"/> of <paramref name="target"/>.
        /// </summary>
        public static TReturn Get<TReturn>(this FieldInfo fieldInfo, object target)
        {
            return target.GetField<TReturn>(fieldInfo.Name);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the static field <paramref name="fieldInfo"/>.
        /// </summary>
        public static StaticMemberGetter DelegateForGetStaticField(this FieldInfo fieldInfo)
        {
            return fieldInfo.DeclaringType.DelegateForGetStaticField(fieldInfo.Name);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the static field <paramref name="fieldInfo"/>.
        /// </summary>
        public static MemberGetter DelegateForGetField(this FieldInfo fieldInfo)
        {
            return fieldInfo.DeclaringType.DelegateForGetField(fieldInfo.Name);
        }
        #endregion
    }
}
