#region License

// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
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
	/// An low-level API for Fasterflect.  The extension method-based
	/// wrapper API, declared in <see cref="ExtensionApi"/>, should be used as the default API.  
	/// 
	/// You should only consider this API only if you need fine-grain control 
	/// of <see cref="Reflector"/> instances and their cache (e.g. use an instance 
	/// for a certain period and then have it garbaged collected to release
	/// memory of the cached methods).
	/// </summary>
	/// <seealso cref="ExtensionApi"/>
	public static class Reflector
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