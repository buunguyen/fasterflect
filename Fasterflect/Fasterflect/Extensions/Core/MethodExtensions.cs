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
using Fasterflect.Internal;

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
        /// Invokes the static method specified by <paramref name="methodName"/> of the given
        /// <paramref name="targetType"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string methodName )
        {
            return DelegateForStaticInvoke( targetType, methodName )();
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of the given
        /// <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="Invoke(Type, string, Type[], object[])"/>
        public static object Invoke( this Type targetType, string methodName, params object[] parameters )
        {
            return DelegateForStaticInvoke( targetType, methodName, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of the given
        /// <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="parameterTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string methodName, Type[] parameterTypes,
                                     params object[] parameters )
        {
            return
                DelegateForStaticInvoke( targetType, methodName, parameterTypes ?? parameters.GetTypeArray() )(
                    parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="targetType"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string methodName, Flags bindingFlags )
        {
            return DelegateForStaticInvoke( targetType, methodName, bindingFlags, Type.EmptyTypes )();
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="Invoke(Type, string, Flags, Type[], object[])"/>
        public static object Invoke( this Type targetType, string methodName, Flags bindingFlags,
                                     params object[] parameters )
        {
            return DelegateForStaticInvoke( targetType, methodName, bindingFlags, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="parameterTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this Type targetType, string methodName, Flags bindingFlags, Type[] parameterTypes,
                                     params object[] parameters )
        {
            return DelegateForStaticInvoke( targetType, methodName, bindingFlags, parameterTypes )( parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// on the given <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this Type targetType, string methodName )
        {
            return DelegateForStaticInvoke( targetType, methodName, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// whose parameter types are specified by <paramref name="parameterTypes"/> on the given <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this Type targetType, string methodName,
                                                                   params Type[] parameterTypes )
        {
            return DelegateForStaticInvoke( targetType, methodName, Flags.StaticAnyVisibility, parameterTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/>
        /// matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this Type targetType, string methodName,
                                                                   Flags bindingFlags )
        {
            return DelegateForStaticInvoke( targetType, methodName, bindingFlags, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// matching <paramref name="bindingFlags"/> whose parameter types are specified by <paramref name="parameterTypes"/> on the given <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this Type targetType, string methodName,
                                                                   Flags bindingFlags,
                                                                   params Type[] parameterTypes )
        {
            return (StaticMethodInvoker)
                   new MethodInvocationEmitter( targetType, bindingFlags, methodName, parameterTypes ).GetDelegate();
        }
        #endregion

        #region Instance Method Invocation
        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> of the given
        /// <paramref name="target"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string methodName )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName )( target );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> of the given
        /// <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="Invoke(object, string, Type[], object[])"/>
        public static object Invoke( this object target, string methodName, params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName, parameters.GetTypeArray() )( target,
                                                                                                         parameters );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> of the given
        /// <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="parameterTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string methodName, Type[] parameterTypes,
                                     params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName, parameterTypes )( target, parameters );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="target"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string methodName, Flags bindingFlags )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName, bindingFlags, Type.EmptyTypes )( target );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> matching <paramref name="bindingFlags"/> 
        /// of the given <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="Invoke(object, string, Flags, Type[], object[])"/>
        public static object Invoke( this object target, string methodName, Flags bindingFlags,
                                     params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName, bindingFlags, parameters.GetTypeArray() )(
            						  target, parameters );
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> matching <paramref name="bindingFlags"/>
        /// of the given <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="parameterTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this object target, string methodName, Flags bindingFlags, Type[] parameterTypes,
                                     params object[] parameters )
        {
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName, bindingFlags, parameterTypes )( target,
                                                                                                            parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// on the given <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this Type targetType, string methodName )
        {
            return DelegateForInvoke( targetType, methodName, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// whose parameter types are specified by <paramref name="parameterTypes"/> on the given <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this Type targetType, string methodName,
                                                       params Type[] parameterTypes )
        {
            return DelegateForInvoke( targetType, methodName, Flags.InstanceAnyVisibility, parameterTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this Type targetType, string methodName, Flags bindingFlags )
        {
            return DelegateForInvoke( targetType, methodName, bindingFlags, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// matching <paramref name="bindingFlags"/> whose parameter types are specified by 
        /// <paramref name="parameterTypes"/> on the given <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this Type targetType, string methodName, Flags bindingFlags,
                                                       params Type[] parameterTypes )
        {
            return (MethodInvoker)
                new MethodInvocationEmitter( targetType, bindingFlags, methodName, parameterTypes ).GetDelegate();
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
            return type.Method( name, Flags.InstanceAnyVisibility, null );
        }

        /// <summary>
        /// Find the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
        /// on the given <paramref name="type"/>.
        /// Use the <seealso href="MethodDeclared"/> method if you do not wish to include base types in the search.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        /// default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        /// to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        /// by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method( this Type type, string name, Flags bindingFlags )
        {
            return type.Method( name, bindingFlags, null );
        }

        /// <summary>
        /// Find the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
        /// on the given <paramref name="type"/> where the parameter types correspond in order with the
        /// given <paramref name="parameterTypes"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        /// default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        /// to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        /// by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility,
        /// but this can be changed to exact matching by passing <see href="Flags.ExactParameterMatch"/>.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method( this Type type, string name, Flags bindingFlags, Type[] parameterTypes )
        {
            // we need to check all methods to do partial name matches
            if( bindingFlags.IsAnySet( Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented ) )
            {
                return type.Methods( parameterTypes, bindingFlags, name ).FirstOrDefault();
            }

            bool hasTypes = parameterTypes != null;
            var result = hasTypes
                             ? type.GetMethod( name, bindingFlags, null, parameterTypes, null )
                             : type.GetMethod( name, bindingFlags );
            if( result == null && bindingFlags.IsNotSet( Flags.DeclaredOnly ) )
            {
                if( type.BaseType != typeof(object) && type.BaseType != null )
                {
                    return type.BaseType.Method( name, bindingFlags, parameterTypes );
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
        /// Find all public and non-public instance methods on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type )
        {
            return type.Methods( null, Flags.InstanceAnyVisibility, null );
        }

        /// <summary>
        /// Find all methods on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, Flags bindingFlags )
        {
            return type.Methods( null, bindingFlags, null );
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
            return type.Methods( null, Flags.InstanceAnyVisibility, names );
        }

        /// <summary>
        /// Find all public and non-public instance methods on the given <paramref name="type"/> that match the 
        /// given <paramref name="names"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">If this parameter is supplied then only methods whose name matches one of the supplied
        /// names will be included in the result. The default behavior is to check for an exact, case-sensitive match.
        /// Pass <see href="Flags.ExplicitNameMatch"/> to include explicitly implemented interface members, 
        /// <see href="Flags.PartialNameMatch"/> to locate by substring, and <see href="Flags.IgnoreCase"/> to 
        /// ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, Flags bindingFlags, params string[] names )
        {
            return type.Methods( null, bindingFlags, names );
        }

        /// <summary>
        /// Find all methods on the given <paramref name="type"/> that match the given lookup criteria and values.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility,
        /// but this can be changed to exact matching by passing <see href="Flags.ExactParameterMatch"/>.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">If this parameter is supplied then only methods whose name matches one of the supplied
        /// names will be included in the result. The default behavior is to check for an exact, case-sensitive match.
        /// Pass <see href="Flags.ExplicitNameMatch"/> to include explicitly implemented interface members, 
        /// <see href="Flags.PartialNameMatch"/> to locate by substring, and <see href="Flags.IgnoreCase"/> to 
        /// ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, Type[] parameterTypes, Flags bindingFlags,
                                                 params string[] names )
        {
            if( type == null || type == typeof(object) )
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
                return type.GetMethods( bindingFlags ) ?? new MethodInfo[0];
            }

            var methods = GetMethods( type, bindingFlags );
            methods = hasNames ? methods.Filter( bindingFlags, names ) : methods;
            methods = hasTypes ? methods.Filter( bindingFlags, parameterTypes ) : methods;
            methods = hasSpecialFlags ? methods.Filter( bindingFlags ) : methods;
            return methods;
        }

        private static IList<MethodInfo> GetMethods( Type type, Flags bindingFlags )
        {
            bool recurse = bindingFlags.IsNotSet( Flags.DeclaredOnly );

            if( ! recurse )
            {
                return type.GetMethods( bindingFlags ) ?? new MethodInfo[0];
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var methods = new List<MethodInfo>();
            methods.AddRange( type.GetMethods( bindingFlags ) );
            Type baseType = type.BaseType;
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