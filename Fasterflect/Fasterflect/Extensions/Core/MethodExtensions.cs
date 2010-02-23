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
using Fasterflect.Internal;
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
            return DelegateForStaticInvoke( targetType, methodName )();
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
            return
                DelegateForStaticInvoke( targetType, methodName, paramTypes ?? parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type
        /// <paramref name="targetType"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this Type targetType, Flags flags, string methodName)
        {
            return DelegateForStaticInvoke(targetType, flags, methodName, Type.EmptyTypes)();
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type
        /// <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are inferred from <paramref name="parameters"/> which 
        /// must contain no null argument or else, <c>NullReferenceException</c> is thrown.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this Type targetType, Flags flags, string methodName, params object[] parameters)
        {
            return DelegateForStaticInvoke( targetType, flags, methodName, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes the static method specified by <paramref name="methodName"/> of type
        /// <paramref name="targetType"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="paramTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this Type targetType, Flags flags, string methodName, Type[] paramTypes,
                                     params object[] parameters)
        {
            return DelegateForStaticInvoke( targetType, flags, methodName, paramTypes )( parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// on type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke(this Type targetType, string methodName)
        {
            return DelegateForStaticInvoke(targetType, methodName, Type.EmptyTypes);
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// whose parameter types are specified by <paramref name="paramTypes"/> on type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke(this Type targetType, string methodName,
                                                                   params Type[] paramTypes)
        {
            return DelegateForStaticInvoke(targetType, Flags.DefaultCriteria, methodName, paramTypes);
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// on type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke(this Type targetType, Flags flags, string methodName)
        {
            return DelegateForStaticInvoke( targetType, flags, methodName, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method <paramref name="methodName"/> 
        /// whose parameter types are specified by <paramref name="paramTypes"/> on type <paramref name="targetType"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke(this Type targetType, Flags flags, string methodName,
                                                                   params Type[] paramTypes)
        {
            return (StaticMethodInvoker)
                   new MethodInvocationEmitter(targetType, flags, methodName, paramTypes, true).GetDelegate();
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
            return DelegateForInvoke( target.GetTypeAdjusted(), methodName )( target );
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
        /// Invokes the instance method specified by <paramref name="methodName"/> of object
        /// <paramref name="target"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this object target, Flags flags, string methodName)
        {
            return DelegateForInvoke(target.GetTypeAdjusted(), flags, methodName, Type.EmptyTypes)(target);
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> of object
        /// <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are inferred from <paramref name="parameters"/> which 
        /// must contain no null argument or else, <c>NullReferenceException</c> is thrown.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this object target, Flags flags, string methodName, params object[] parameters)
        {
            return DelegateForInvoke(target.GetTypeAdjusted(), flags, methodName, parameters.GetTypeArray())(target,
                                                                                                         parameters);
        }

        /// <summary>
        /// Invokes the instance method specified by <paramref name="methodName"/> of object
        /// <paramref name="target"/> using <paramref name="parameters"/> as arguments.
        /// Method parameter types are specified by <paramref name="paramTypes"/>.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke(this object target, Flags flags, string methodName, Type[] paramTypes,
                                     params object[] parameters)
        {
            return DelegateForInvoke(target.GetTypeAdjusted(), flags, methodName, paramTypes)(target, parameters);
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// on type <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke(this Type targetType, string methodName)
        {
            return DelegateForInvoke( targetType, methodName, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// whose parameter types are specified by <paramref name="paramTypes"/> on type <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke(this Type targetType, string methodName, params Type[] paramTypes)
        {
            return DelegateForInvoke( targetType, Flags.DefaultCriteria, methodName, paramTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// on type <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke(this Type targetType, Flags flags, string methodName)
        {
            return DelegateForInvoke( targetType, flags, methodName, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method <paramref name="methodName"/> 
        /// whose parameter types are specified by <paramref name="paramTypes"/> on type <paramref name="targetType"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke(this Type targetType, Flags flags, string methodName, params Type[] paramTypes)
        {
            return
                (MethodInvoker)new MethodInvocationEmitter(targetType, flags, methodName, paramTypes, false).GetDelegate();
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
        	return type.Method( name, Flags.InstanceCriteria, null );
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
        	return type.Method( name, flags, null );
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
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method( this Type type, string name, Flags flags, Type[] paramTypes )
        {
			// we need to check all methods to do partial name matches
			if( flags.IsAnySet( Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented ) )
				return type.Methods( paramTypes, flags, name ).FirstOrDefault();

        	bool hasTypes = paramTypes != null;
       		var result = hasTypes ? type.GetMethod( name, flags, null, paramTypes, null ) : type.GetMethod( name, flags );
			if( result == null && flags.IsNotSet( Flags.DeclaredOnly ) )
			{ 
				if( type.BaseType != typeof(object) && type.BaseType != null )
        			return type.BaseType.Method( name, flags, paramTypes );
			}
			bool hasSpecialFlags = flags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
			if( hasSpecialFlags )
			{
				IList<MethodInfo> methods = new List<MethodInfo> { result };
				methods = methods.Filter( flags );
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
            return type.Methods( null, Flags.InstanceCriteria, null );
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
            return type.Methods( null, flags, null );
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
		    return type.Methods( null, Flags.InstanceCriteria, names );
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
            return type.Methods( null, flags, names );
        }

        /// <summary>
        /// Find all methods on the given <paramref name="type"/> that match the given lookup criteria and values.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="paramTypes">If this parameter is supplied then only methods with the same parameter signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility,
        /// but this can be changed to exact matching by passing <see href="Flags.ExactParameterMatch"/>.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">If this parameter is supplied then only methods whose name matches one of the supplied
        /// names will be included in the result. The default behavior is to check for an exact, case-sensitive match.
        /// Pass <see href="Flags.ExplicitNameMatch"/> to include explicitly implemented interface members, 
        /// <see href="Flags.PartialNameMatch"/> to locate by substring, and <see href="Flags.IgnoreCase"/> to 
        /// ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, Type[] paramTypes, Flags flags, params string[] names )
        {
			if( type == null || type == typeof(object) ) { return new MethodInfo[ 0 ]; }

			bool recurse = flags.IsNotSet( Flags.DeclaredOnly );
			bool hasNames = names != null && names.Length > 0;
			bool hasTypes = paramTypes != null;
			bool hasSpecialFlags = flags.IsAnySet( Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented );
        	
			if( ! recurse && ! hasNames && ! hasTypes && ! hasSpecialFlags )
				return type.GetMethods( flags ) ?? new MethodInfo[ 0 ];

			var methods = GetMethods( type, flags );
			methods = hasNames ? methods.Filter( flags, names ) : methods;
			methods = hasTypes ? methods.Filter( flags, paramTypes ) : methods;
			methods = hasSpecialFlags ? methods.Filter( flags ) : methods;
        	return methods;
		}

        private static IList<MethodInfo> GetMethods( Type type, Flags flags )
        {
			bool recurse = flags.IsNotSet( Flags.DeclaredOnly );

			if( ! recurse )
				return type.GetMethods( flags ) ?? new MethodInfo[ 0 ];

			flags |= Flags.DeclaredOnly;
			flags &= ~BindingFlags.FlattenHierarchy;

        	var methods = new List<MethodInfo>();
			methods.AddRange( type.GetMethods( flags ) );
			Type baseType = type.BaseType;
			while( baseType != null && baseType != typeof(object) )
			{
				methods.AddRange( baseType.GetMethods( flags ) );
			    baseType = baseType.BaseType;
			}
			return methods;
		}
        #endregion
    }
}