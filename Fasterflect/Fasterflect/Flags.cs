using System.Reflection;

namespace Fasterflect
{
    internal static class Flags
    {
        #region BindingFlags
        /// <summary>
        /// Search criteria encompassing all public and non-public members.
        /// </summary>
        public static readonly BindingFlags DefaultCriteria = BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Search criteria encompassing all public and non-public instance members.
        /// </summary>
        public static readonly BindingFlags InstanceCriteria = DefaultCriteria | BindingFlags.Instance;

        /// <summary>
        /// Search criteria encompassing all public and non-public static members, including those of parent classes.
        /// </summary>
        public static readonly BindingFlags StaticCriteria = DefaultCriteria | BindingFlags.Static |
                                                             BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Search criteria encompassing all members, including those of parent classes.
        /// </summary>
        public static readonly BindingFlags AllCriteria = InstanceCriteria | StaticCriteria;
        #endregion
    }
}
