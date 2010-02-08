using System.Reflection;

namespace Fasterflect
{
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Invokes the static method identified by <paramref name="methodInfo"/> with no argument.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this MethodInfo methodInfo)
        {
            return methodInfo.DelegateForStaticInvoke()();
        }

        /// <summary>
        /// Invokes the static method identified by <paramref name="methodInfo"/> with <paramref name="parameters"/>
        /// as arguments.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this MethodInfo methodInfo, params object[] parameters)
        {
            return methodInfo.DelegateForStaticInvoke()(parameters);
        }

        /// <summary>
        /// Invokes the instance method identified by <paramref name="methodInfo"/> on the object
        /// <paramref name="target"/> with no argument.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this MethodInfo methodInfo, object target)
        {
            return methodInfo.DelegateForInvoke()(target);
        }

        /// <summary>
        /// Invokes the instance method identified by <paramref name="methodInfo"/> on the object
        /// <paramref name="target"/> with <paramref name="parameters"/> as arguments.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this MethodInfo methodInfo, object target, params object[] parameters)
        {
            return methodInfo.DelegateForInvoke()(target, parameters);
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method identified by <paramref name="methodInfo"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke(this MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType.DelegateForStaticInvoke( methodInfo.Name,
                                                                     methodInfo.GetParameters().GetTypeArray() );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method identified by <paramref name="methodInfo"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke(this MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType.DelegateForInvoke( methodInfo.Name,
                                                               methodInfo.GetParameters().GetTypeArray() );
        }
	}
}
