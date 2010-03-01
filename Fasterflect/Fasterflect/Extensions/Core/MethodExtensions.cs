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
        /// Invokes the static method specified by <paramref name="name"/> of the given
        /// <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Leave <paramref name="parameters"/> empty if the method has no argument.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="Invoke(Type, string, Type[], object[])"/>
        public static object Invoke( this Type targetType, string name, params object[] parameters )
        {
            return DelegateForStaticInvoke( targetType, name, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="name"/> of the given
        /// <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="parameterTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string name, Type[] parameterTypes,
                                     params object[] parameters )
        {
            return
                DelegateForStaticInvoke( targetType, name, parameterTypes ?? parameters.GetTypeArray() )(
                    parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="name"/> matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Leave <paramref name="parameters"/> empty if the method has no argument.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="Invoke(System.Type,string,System.Type[],Fasterflect.Flags,object[])"/>
        public static object Invoke( this Type targetType, string name, Flags bindingFlags,
                                     params object[] parameters )
        {
            return DelegateForStaticInvoke( targetType, name, bindingFlags, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="name"/> matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="parameterTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string name, Type[] parameterTypes, Flags bindingFlags, params object[] parameters )
        {
            return DelegateForStaticInvoke( targetType, name, bindingFlags, parameterTypes )( parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="name"/> 
        /// whose parameter types are specified by <paramref name="parameterTypes"/> on the given <paramref name="targetType"/>.
        /// Leave <paramref name="parameterTypes"/> empty if the method has no argument.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this Type targetType, string name,
                                                                   params Type[] parameterTypes )
        {
            return DelegateForStaticInvoke( targetType, name, Flags.StaticAnyVisibility, parameterTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="name"/> 
        /// matching <paramref name="bindingFlags"/> whose parameter types are specified by <paramref name="parameterTypes"/> 
        /// on the given <paramref name="targetType"/>. Leave <paramref name="parameterTypes"/> empty if the 
        /// method has no argument.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this Type targetType, string name,
                                                                   Flags bindingFlags,
                                                                   params Type[] parameterTypes )
        {
            return (StaticMethodInvoker)
                   new MethodInvocationEmitter( targetType, bindingFlags, name, parameterTypes ).GetDelegate();
        }
        #endregion

        #region Instance Method Invocation
        /// <summary>
        /// Invokes the instance method specified by <paramref name="name"/> of the given
        /// <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Leave <paramref name="parameters"/> empty if the method has no argument.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="Invoke(object, string, Type[], object[])"/>
        public static object Invoke( this object target, string name, params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), name, parameters.GetTypeArray() )( target,
                                                                                                         parameters );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="name"/> of the given
        /// <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="parameterTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string name, Type[] parameterTypes,
                                     params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), name, parameterTypes )( target, parameters );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="name"/> matching <paramref name="bindingFlags"/> 
        /// of the given <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Leave <paramref name="parameters"/> empty if the method has no argument.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="Invoke(object,string,System.Type[],Fasterflect.Flags,object[])"/>
        public static object Invoke( this object target, string name, Flags bindingFlags,
                                     params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), name, bindingFlags, parameters.GetTypeArray() )(
            						  target, parameters );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="name"/> matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="parameterTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string name, Type[] parameterTypes, Flags bindingFlags, params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), name, bindingFlags, parameterTypes )( target,
                                                                                                            parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="name"/> 
        /// whose parameter types are specified by <paramref name="parameterTypes"/> on the given <paramref name="targetType"/>.
        /// Leave <paramref name="parameterTypes"/> empty if the method has no argument.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this Type targetType, string name,
                                                       params Type[] parameterTypes )
        {
            return DelegateForInvoke( targetType, name, Flags.InstanceAnyVisibility, parameterTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="name"/> 
        /// matching <paramref name="bindingFlags"/> whose parameter types are specified by 
        /// <paramref name="parameterTypes"/> on the given <paramref name="targetType"/>.
        /// Leave <paramref name="parameterTypes"/> empty if the method has no argument.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this Type targetType, string name, Flags bindingFlags,
                                                       params Type[] parameterTypes )
        {
            return (MethodInvoker)
                new MethodInvocationEmitter( targetType, bindingFlags, name, parameterTypes ).GetDelegate();
        }
        #endregion

        #endregion

        #region Method Lookup (Single)
        /// <summary>
        /// Gets the public or non-public instance method with the given <paramref name="name"/> on the
        /// given <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        /// default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        /// to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        /// by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method( this Type targetType, string name )
        {
            return targetType.Method( name, null, Flags.InstanceAnyVisibility );
        }

        /// <summary>
        /// Gets the public or non-public instance method with the given <paramref name="name"/> on the 
        /// given <paramref name="targetType"/> where the parameter types correspond in order with the
        /// supplied <paramref name="parameterTypes"/>.
        /// </summary>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        /// default behavior is to check for an exact, case-sensitive match.</param>
        /// <param name="parameterTypes">If this parameter is not null then only methods with the same 
        /// parameter signature will be included in the result.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method( this Type targetType, string name, Type[] parameterTypes )
        {
        	return targetType.Method( name, parameterTypes, Flags.InstanceAnyVisibility );
        }

    	/// <summary>
        /// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
        /// on the given <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        /// default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        /// to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        /// by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method( this Type targetType, string name, Flags bindingFlags )
        {
            return targetType.Method( name, null, bindingFlags );
        }

        /// <summary>
        /// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
        /// on the given <paramref name="targetType"/> where the parameter types correspond in order with the
        /// supplied <paramref name="parameterTypes"/>.
        /// </summary>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        ///   default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        ///   to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        ///   by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
        ///   will be included in the result. The default behavior is to check only for assignment compatibility,
        ///   but this can be changed to exact matching by passing <see href="Flags.ExactBinding"/>.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        ///   the search behavior and result filtering.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method( this Type targetType, string name, Type[] parameterTypes, Flags bindingFlags )
        {
            // we need to check all methods to do partial name matches
            if( bindingFlags.IsAnySet( Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented ) )
            {
                return targetType.Methods( parameterTypes, bindingFlags, name ).FirstOrDefault();
            }

            bool hasTypes = parameterTypes != null;
            var result = hasTypes
                             ? targetType.GetMethod( name, bindingFlags, null, parameterTypes, null )
                             : targetType.GetMethod( name, bindingFlags );
            if( result == null && bindingFlags.IsNotSet( Flags.DeclaredOnly ) )
            {
                if( targetType.BaseType != typeof(object) && targetType.BaseType != null )
                {
                    return targetType.BaseType.Method( name, parameterTypes, bindingFlags );
                }
            }
            bool hasSpecialFlags =
                bindingFlags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
            if( hasSpecialFlags )
            {
                IList<MethodInfo> methods = new List<MethodInfo> { result };
                methods = methods.Filter( bindingFlags );
                return methods.Count > 0 ? methods[ 0 ] : null;
            }
            return result;
        }
        #endregion

        #region Method Lookup (Multiple)
        /// <summary>
        /// Gets all public and non-public instance methods on the given <paramref name="targetType"/> that match the 
        /// given <paramref name="names"/>.
        /// </summary>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
		/// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type targetType, params string[] names )
        {
            return targetType.Methods( null, Flags.InstanceAnyVisibility, names );
        }

        /// <summary>
        /// Gets all public and non-public instance methods on the given <paramref name="targetType"/> that match the 
        /// given <paramref name="names"/>.
        /// </summary>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
		/// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type targetType, Flags bindingFlags, params string[] names )
        {
            return targetType.Methods( null, bindingFlags, names );
        }


        /// <summary>
        /// Gets all public and non-public instance methods on the given <paramref name="targetType"/> that match the given 
        ///  <paramref name="names"/>.
        /// </summary>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter 
        /// signature will be included in the result.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type targetType, Type[] parameterTypes, params string[] names )
        {
        	return targetType.Methods( parameterTypes, Flags.InstanceAnyVisibility, names );
        }

    	/// <summary>
        /// Gets all methods on the given <paramref name="targetType"/> that match the given lookup criteria and values.
        /// </summary>
        /// <param name="targetType">The type on which to reflect.</param>
        /// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility,
        /// but this can be changed to exact matching by passing <see href="Flags.ExactBinding"/>.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
		/// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type targetType, Type[] parameterTypes, Flags bindingFlags,
                                                 params string[] names )
        {
            if( targetType == null || targetType == typeof(object) )
            {
                return new MethodInfo[0];
            }

            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );
            bool hasNames = names != null && names.Length > 0;
            bool hasTypes = parameterTypes != null;
            bool hasSpecialFlags =
                bindingFlags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );

            if( ! recurse && ! hasNames && ! hasTypes && ! hasSpecialFlags )
            {
                return targetType.GetMethods( bindingFlags ) ?? new MethodInfo[0];
            }

            var methods = GetMethods( targetType, bindingFlags );
            methods = hasNames ? methods.Filter( bindingFlags, names ) : methods;
            methods = hasTypes ? methods.Filter( bindingFlags, parameterTypes ) : methods;
            methods = hasSpecialFlags ? methods.Filter( bindingFlags ) : methods;
            return methods;
        }

        private static IList<MethodInfo> GetMethods( Type targetType, Flags bindingFlags )
        {
            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );

            if( ! recurse )
            {
                return targetType.GetMethods( bindingFlags ) ?? new MethodInfo[0];
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var methods = new List<MethodInfo>();
            methods.AddRange( targetType.GetMethods( bindingFlags ) );
            Type baseType = targetType.BaseType;
            while( baseType != null && baseType != typeof(object) )
            {
                methods.AddRange( baseType.GetMethods( bindingFlags ) );
                baseType = baseType.BaseType;
            }
            return methods;
        }
        #endregion
    }
}