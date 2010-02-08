using System.Reflection;

namespace Fasterflect
{
    public static class ConstructorInfoExtensions
    {
        /// <summary>
        /// Invokes the constructor <paramref name="ctorInfo"/> with no argument.
        /// </summary>
        public static object CreateInstance(this ConstructorInfo ctorInfo)
        {
            return ctorInfo.DelegateForCreateInstance()();
        }

        /// <summary>
        /// Invokes the constructor <paramref name="ctorInfo"/> with <paramref name="parameters"/> as arguments.
        /// </summary>
        public static object CreateInstance(this ConstructorInfo ctorInfo, params object[] parameters)
        {
            return ctorInfo.DelegateForCreateInstance()(parameters);
        }

        /// <summary>
        /// Creates a delegate which can create instance based on the constructor <paramref name="ctorInfo"/>.
        /// </summary>
        public static ConstructorInvoker DelegateForCreateInstance(this ConstructorInfo ctorInfo)
        {
            return ctorInfo.DeclaringType.DelegateForCreateInstance( ctorInfo.GetParameters().GetTypeArray() );
        }
    }
}
