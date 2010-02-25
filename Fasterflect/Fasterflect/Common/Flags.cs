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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fasterflect
{
    /// <summary>
    /// This class encapsulates common <see cref="BindingFlags"/> combinations and provides various
    /// additional Fasterflect-specific flags to further tailor the lookup experience.
    /// </summary>
    public struct Flags
    {
        private readonly long flags;
        private static readonly Dictionary<Flags, string> flagNames = new Dictionary<Flags, string>( 64 );

        #region Constructors
        private Flags( long flags )
        {
            this.flags = flags;
        }

        static Flags()
        {
            foreach( BindingFlags flag in Enum.GetValues( typeof(BindingFlags) ) )
            {
                flagNames[ new Flags( (long) flag ) ] = flag.ToString();
            }
            flagNames[ PartialNameMatch ] = "PartialNameMatch"; // new Flags( 1L << 32 );
            flagNames[ TrimExplicitlyImplemented ] = "TrimExplicitlyImplemented"; // new Flags( 1L << 33 );
            flagNames[ ExcludeExplicitlyImplemented ] = "ExcludeExplicitlyImplemented"; // = new Flags( 1L << 34 );
            flagNames[ ExcludeBackingMembers ] = "ExcludeBackingMembers"; // = new Flags( 1L << 35 );

            // not yet supported:
            //flagNames[ VisibilityMatch ] = "VisibilityMatch"; // = new Flags( 1L << 55 );
            //flagNames[ Private ] = "Private"; //   = new Flags( 1L << 56 );
            //flagNames[ Protected ] = "Protected"; // = new Flags( 1L << 57 );
            //flagNames[ Internal ] = "Internal"; //  = new Flags( 1L << 58 );

            //flagNames[ ModifierMatch ] = "ModifierMatch"; // = new Flags( 1L << 59 );
            //flagNames[ Abstract ] = "Abstract"; //  = new Flags( 1L << 60 );
            //flagNames[ Virtual ] = "Virtual"; //   = new Flags( 1L << 61 );
            //flagNames[ Override ] = "Override"; //  = new Flags( 1L << 62 );
            //flagNames[ New ] = "New"; //      = new Flags( 1L << 63 );
        }
        #endregion

        #region Flags Selectors

        #region BindingFlags
        public static readonly Flags None = new Flags( (long) BindingFlags.Default );
        public static readonly Flags IgnoreCase = new Flags( (long) BindingFlags.IgnoreCase );
        public static readonly Flags DeclaredOnly = new Flags( (long) BindingFlags.DeclaredOnly );
        public static readonly Flags ExactBinding = new Flags( (long) BindingFlags.ExactBinding );
        public static readonly Flags Public = new Flags( (long) BindingFlags.Public );
        public static readonly Flags NonPublic = new Flags( (long) BindingFlags.NonPublic );
        public static readonly Flags Instance = new Flags( (long) BindingFlags.Instance );
        public static readonly Flags Static = new Flags( (long) BindingFlags.Static );
        #endregion

        #region FasterflectFlags
        /// <summary>
        /// If this option is specified the search for a named member will perform a partial match instead
        /// of an exact match. If <see href="TrimExplicitlyImplemented"/> is specified the trimmed name is
        /// used instead of the original member name. If <see href="IgnoreCase"/> is specified the 
        /// comparison uses <see href="StringComparison.InvariantCultureIgnoreCase"/> and otherwise
        /// uses <see href="StringComparison.InvariantCulture"/>.
        /// </summary>
        public static readonly Flags PartialNameMatch = new Flags( 1L << 32 );

        /// <summary>
        /// If this option is specified the search for a named member will strip off the namespace and
        /// interface name from explicitly implemented interface members before applying any comparison
        /// operations.
        /// </summary>
        public static readonly Flags TrimExplicitlyImplemented = new Flags( 1L << 33 );

        /// <summary>
        /// If this option is specified the search for members will exclude explicitly implemented
        /// interface members.
        /// </summary>
        public static readonly Flags ExcludeExplicitlyImplemented = new Flags( 1L << 34 );

        /// <summary>
        /// If this option is specified all members that are backers for another member, such as backing
        /// fields for automatic properties or get/set methods for properties, will be excluded from the 
        /// result.
        /// </summary>
        public static readonly Flags ExcludeBackingMembers = new Flags( 1L << 35 );

        #region For The Future
        ///// <summary>
        ///// If this option is specified only members with one (or more) of the specified visibility 
        ///// flags will be included in the result.
        ///// </summary>
        //public static readonly Flags VisibilityMatch = new Flags( 1L << 55 );
        ///// <summary>
        ///// Visibility flags
        ///// </summary>
        //public static readonly Flags Private   = new Flags( 1L << 56 );
        //public static readonly Flags Protected = new Flags( 1L << 57 );
        //public static readonly Flags Internal  = new Flags( 1L << 58 );

        ///// <summary>
        ///// If this option is specified only members with one (or more) of the specified modifier 
        ///// flags will be included in the result.
        ///// </summary>
        //public static readonly Flags ModifierMatch = new Flags( 1L << 59 );
        ///// <summary>
        ///// Modifier flags
        ///// </summary>
        //public static readonly Flags Abstract  = new Flags( 1L << 60 );
        //public static readonly Flags Virtual   = new Flags( 1L << 61 );
        //public static readonly Flags Override  = new Flags( 1L << 62 );
        //public static readonly Flags New       = new Flags( 1L << 63 );
        #endregion

        #endregion

        #region Common Selections
        /// <summary>
        /// Search criteria encompassing all public and non-public members, including base members.
        /// </summary>
        public static readonly Flags AnyVisibility = Public | NonPublic;

        /// <summary>
        /// Search criteria encompassing all public and non-public instance members, including base members.
        /// </summary>
        public static readonly Flags InstanceAnyVisibility = AnyVisibility | Instance;

        /// <summary>
        /// Search criteria encompassing all public and non-public static members, including base members.
        /// </summary>
        public static readonly Flags StaticAnyVisibility = AnyVisibility | Static;

        /// <summary>
        /// Search criteria encompassing all public and non-public instance members, excluding base members.
        /// </summary>
        public static readonly Flags InstanceAnyDeclaredOnly = InstanceAnyVisibility | DeclaredOnly;

        /// <summary>
        /// Search criteria encompassing all public and non-public static members, excluding base members.
        /// </summary>
        public static readonly Flags StaticAnyDeclaredOnly = StaticAnyVisibility | DeclaredOnly;

        /// <summary>
        /// Search criteria encompassing all members, including base members.
        /// </summary>
        public static readonly Flags StaticInstanceAnyVisibility = InstanceAnyVisibility | Static;
        #endregion

        #endregion

        #region Helper Methods
        public bool IsSet( BindingFlags mask )
        {
            return ((BindingFlags) flags & mask) == mask;
        }

        public bool IsSet( Flags mask )
        {
            return (flags & mask) == mask;
        }

        public bool IsAnySet( BindingFlags mask )
        {
            return ((BindingFlags) flags & mask) != 0;
        }

        public bool IsAnySet( Flags mask )
        {
            return (flags & mask) != 0;
        }

        public bool IsNotSet( BindingFlags mask )
        {
            return ((BindingFlags) flags & mask) == 0;
        }

        public bool IsNotSet( Flags mask )
        {
            return (flags & mask) == 0;
        }

        public static Flags SetIf( Flags flags, Flags mask, bool condition )
        {
            return condition ? flags | mask : flags;
        }

        public static Flags SetOnlyIf( Flags flags, Flags mask, bool condition )
        {
            return condition ? flags | mask : (Flags) (flags & ~mask);
        }

        public static Flags ClearIf( Flags flags, Flags mask, bool condition )
        {
            return condition ? (Flags) (flags & ~mask) : flags;
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
            return new Flags( (long) m );
        }

        public static explicit operator Flags( long m )
        {
            return new Flags( m );
        }

        public static implicit operator BindingFlags( Flags m )
        {
            return (BindingFlags) m.flags;
        }

        public static implicit operator long( Flags m )
        {
            return m.flags;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            Flags @this = this;
            var names =
                flagNames.Where( kvp => @this.IsSet( kvp.Key ) ).Select( kvp => kvp.Value ).OrderBy( n => n ).ToList();
            int index = 0;
            var sb = new StringBuilder();
            names.ForEach( n => sb.AppendFormat( "{0}{1}", n, ++index < names.Count ? " | " : "" ) );
            return sb.ToString();
        }
        #endregion
    }
}