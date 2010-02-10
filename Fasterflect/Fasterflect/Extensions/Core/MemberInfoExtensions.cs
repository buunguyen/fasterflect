using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the static field or property identified by <paramref name="memberInfo"/>.
        /// </summary>
        public static object Get( this MemberInfo memberInfo )
        {
            var @delegate = (StaticMemberGetter) new MemberGetEmitter( memberInfo, true ).GetDelegate();
            return @delegate();
        }

        /// <summary>
        /// Sets the static field or property identified by <paramref name="memberInfo"/> with <paramref name="value"/>.
        /// </summary>
        public static void Set( this MemberInfo memberInfo, object value )
        {
            var @delegate = (StaticMemberSetter) new MemberSetEmitter( memberInfo, true ).GetDelegate();
            @delegate( value );
        }

        /// <summary>
        /// Gets the instance field or property identified by <paramref name="memberInfo"/> on
        /// the <paramref name="target"/>.
        /// </summary>
        public static object Get( this MemberInfo memberInfo, object target )
        {
            var @delegate = (MemberGetter) new MemberGetEmitter( memberInfo, false ).GetDelegate();
            return @delegate( target );
        }

        /// <summary>
        /// Sets the instance field or property identified by <paramref name="memberInfo"/> on
        /// the <paramref name="target"/> object with <paramref name="value"/>.
        /// </summary>
        public static void Set( this MemberInfo memberInfo, object target, object value )
        {
            var @delegate = (MemberSetter) new MemberSetEmitter( memberInfo, false ).GetDelegate();
            @delegate( target, value );
        }
    }
}