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
using Fasterflect.Emitter;

namespace Fasterflect
{
    /// <summary>
    /// Extension methods for working with arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        #region Array Access
        /// <summary>
        /// Sets <paramref name="value"/> to the element at position <paramref name="index"/> of <paramref name="array"/>.
        /// </summary>
        /// <returns><paramref name="array"/>.</returns>
        public static object SetElement( this object array, long index, object value )
        {
            ((Array) array).SetValue( value, index );
            return array;
        }

        /// <summary>
        /// Gets the element at position <paramref name="index"/> of <paramref name="array"/>.
        /// </summary>
        public static object GetElement( this object array, long index )
        {
            return ((Array) array).GetValue( index );
        }

        /// <summary>
        /// Creates a delegate which can set element of the array type <paramref name="targetArrayType"/>.
        /// </summary>
        public static ArrayElementSetter DelegateForSetElement( this Type targetArrayType )
        {
            return (ArrayElementSetter) new ArraySetEmitter( targetArrayType ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can retrieve element of the array type <paramref name="targetArrayType"/>.
        /// </summary>
        public static ArrayElementGetter DelegateForGetElement( this Type targetArrayType )
        {
            return (ArrayElementGetter) new ArrayGetEmitter( targetArrayType ).GetDelegate();
        }
        #endregion
    }
}