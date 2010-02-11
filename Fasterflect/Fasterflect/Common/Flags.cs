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
        public static readonly BindingFlags StaticCriteria = DefaultCriteria | BindingFlags.Static;

        /// <summary>
        /// Search criteria encompassing all members, including those of parent classes.
        /// </summary>
        public static readonly BindingFlags AllCriteria = InstanceCriteria | StaticCriteria;
    }
}
