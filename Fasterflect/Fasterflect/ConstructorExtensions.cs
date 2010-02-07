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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect.Emitter;
using Fasterflect.ObjectConstruction;

namespace Fasterflect
{
	/// <summary>
	/// Extension methods for locating, inspecting and invoking constructors.
	/// </summary>
	public static class ConstructorExtensions
	{
		#region Constructor Invocation (CreateInstance)
		/// <summary>
		/// Invokes the no-arg constructor on type <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The type whose constructor is to be invoked.</param>
		/// <returns>An instance of type <paramref name="targetType"/>.</returns>
		public static object CreateInstance(this Type targetType)
		{
			return CreateInstance(targetType, Type.EmptyTypes, Constants.EmptyObjectArray);
		}

		/// <summary>
		/// Invokes a constructor specified by <paramref name="parameters" /> on the type <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The type whose constructor is to be invoked.</param>
		/// <param name="parameters">The parameters of the constructor (must be in the right order).
		/// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
		/// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
		/// use a different overload of this method.</param>
		/// <returns>An instance of type <paramref name="targetType"/>.</returns>
		public static object CreateInstance(this Type targetType, params object[] parameters)
		{
			return DelegateForCreateInstance(targetType, parameters.GetTypeArray())(parameters);
		}

		/// <summary>
		/// Invokes a constructor specified by <paramref name="parameters" /> on the type <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The type whose constructor is to be invoked.</param>
		/// <param name="paramTypes">The types of the constructor parameters (must be in the right order).</param>
		/// <param name="parameters">The parameters of the constructor (must be in the right order).</param>
		/// <returns>An instance of type <paramref name="targetType"/>.</returns>
		public static object CreateInstance(this Type targetType, Type[] paramTypes, params object[] parameters)
		{
			return DelegateForCreateInstance(targetType, paramTypes)(parameters);
		}

		/// <summary>
		/// Creates a delegate which can invoke the constructor of type <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The type which has the constructor to be invoked.</param>
		/// <returns>The delegate which can invoke the constructor of type <paramref name="targetType"/>.</returns>
		public static ConstructorInvoker DelegateForCreateInstance(this Type targetType)
		{
			return DelegateForCreateInstance(targetType, Type.EmptyTypes);
		}

		/// <summary>
		/// Creates a delegate which can invoke the constructor of type <paramref name="targetType"/>.
		/// </summary>
		/// <param name="targetType">The type which has the constructor to be invoked.</param>
		/// <param name="paramTypes">The types of the constructor parameters (must be in the right order).</param>
		/// <returns>The delegate which can invoke the constructor of type <paramref name="targetType"/>.</returns>
		public static ConstructorInvoker DelegateForCreateInstance(this Type targetType, params Type[] paramTypes)
		{
			return (ConstructorInvoker) new CtorInvocationEmitter(targetType, paramTypes).GetDelegate();
		}
		#endregion

		#region Constructor Invocation (TryCreateInstance)
		/// <summary>
		/// Creates an instance of the given <paramref name="type"/> using the public properties of the 
		/// supplied <paramref name="source"/> object as input.
		/// This method will try to determine the least-cost route to constructing the instance, which
		/// implies mapping as many properties as possible to constructor parameters. Remaining properties
		/// on the source are mapped to properties on the created instance or ignored if none matches.
		/// </summary>
		/// <returns>An instance of type <paramref name="type"/>.</returns>
		public static object TryCreateInstance(this Type type, object source)
		{
			Type sourceType = source.GetType();
			SourceInfo sourceInfo = MapFactory.GetSourceInfo(sourceType);
			if (sourceInfo == null)
			{
				sourceInfo = new SourceInfo(sourceType);
				MapFactory.AddSourceInfo(sourceType, sourceInfo);
			}
			object[] paramValues = sourceInfo.GetParameterValues( source );
			MethodMap map = type.PrepareInvoke( sourceInfo.ParamNames, sourceInfo.ParamTypes, paramValues );
			return map.Invoke( paramValues );
		}

		/// <summary>
		/// Creates an instance of the given <paramref name="type"/> using the values in the supplied
		/// <paramref name="parameters"/> dictionary as input.
		/// This method will try to determine the least-cost route to constructing the instance, which
		/// implies mapping as many values as possible to constructor parameters. Remaining values
		/// are mapped to properties on the created instance or ignored if none matches.
		/// </summary>
		/// <returns>An instance of type <paramref name="type"/>.</returns>
		public static object TryCreateInstance( this Type type, IDictionary<string, object> parameters )
		{
			string[] names = parameters.Keys.ToArray();
			object[] values = parameters.Values.ToArray();
			return type.TryCreateInstance(names, values);
		}

		/// <summary>
		/// Creates an instance of the given <paramref name="type"/> using the supplied parameter information as input.
		/// Parameter types are inferred from the supplied <paramref name="parameterValues"/> and as such these
		/// should not be null.
		/// This method will try to determine the least-cost route to constructing the instance, which
		/// implies mapping as many properties as possible to constructor parameters. Remaining properties
		/// on the source are mapped to properties on the created instance or ignored if none matches.
		/// </summary>
		/// <param name="type">The type of which an instance should be created.</param>
		/// <param name="parameterNames">The names of the supplied parameters.</param>
		/// <param name="parameterValues">The values of the supplied parameters.</param>
		/// <returns>An instance of type <paramref name="type"/>.</returns>
		public static object TryCreateInstance( this Type type, string[] parameterNames, object[] parameterValues )
		{
			var parameterTypes = new Type[parameterValues.Length];
			for (int i = 0; i < parameterNames.Length; i++)
			{
				object value = parameterValues[i];
				parameterTypes[i] = value != null ? value.GetType() : null;
			}
			return type.TryCreateInstance(parameterNames, parameterTypes, parameterValues);
		}

		/// <summary>
		/// Creates an instance of the given <paramref name="type"/> using the supplied parameter information as input.
		/// This method will try to determine the least-cost route to constructing the instance, which
		/// implies mapping as many properties as possible to constructor parameters. Remaining properties
		/// on the source are mapped to properties on the created instance or ignored if none matches.
		/// </summary>
		/// <param name="type">The type of which an instance should be created.</param>
		/// <param name="parameterNames">The names of the supplied parameters.</param>
		/// <param name="parameterTypes">The types of the supplied parameters.</param>
		/// <param name="parameterValues">The values of the supplied parameters.</param>
		/// <returns>An instance of type <paramref name="type"/>.</returns>
		public static object TryCreateInstance( this Type type, string[] parameterNames, Type[] parameterTypes,
		                                       object[] parameterValues)
		{
			MethodMap map = type.PrepareInvoke(parameterNames, parameterTypes, parameterValues);
			return map.Invoke(parameterValues);
		}
		#endregion

		#region Constructor Lookup
		/// <summary>
		/// Find all available (non-abstract) constructors for the specified type.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <returns>An enumeration of available constructors</returns>
		public static IList<ConstructorInfo> Constructors(this Type type)
		{
            return type.GetConstructors(Flags.InstanceCriteria).Where(ci => !ci.IsAbstract).ToList();
		}

		/// <summary>
		/// Find the constructor with the specified parameter list.
		/// </summary>
		/// <param name="type">The type to reflect on</param>
		/// <param name="parameterTypes">The types of the constructor parameters in order</param>
		/// <returns>The matching constructor or null if no match was found</returns>
		public static ConstructorInfo Constructor(this Type type, params Type[] parameterTypes)
		{
            return type.GetConstructor(Flags.InstanceCriteria, null, parameterTypes, null);
		}
		#endregion
	}
}