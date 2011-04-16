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

using System;

namespace Fasterflect
{
    /// <summary>
    /// A wrapper for struct type.  Must be used in order for Fasterflect to 
    /// work with struct.
    /// </summary>
    public class Struct
    {
        /// <summary>
        /// The actual struct wrapped by this instance.
        /// </summary>
        public ValueType Value { get; set; }

        /// <summary>
        /// Creates a wrapper for <paramref name="value"/> struct.  The wrapper
        /// can then be used with Fasterflect.
        /// </summary>
        /// <param name="value">The struct to be wrapped.  
        /// Must be a derivative of <code>ValueType</code>.</param>
        public Struct(object value)
        {
            Value = (ValueType)value;
        }
    }
}
