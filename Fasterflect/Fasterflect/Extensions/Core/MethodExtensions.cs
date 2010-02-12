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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Fasterflect.Emitter;
using Fasterflect.Selectors.Core;

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
        /// <paramref name="targetType"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string methodName )
        {
            return DelegateForStaticInvoke( targetType, methodName, Type.EmptyTypes )();
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type
        /// <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are inferred from <paramref name="parameters"/> which 
        /// must contain no null argument or else, <c>NullReferenceException</c> is thrown.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string methodName, params object[] parameters )
        {
            return DelegateForStaticInvoke( targetType, methodName, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type
        /// <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="paramTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string methodName, Type[] paramTypes,
                                     params object[] parameters )
        {
            return DelegateForStaticInvoke( targetType, methodName, paramTypes )( parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// on type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this Type targetType, string methodName )
        {
            return (StaticMethodInvoker)
                   new MethodInvocationEmitter( methodName, targetType, Type.EmptyTypes, true ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// whose parameter types are specified by <paramref name="paramTypes"/> on type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this Type targetType, string methodName,
                                                                   params Type[] paramTypes )
        {
            return (StaticMethodInvoker)
                   new MethodInvocationEmitter( methodName, targetType, paramTypes, true ).GetDelegate();
        }
        #endregion

        #region Instance Method Invocation
        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> of object
        /// <paramref name="target"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string methodName )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName, Type.EmptyTypes )( target );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> of object
        /// <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are inferred from <paramref name="parameters"/> which 
        /// must contain no null argument or else, <c>NullReferenceException</c> is thrown.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string methodName, params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName, parameters.GetTypeArray() )( target,
                                                                                                         parameters );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> of object
        /// <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="paramTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string methodName, Type[] paramTypes,
                                     params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName, paramTypes )( target, parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// on type <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this Type targetType, string methodName )
        {
            return
                (MethodInvoker)
                new MethodInvocationEmitter( methodName, targetType, Type.EmptyTypes, false ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// whose parameter types are specified by <paramref name="paramTypes"/> on type <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this Type targetType, string methodName, params Type[] paramTypes )
        {
            return
                (MethodInvoker) new MethodInvocationEmitter( methodName, targetType, paramTypes, false ).GetDelegate();
        }
        #endregion
        #endregion

        #region Method Lookup (Single)
        /// <summary>
        /// Find the public or non-public instance method with the given <paramref name="name"/> on the
        /// given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        /// default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        /// to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        /// by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method( this Type type, string name )
        {
        	return type.Method( name, Flags.InstanceCriteria, null, null );
        }

        /// <summary>
        /// Find the method with the given <paramref name="name"/> and matching <paramref name="flags"/>
        /// on the given <paramref name="type"/>.
        /// Use the <seealso href="MethodDeclared"/> method if you do not wish to include base types in the search.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        /// default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        /// to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        /// by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method( this Type type, string name, Flags flags )
        {
        	return type.Method( name, flags, null, null );
        }

        /// <summary>
        /// Find the method with the given <paramref name="name"/> and matching <paramref name="flags"/>
        /// on the given <paramref name="type"/> where the parameter types correspond in order with the
        /// given <paramref name="paramTypes"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        /// default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        /// to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        /// by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="paramTypes">If this parameter is supplied then only methods with the same parameter signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility,
        /// but this can be changed to exact matching by passing <see href="Flags.ExactParameterMatch"/>.</param>
        /// <param name="returnType">If this parameter is supplied then only methods with the same return type signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility, but this 
        /// can be changed to exact matching by passing <see href="Flags.ExactParameterMatch"/>.
        /// </param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method( this Type type, string name, Flags flags, Type[] paramTypes, Type returnType )
        {
			if( string.IsNullOrEmpty( name ) )
				throw new ArgumentException( "You must supply a valid name to search for.", "name" );
        	return type.Methods( flags, paramTypes, returnType, name ).FirstOrDefault();
        }
        #endregion

        #region Method Lookup (Multiple)
        /// <summary>
        /// Find all public and non-public instance methods on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type )
        {
            return type.Methods( Flags.InstanceCriteria, null, (Type) null, null );
        }

        /// <summary>
        /// Find all methods on the given <paramref name="type"/> that match the specified <paramref name="flags"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, Flags flags )
        {
            return type.Methods( flags, null, (Type) null, null );
        }

        /// <summary>
        /// Find all public and non-public instance methods on the given <paramref name="type"/> that match the 
        /// given <paramref name="names"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="names">If this parameter is supplied then only methods whose name matches one of the supplied
        /// names will be included in the result. The default behavior is to check for an exact, case-sensitive match.
        /// Pass <see href="Flags.ExplicitNameMatch"/> to include explicitly implemented interface members, 
        /// <see href="Flags.PartialNameMatch"/> to locate by substring, and <see href="Flags.IgnoreCase"/> to 
        /// ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
		public static IList<MethodInfo> Methods( this Type type, params string[] names )
		{
		    return type.Methods( Flags.InstanceCriteria, null, null, names );
		}

        /// <summary>
        /// Find all public and non-public instance methods on the given <paramref name="type"/> that match the 
        /// given <paramref name="names"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">If this parameter is supplied then only methods whose name matches one of the supplied
        /// names will be included in the result. The default behavior is to check for an exact, case-sensitive match.
        /// Pass <see href="Flags.ExplicitNameMatch"/> to include explicitly implemented interface members, 
        /// <see href="Flags.PartialNameMatch"/> to locate by substring, and <see href="Flags.IgnoreCase"/> to 
        /// ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, Flags flags, params string[] names )
        {
            return type.Methods( flags, null, null, names );
        }

        /// <summary>
        /// Find all methods on the given <paramref name="type"/> that match the given lookup criteria and values.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="paramTypes">If this parameter is supplied then only methods with the same parameter signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility,
        /// but this can be changed to exact matching by passing <see href="Flags.ExactParameterMatch"/>.</param>
        /// <param name="returnType">If this parameter is supplied then only methods with the same return type signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility, but this 
        /// can be changed to exact matching by passing <see href="Flags.ExactParameterMatch"/>.
        /// </param>
        /// <param name="names">If this parameter is supplied then only methods whose name matches one of the supplied
        /// names will be included in the result. The default behavior is to check for an exact, case-sensitive match.
        /// Pass <see href="Flags.ExplicitNameMatch"/> to include explicitly implemented interface members, 
        /// <see href="Flags.PartialNameMatch"/> to locate by substring, and <see href="Flags.IgnoreCase"/> to 
        /// ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, Flags flags, Type[] paramTypes, Type returnType, params string[] names )
        {
        	flags = flags ?? Flags.Default;
        	flags = Flags.SetIf( flags, Flags.ParameterMatch, paramTypes != null );
        	flags = Flags.ClearIf( flags, Flags.ExactParameterMatch, paramTypes == null );
        	var methods = type.Members( MemberTypes.Method, flags, names ).Cast<MethodInfo>().ToList();

			var selectors = SelectorFactory.GetMethodSelectors( flags );
			return methods.Where( m => selectors.All( s => s.IsMatch( m, flags, paramTypes, returnType ) ) ).ToList();
        }
        #endregion
    }
}