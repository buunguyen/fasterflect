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

namespace Fasterflect.Selectors
{
	public class Flags
	{
		private readonly FasterflectFlags flags;

		#region Constructors
		private Flags( BindingFlags flags )
		{
			this.flags = (FasterflectFlags) flags;
		}
		private Flags( FasterflectFlags flags )
		{
			this.flags = flags;
		}
		#endregion

		#region Flags Selectors
		#region BindingFlags
		public static readonly Flags None = new Flags( BindingFlags.Default );
		public static readonly Flags IgnoreCase = new Flags( BindingFlags.IgnoreCase );
		public static readonly Flags DeclaredOnly = new Flags( BindingFlags.DeclaredOnly );
		public static readonly Flags Public = new Flags( BindingFlags.Public );
		public static readonly Flags NonPublic = new Flags( BindingFlags.NonPublic );
		public static readonly Flags Instance = new Flags( BindingFlags.Instance );
		public static readonly Flags Static = new Flags( BindingFlags.Static );
		#endregion

		#region FasterflectFlags
		public static readonly Flags ExplicitName = new Flags( FasterflectFlags.ExplicitNameMatch );
		public static readonly Flags PartialName = new Flags( FasterflectFlags.PartialNameMatch );
		public static readonly Flags ParameterMatch = new Flags( FasterflectFlags.ParameterMatch );
		public static readonly Flags ExactParameterMatch = new Flags( FasterflectFlags.ExactParameterMatch );
		#endregion

		#region Common Selections
		public static readonly Flags Default = Public | NonPublic | IgnoreCase;
		public static readonly Flags AllInstance = Default | Instance | ExplicitName;
		public static readonly Flags AllStatic = Default | Static;
		public static readonly Flags DeclaredInstance = Instance | DeclaredOnly | ExplicitName;
		public static readonly Flags DeclaredStatic = Static | DeclaredOnly;
		public static readonly Flags All = AllInstance | AllStatic;
		#endregion
		#endregion

		#region Helper Methods
		public bool AreAllSet( BindingFlags mask )
		{
			return ((BindingFlags)flags & mask) == mask;
		}
		public bool AreAllSet( FasterflectFlags mask )
		{
			return (flags & mask) == mask;
		}
		public bool AreAllSet( Flags mask )
		{
			return (flags & mask) == mask;
		}
		public bool IsAnySet( BindingFlags mask )
		{
			return ((BindingFlags)flags & mask) != BindingFlags.Default;
		}
		public bool IsAnySet( FasterflectFlags mask )
		{
			return (flags & mask) != FasterflectFlags.None;
		}
		public bool IsAnySet( Flags mask )
		{
			return (flags & mask) != FasterflectFlags.None;
		}
		public bool AreNoneSet( BindingFlags mask )
		{
			return ((BindingFlags)flags & mask) == BindingFlags.Default;
		}
		public bool AreNoneSet( FasterflectFlags mask )
		{
			return (flags & mask) == FasterflectFlags.None;
		}
		public bool AreNoneSet( Flags mask )
		{
			return (flags & mask) == FasterflectFlags.None;
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
		#endregion

		#region Conversion Operators
		public static implicit operator Flags( BindingFlags m )
		{
			return new Flags( m );
		}
		public static implicit operator Flags( FasterflectFlags m )
		{
			return new Flags( m );
		}
		public static implicit operator BindingFlags( Flags m )
		{
			return (BindingFlags) m.flags;
		}
		public static implicit operator FasterflectFlags( Flags m )
		{
			return m.flags;
		}
		#endregion
	}
}
