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
using System.Linq;
using System.Collections.Generic;

namespace Fasterflect
{
	/// <summary>
	/// Extension methods for inspecting types.
	/// </summary>
	public static class TypeExtensions
	{
		#region Implements
		/// <summary>
		/// Returns true of the supplied <paramref name="type"/> implements the given <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type (interface) to check for.</typeparam>
		/// <param name="type">The type to check.</param>
		/// <returns>True if the given type implements the interface.</returns>
		public static bool Implements<T>( this Type type )
		{
			return typeof(T).IsAssignableFrom( type ) && typeof(T) != type;
		}
		#endregion

		#region IsFrameworkType
		#region IsFrameworkType Helpers
		private static readonly List<byte[]> tokens = new List<byte[]>
		                                              {
		                                              	new byte[] { 0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89 },
		                                              	new byte[] { 0x31, 0xbf, 0x38, 0x56, 0xad, 0x36, 0x4e, 0x35 },
		                                              	new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }
		                                              };

		internal class ByteArrayEqualityComparer : EqualityComparer<byte[]>
		{
			public override bool Equals( byte[] x, byte[] y )
			{
				return x != null && y != null && x.SequenceEqual( y );
			}

			public override int GetHashCode( byte[] obj )
			{
				return obj.GetHashCode();
			}
		}
		#endregion

		/// <summary>
		/// Returns true if the supplied type is defined in an assembly signed by Microsoft.
		/// </summary>
		public static bool IsFrameworkType( this Type type )
		{
			if( type == null )
			{
				throw new ArgumentNullException( "type" );
			}
			byte[] publicKeyToken = type.Assembly.GetName().GetPublicKeyToken();
			return publicKeyToken != null && tokens.Contains( publicKeyToken, new ByteArrayEqualityComparer() );
		}
		#endregion
	}
}