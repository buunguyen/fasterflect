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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
	/// <summary>
	/// Extension methods for locating, inspecting and invoking methods.
	/// </summary>
	public static class MethodExtensions
	{
		#region Method Invocation
		#region Static Method Invocation
		/// <summary>
		/// Invokes the static method specified by <paramref name="methodName"/> of type
		/// <paramref name="targetType"/>.  Use this overload when the static method has no return type or 
		/// developers are not interested in the return value.
		/// </summary>
		/// <param name="targetType">The type whose static method is to be invoked.</param>
		/// <param name="methodName">The name of the static method to be invoked.</param>
		/// <returns>The type whose static method is to be invoked.</returns>
		public static Type Invoke(this Type targetType, string methodName)
		{
			return targetType.Invoke(methodName, Type.EmptyTypes, Constants.EmptyObjectArray);
		}

		/// <summary>
		/// Invokes the static method specified by <paramref name="methodName"/> of type
		/// <paramref name="targetType"/>.  Use this overload when the static method has no return type or 
		/// developers are not interested in the return value.
		/// </summary>
		/// <param name="targetType">The type whose static method is to be invoked.</param>
		/// <param name="methodName">The name of the static method to be invoked.</param>
		/// <param name="parameters">The parameters of the static method (must be in the right order).  
		/// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
		/// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
		/// use a different overload of this method.</param>
		/// <returns>The type whose static method is to be invoked.</returns>
		public static Type Invoke(this Type targetType, string methodName, params object[] parameters)
		{
			return targetType.Invoke(methodName, parameters.GetTypeArray(), parameters);
		}

		/// <summary>
		/// Invokes the static method specified by <paramref name="methodName"/> of type
		/// <paramref name="targetType"/>.  Use this overload when the static method has no return type or 
		/// developers are not interested in the return value.
		/// </summary>
		/// <param name="targetType">The type whose static method is to be invoked.</param>
		/// <param name="methodName">The name of the static method to be invoked.</param>
		/// <param name="paramTypes">The types of the static method's parameters (must be in the right order).</param>
		/// <param name="parameters">The parameters of the static method (must be in the right order).</param>
		/// <returns>The type whose static method is to be invoked.</returns>
		public static Type Invoke(this Type targetType, string methodName, Type[] paramTypes, params object[] parameters)
		{
			DelegateForStaticInvoke(targetType, methodName, paramTypes)(parameters);
			return targetType;
		}

		/// <summary>
		/// Invokes the static method specified by <paramref name="methodName"/> of type <paramref name="targetType"/>
		/// and get back the return value, casted to <typeparamref name="TReturn"/>.
        /// </summary>
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual return type.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="targetType">The type whose static method is to be invoked.</param>
		/// <param name="methodName">The name of the static method to be invoked.</param>
		/// <returns>The return value of the static method.</returns>
		public static TReturn Invoke<TReturn>(this Type targetType, string methodName)
		{
			return targetType.Invoke<TReturn>(methodName, Type.EmptyTypes, Constants.EmptyObjectArray);
		}

		/// <summary>
		/// Invokes the static method specified by <paramref name="methodName"/> of type <paramref name="targetType"/>
		/// and get back the return value, casted to <typeparamref name="TReturn"/>
        /// </summary>
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual return type.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="targetType">The type whose static method is to be invoked.</param>
		/// <param name="methodName">The name of the static method to be invoked.</param>
		/// <param name="parameters">The parameters of the static method (must be in the right order).
		/// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
		/// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
		/// use a different overload of this method.</param>
		/// <returns>The return value of the static method.</returns>
		public static TReturn Invoke<TReturn>(this Type targetType, string methodName, params object[] parameters)
		{
			return targetType.Invoke<TReturn>(methodName, parameters.GetTypeArray(), parameters);
		}

		/// <summary>
		/// Invokes the static method specified by <paramref name="methodName"/> of type <paramref name="targetType"/>
		/// and get back the return value, casted to <typeparamref name="TReturn"/>
        /// </summary>
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual return type.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="targetType">The type whose static method is to be invoked.</param>
		/// <param name="methodName">The name of the static method to be invoked.</param>
		/// <param name="paramTypes">The types of the static method's parameters (must be in the right order).</param>
		/// <param name="parameters">The parameters of the static method (must be in the right order).</param>
		/// <returns>The return value of the static method.</returns>
		public static TReturn Invoke<TReturn>(this Type targetType, string methodName, Type[] paramTypes,
		                                      params object[] parameters)
		{
			return (TReturn) DelegateForStaticInvoke(targetType, methodName, paramTypes)(parameters);
		}

		/// <summary>
		/// Creates a delegate which can invoke the specified static method.
		/// </summary>
		/// <param name="targetType">The type which the static method belongs to.</param>
		/// <param name="methodName">The name of the static method to be invoked.</param>
		/// <returns>A delegate which can invoke the specified static method.</returns>
		public static StaticMethodInvoker DelegateForStaticInvoke(this Type targetType, string methodName)
		{
			return (StaticMethodInvoker) new MethodInvocationEmitter(methodName, targetType, Type.EmptyTypes, true).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can invoke the specified static method.
		/// </summary>
		/// <param name="targetType">The type which the static method belongs to.</param>
		/// <param name="methodName">The name of the static method to be invoked.</param>
		/// <param name="paramTypes">The types of the static method's parameters (must be in the right order).</param>
		/// <returns>A delegate which can invoke the specified static method.</returns>
		public static StaticMethodInvoker DelegateForStaticInvoke(this Type targetType, string methodName,
		                                                          params Type[] paramTypes)
		{
			return (StaticMethodInvoker) new MethodInvocationEmitter(methodName, targetType, paramTypes, true).GetDelegate();
		}
		#endregion

		#region Instance Method Invocation
		/// <summary>
		/// Invokes the method specified by <paramref name="methodName"/> of object
		/// <paramref name="target"/>.  Use this overload when the method has no return type or 
		/// developers are not interested in the return value.
		/// </summary>
		/// <param name="target">The object whose method is to be invoked.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <returns>The object whose method is to be invoked.</returns>
		public static object Invoke(this object target, string methodName)
		{
			return Invoke(target, methodName, Type.EmptyTypes, Constants.EmptyObjectArray);
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="methodName"/> of object
		/// <paramref name="target"/>.  Use this overload when the method has no return type or 
		/// developers are not interested in the return value.
		/// </summary>
		/// <param name="target">The object whose method is to be invoked.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <param name="parameters">The parameters of the method (must be in the right order).
		/// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
		/// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
		/// use a different overload of this method.</param>
		/// <returns>The object whose method is to be invoked.</returns>
		public static object Invoke(this object target, string methodName, params object[] parameters)
		{
			DelegateForInvoke(target.GetTypeAdjusted(), methodName, parameters.GetTypeArray())(target, parameters);
			return target;
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="methodName"/> of object
		/// <paramref name="target"/>.  Use this overload when the method has no return type or 
		/// developers are not interested in the return value.
		/// </summary>
		/// <param name="target">The object whose method is to be invoked.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <param name="paramTypes">The types of the method parameters (must be in the right order).</param>
		/// <param name="parameters">The parameters of the method (must be in the right order).</param>
		/// <returns>The object whose method is to be invoked.</returns>
		public static object Invoke(this object target, string methodName, Type[] paramTypes, params object[] parameters)
		{
			DelegateForInvoke(target.GetTypeAdjusted(), methodName, paramTypes)(target, parameters);
			return target;
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="methodName"/> of object <paramref name="target"/>
		/// and get back the return value, casted to <typeparamref name="TReturn"/>.
        /// </summary>
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual return type.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="target">The object whose method is to be invoked.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <returns>The return value of the method.</returns>
		public static TReturn Invoke<TReturn>(this object target, string methodName)
		{
			return Invoke<TReturn>(target, methodName, Type.EmptyTypes, Constants.EmptyObjectArray);
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="methodName"/> of object <paramref name="target"/>
		/// and get back the return value, casted to <typeparamref name="TReturn"/>
        /// </summary>
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual return type.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="target">The object whose method is to be invoked.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <param name="parameters">The parameters of the method (must be in the right order).
		/// The parameter types are determined from these parameters, therefore no parameter can be <code>null</code>.
		/// If any parameter is <code>null</code> (or you can't be sure of that, i.e. receive from a variable), 
		/// use a different overload of this method.</param>
		/// <returns>The return value of the method.</returns>
		public static TReturn Invoke<TReturn>(this object target, string methodName, params object[] parameters)
		{
			return
				(TReturn) DelegateForInvoke(target.GetTypeAdjusted(), methodName, parameters.GetTypeArray())(target, parameters);
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="methodName"/> of object <paramref name="target"/>
		/// and get back the return value, casted to <typeparamref name="TReturn"/>
        /// </summary>
        /// <typeparam name="TReturn">The return type of this method.  There must be an implicit or explicit conversion 
        /// between this type and the actual return type.  Fasterflect simply inserts an explicit cast 
        /// behind the scene, no magic in the generated CIL.</typeparam>
		/// <param name="target">The object whose method is to be invoked.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <param name="paramTypes">The types of the method parameters (must be in the right order).</param>
		/// <param name="parameters">The parameters of the method (must be in the right order).</param>
		/// <returns>The return value of the method.</returns>
		public static TReturn Invoke<TReturn>(this object target, string methodName, Type[] paramTypes,
		                                      params object[] parameters)
		{
			return (TReturn) DelegateForInvoke(target.GetTypeAdjusted(), methodName, paramTypes)(target, parameters);
		}

		/// <summary>
		/// Creates a delegate which can invoke the specified method.
		/// </summary>
		/// <param name="targetType">The type which the method belongs to.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <returns>A delegate which can invoke the specified method.</returns>
		public static MethodInvoker DelegateForInvoke(this Type targetType, string methodName)
		{
			return DelegateForInvoke(targetType, methodName, Type.EmptyTypes);
		}

		/// <summary>
		/// Creates a delegate which can invoke the specified method.
		/// </summary>
		/// <param name="targetType">The type which the method belongs to.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <param name="paramTypes">The types of the method parameters (must be in the right order).</param>
		/// <returns>A delegate which can invoke the specified method.</returns>
		public static MethodInvoker DelegateForInvoke(this Type targetType, string methodName, params Type[] paramTypes)
		{
			return (MethodInvoker) new MethodInvocationEmitter(methodName, targetType, paramTypes, false).GetDelegate();
		}
		#endregion
		#endregion

		#region Method Lookup
		/// <summary>
		/// Gets the public or non-public, instance or static method with the given <paramref name="name"/> on the
		///  given <paramref name="type"/>.
		/// </summary>
		/// <returns>The specified method or null if no method was found.</returns>
		public static MethodInfo Method( this Type type, string name )
		{
			return type.GetMethod( name, Flags.AllCriteria );
		}

		/// <summary>
		/// Gets all public and non-public, instance and static methods on the given <paramref name="type"/>.
		/// </summary>
		/// <returns>A list of all methods.</returns>
		public static IList<MethodInfo> Methods( this Type type )
		{
			return type.Methods( Flags.AllCriteria, null );
		}

		/// <summary>
		/// Gets all methods on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
		/// </summary>
		/// <returns>A list of all matching methods.</returns>
		public static IList<MethodInfo> Methods( this Type type, BindingFlags bindingFlags )
		{
			return type.Methods(bindingFlags, null);
		}

		/// <summary>
		/// Gets all methods on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>
		/// and with the given <paramref name="methodName"/>.
		/// </summary>
		/// <returns>A list of all matching methods.</returns>
		public static IList<MethodInfo> Methods( this Type type, BindingFlags bindingFlags, string methodName )
		{
			return (from memberInfo in type.FindMembers(MemberTypes.Method, bindingFlags, null, null)
			        where methodName == null || memberInfo.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)
			        select memberInfo as MethodInfo).ToList();
		}
		#endregion

		#region Method Parameter Lookup
		/// <summary>
		/// Gets all parameters for the given <paramref name="method"/>.
		/// </summary>
		/// <returns>The list of parameters for the method.</returns>
		public static IList<ParameterInfo> Parameters(this MethodBase method)
		{
			return method.GetParameters();
		}

		/// <summary>
		/// Determines whether null can be assigned to the given <paramref name="parameter"/>.
		/// </summary>
		/// <returns>True if null can be assigned, false otherwise.</returns>
		public static bool IsNullable(this ParameterInfo parameter)
		{
			return ! parameter.ParameterType.IsValueType || parameter.ParameterType.IsSubclassOf(typeof (Nullable));
		}

		/// <summary>
		/// Determines whether the given <paramref name="parameter"/> has the given <paramref name="name"/>.
		/// The comparison uses OrdinalIgnoreCase and allows for a leading underscore in the parameter name
		/// to be ignored (this is useful when mapping data using reserved words to method parameters). 
		/// </summary>
		/// <returns>True if the name considered identical, false otherwise.</returns>
		public static bool HasName(this ParameterInfo parameter, string name)
		{
			bool result = parameter.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
			if (! result && parameter.Name.StartsWith("_"))
				result = parameter.Name.Substring(1).Equals(name, StringComparison.OrdinalIgnoreCase);
			return result;
		}

		/// <summary>
		/// Determines whether the given <paramref name="parameter"/> has an associated default value as
		/// supplied by an <see href="DefaultValueAttribute"/>. This method does not read the value of
		/// the attribute. It also does not support C# 4.0 default parameter specifications.
		/// </summary>
		/// <returns>True if the attribute was detected, false otherwise.</returns>
		public static bool HasDefaultValue( this ParameterInfo parameter )
		{
			var defaultValue = parameter.Attribute<DefaultValueAttribute>();
			return defaultValue != null;
		}

		/// <summary>
		/// Gets the default value associated with the given <paramref name="parameter"/>. The value is
		/// obtained from the <see href="DefaultValueAttribute"/> if present on the parameter. This method 
		/// does not support C# 4.0 default parameter specifications.
		/// </summary>
		/// <returns>True if the attribute was detected, false otherwise.</returns>
		public static object DefaultValue( this ParameterInfo parameter )
		{
			var defaultValue = parameter.Attribute<DefaultValueAttribute>();
			// TODO we should do type conversion here since attributes cannot hold all types
			return defaultValue != null ? defaultValue.Value : null;
		}
		#endregion
	}
}