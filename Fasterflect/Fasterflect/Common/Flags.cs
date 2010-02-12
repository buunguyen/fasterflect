#region License

// Copyright 2010 Buu Nguyen, Morten Mertner
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/

#endregion

using System.Reflection;

namespace Fasterflect
{
	/// <summary>
	/// This class encapsulates common <see cref="BindingFlags"/> combinations and provides various
	/// additional Fasterflect-specific flags to further tailor the lookup experience.
	/// </summary>
	public sealed class Flags
	{
		private readonly long flags;

		#region Constructors
		private Flags( long flags )
		{
			this.flags = flags;
		}
		#endregion

		#region Flags Selectors
		#region BindingFlags
		public static readonly Flags None = new Flags( (long)BindingFlags.Default );
        public static readonly Flags IgnoreCase = new Flags((long)BindingFlags.IgnoreCase);
        public static readonly Flags DeclaredOnly = new Flags((long)BindingFlags.DeclaredOnly);
        public static readonly Flags Public = new Flags((long)BindingFlags.Public);
        public static readonly Flags NonPublic = new Flags((long)BindingFlags.NonPublic);
        public static readonly Flags Instance = new Flags((long)BindingFlags.Instance);
        public static readonly Flags Static = new Flags((long)BindingFlags.Static);
		#endregion

        #region FasterflectFlags
        /// <summary>
        /// If this option is specified the search for a named member will perform a partial match instead
        /// of an exact match.   
        /// </summary>
        public static readonly Flags PartialNameMatch = new Flags(0x100000000);
        /// <summary>
        /// If this option is specified the search for a named member will include explicitly implemented
        /// interface members.
        /// </summary>
        public static readonly Flags ExplicitNameMatch = new Flags(0x200000000);
        /// <summary>
        /// If this option is specified the parameter types are verified for assignment compatibility
        /// with the supplied parameter types.   
        /// </summary>
        public static readonly Flags ParameterMatch = new Flags(0x400000000);
        /// <summary>
        /// If this option is specified the parameter types must match exactly rather than by
        /// assignment compatibility. 
        /// </summary>
        public static readonly Flags ExactParameterMatch = new Flags(0x800000000);
        /// <summary>
        /// If this option is specified all members that are backers for another member, such as backing
        /// fields for automatic properties or get/set methods for properties, will be excluded from the 
        /// result.
        /// </summary>
        public static readonly Flags ExcludeBackingMembers = new Flags(0x1000000000);
 		#endregion

		#region Common Selections
		public static readonly Flags Default = Public | NonPublic;
		public static readonly Flags AllInstance = Default | Instance | ExplicitNameMatch;
		public static readonly Flags AllStatic = Default | Static;
		public static readonly Flags DeclaredInstance = Instance | DeclaredOnly | ExplicitNameMatch;
		public static readonly Flags DeclaredStatic = Static | DeclaredOnly;
		public static readonly Flags All = AllInstance | AllStatic;
		#endregion

		#region Temporary selections for compatibility with old Flags
		/// <summary>
        /// Search criteria encompassing all public and non-public members.
        /// </summary>
        public static readonly Flags DefaultCriteria = Public | NonPublic;

        /// <summary>
        /// Search criteria encompassing all public and non-public instance members.
        /// </summary>
        public static readonly Flags InstanceCriteria = DefaultCriteria | Instance;

        /// <summary>
        /// Search criteria encompassing all public and non-public static members, including those of parent classes.
        /// </summary>
        public static readonly Flags StaticCriteria = DefaultCriteria | Static;

        /// <summary>
        /// Search criteria encompassing all members, including those of parent classes.
        /// </summary>
        public static readonly Flags AllCriteria = InstanceCriteria | StaticCriteria;
		#endregion
		#endregion

		#region Helper Methods
		public bool IsSet( BindingFlags mask )
		{
			return ((BindingFlags)flags & mask) == mask;
		}
		public bool IsSet( Flags mask )
		{
			return (flags & mask) == mask;
		}
		public bool IsAnySet( BindingFlags mask )
		{
			return ((BindingFlags)flags & mask) != 0;
		}
		public bool IsAnySet( Flags mask )
		{
			return (flags & mask) != 0;
		}
		public bool IsNotSet( BindingFlags mask )
		{
			return ((BindingFlags)flags & mask) == 0;
		}
		public bool IsNotSet( Flags mask )
		{
			return (flags & mask) == 0;
		}

		public static Flags SetIf( Flags flags, Flags mask, bool condition )
		{
			return condition ? flags | mask : flags;
		}
		public static Flags ClearIf( Flags flags, Flags mask, bool condition )
		{
			return condition ? flags & ~mask : flags;
		}
		#endregion

		#region Equals
		public override bool Equals( object obj )
		{
			return obj != null && obj.GetType() == typeof(Flags) && flags == ((Flags) obj).flags;
		}

		public override int GetHashCode()
		{
			return flags.GetHashCode();
		}
		#endregion

		#region Operators
		public static Flags operator -( Flags f1, Flags f2 )
		{
			return new Flags( f1.flags & ~f2.flags );
		}

		public static Flags operator |( Flags f1, Flags f2 )
		{
			return new Flags( f1.flags | f2.flags );
		}

		public static Flags operator &( Flags f1, Flags f2 )
		{
			return new Flags( f1.flags & f2.flags );
		}

		public static bool operator ==( Flags f1, Flags f2 )
		{
			return f1.flags == f2.flags;
		}

		public static bool operator !=( Flags f1, Flags f2 )
		{
			return f1.flags != f2.flags;
		}
		#endregion

		#region Conversion Operators
		public static implicit operator Flags( BindingFlags m )
		{
			return new Flags( (long)m );
		}
		public static implicit operator Flags( long m )
		{
			return new Flags( m );
		}
		public static implicit operator BindingFlags( Flags m )
		{
			return (BindingFlags) m.flags;
		}
        public static implicit operator long(Flags m)
		{
			return m.flags;
		}
		#endregion
	}
}
