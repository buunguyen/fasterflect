using System.Reflection;

namespace Fasterflect
{
	/// <summary>
	/// This class provides access to common <see cref="BindingFlags"/> combinations.
	/// </summary>
    public static class Flags
    {
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
    }
}
