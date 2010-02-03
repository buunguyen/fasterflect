#region License

// Copyright 2010 Morten Mertner, Buu Nguyen (http://www.buunguyen.net/blog)
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
		/// Sets <paramref name="parameter"/> to <paramref name="index"/>'th location of the target array.
		/// </summary>
		/// <param name="target">The array whose element is to be set</param>
		/// <param name="index">The index of the element to be set</param>
		/// <param name="parameter">The value to set</param>
		/// <returns>The array itself</returns>
		public static object SetElement(this object target, int index, object parameter)
		{
			DelegateForSetElement(target.GetTypeAdjusted())(target, index, parameter);
			return target;
		}

		/// <summary>
		/// Retrieves the element at <paramref name="index"/>'th location of the target array.
		/// </summary>
		/// <param name="target">The target array whose element is to be retrieved</param>
        /// <param name="index">The index of the element to be retrieved</param>
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual type of the array.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
		/// <returns>The element at <paramref name="index"/>'th location of the target array</returns>
		public static TReturn GetElement<TReturn>(this object target, int index)
		{
			return (TReturn) DelegateForGetElement(target.GetTypeAdjusted())(target, index);
		}

		/// <summary>
		/// Creates a delegate which can set element of the array type <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The array type</param>
		/// <returns>The delegate which can set element of the array type <paramref name="targetType"/></returns>
		public static ArrayElementSetter DelegateForSetElement(this Type targetType)
		{
			return (ArrayElementSetter) new ArraySetEmitter(targetType).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can retrieve element of the array type <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The array type</param>
		/// <returns>The delegate which can retrieve element of the array type <paramref name="targetType"/></returns>
		public static ArrayElementGetter DelegateForGetElement(this Type targetType)
		{
			return (ArrayElementGetter) new ArrayGetEmitter(targetType).GetDelegate();
		}

		#endregion
	}
}