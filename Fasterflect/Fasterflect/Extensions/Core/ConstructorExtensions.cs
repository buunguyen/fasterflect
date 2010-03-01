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
using Fasterflect.Caching;
using Fasterflect.Emitter;
using Fasterflect.ObjectConstruction;

namespace Fasterflect
{
    /// <summary>
    /// Extension methods for locating, inspecting and invoking constructors.
    /// </summary>
    public static class ConstructorExtensions
    {
        /// <summary>
        /// This field is used to cache information on objects used as parameters for object construction, which
        /// improves performance for subsequent instantiations of the same type using a compatible source type.
        /// </summary>
        private static readonly Cache<Type, SourceInfo> sourceInfoCache = new Cache<Type, SourceInfo>();

        #region Constructor Invocation (CreateInstance)
        /// <summary>
        /// Invokes a constructor whose parameter types are inferred from <paramref name="parameters" /> 
        /// on the given <paramref name="targetType"/> with <paramref name="parameters" /> being the arguments.
        /// Leave <paramref name="parameters"/> empty if the constructor has no argument.
        /// </summary>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="CreateInstance(Type, Type[], object[])"/>
        public static object CreateInstance( this Type targetType, params object[] parameters )
        {
            return DelegateForCreateInstance( targetType, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes a constructor having parameter types specified by <paramref name="parameterTypes" /> 
        /// on the the given <paramref name="targetType"/> with <paramref name="parameters" /> being the arguments.
        /// </summary>
        public static object CreateInstance( this Type targetType, Type[] parameterTypes, params object[] parameters )
        {
            return DelegateForCreateInstance( targetType, parameterTypes )( parameters );
        }

        /// <summary>
        /// Invokes a constructor whose parameter types are inferred from <paramref name="parameters" /> and
        /// matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/> 
        /// with <paramref name="parameters" /> being the arguments. 
        /// Leave <paramref name="parameters"/> empty if the constructor has no argument.
        /// </summary>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="CreateInstance(System.Type,System.Type[],Fasterflect.Flags,object[])"/>
        public static object CreateInstance( this Type targetType, Flags bindingFlags, params object[] parameters )
        {
            return DelegateForCreateInstance( targetType, bindingFlags, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes a constructor whose parameter types are <paramref name="parameterTypes" /> and
        /// matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/> 
        /// with <paramref name="parameters" /> being the arguments.
        /// </summary>
        public static object CreateInstance( this Type targetType, Type[] parameterTypes, Flags bindingFlags, params object[] parameters )
        {
            return DelegateForCreateInstance( targetType, bindingFlags, parameterTypes )( parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the constructor whose parameter types are <paramref name="parameterTypes" />
        /// on the given <paramref name="targetType"/>.  Leave <paramref name="parameterTypes"/> empty if the constructor
        /// has no argument.
        /// </summary>
        public static ConstructorInvoker DelegateForCreateInstance( this Type targetType, params Type[] parameterTypes )
        {
            return DelegateForCreateInstance( targetType, Flags.InstanceAnyVisibility, parameterTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the constructor whose parameter types are <paramref name="parameterTypes" />
        /// and matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/>.  
        /// Leave <paramref name="parameterTypes"/> empty if the constructor has no argument. 
        /// </summary>
        public static ConstructorInvoker DelegateForCreateInstance( this Type targetType, Flags bindingFlags,
                                                                    params Type[] parameterTypes )
        {
            return (ConstructorInvoker) new CtorInvocationEmitter( targetType, bindingFlags, parameterTypes ).GetDelegate();
        }
        #endregion

        #region Constructor Invocation (TryCreateInstance)
        /// <summary>
        /// Creates an instance of the given <paramref name="targetType"/> using the public properties of the 
        /// supplied <paramref name="sample"/> object as input.
        /// This method will try to determine the least-cost route to constructing the instance, which
        /// implies mapping as many properties as possible to constructor parameters. Remaining properties
        /// on the source are mapped to properties on the created instance or ignored if none matches.
        /// TryCreateInstance is very liberal and attempts to convert values that are not otherwise
        /// considered compatible, such as between strings and enums or numbers, Guids and byte[], etc.
        /// </summary>
        /// <returns>An instance of targetType <paramref name="targetType"/>.</returns>
        public static object TryCreateInstance( this Type targetType, object sample )
        {
            Type sourceType = sample.GetType();
            SourceInfo sourceInfo = sourceInfoCache.Get( sourceType );
            if( sourceInfo == null )
            {
                sourceInfo = new SourceInfo( sourceType );
                sourceInfoCache.Insert( sourceType, sourceInfo );
            }
            object[] paramValues = sourceInfo.GetParameterValues( sample );
            MethodMap map = targetType.PrepareInvoke( sourceInfo.ParamNames, sourceInfo.ParamTypes, paramValues );
            return map.Invoke( paramValues );
        }

        /// <summary>
        /// Creates an instance of the given <paramref name="targetType"/> using the values in the supplied
        /// <paramref name="parameters"/> dictionary as input.
        /// This method will try to determine the least-cost route to constructing the instance, which
        /// implies mapping as many values as possible to constructor parameters. Remaining values
        /// are mapped to properties on the created instance or ignored if none matches.
        /// TryCreateInstance is very liberal and attempts to convert values that are not otherwise
        /// considered compatible, such as between strings and enums or numbers, Guids and byte[], etc.
        /// </summary>
        /// <returns>An instance of targetType <paramref name="targetType"/>.</returns>
        public static object TryCreateInstance( this Type targetType, IDictionary<string, object> parameters )
        {
			bool hasParameters = parameters != null && parameters.Count > 0;
            string[] names = hasParameters ? parameters.Keys.ToArray() : new string[ 0 ];
            object[] values = hasParameters ? parameters.Values.ToArray() : new object[ 0 ];
            return targetType.TryCreateInstance( names, values );
        }

        /// <summary>
        /// Creates an instance of the given <paramref name="targetType"/> using the supplied parameter information as input.
        /// Parameter types are inferred from the supplied <paramref name="parameterValues"/> and as such these
        /// should not be null.
        /// This method will try to determine the least-cost route to constructing the instance, which
        /// implies mapping as many properties as possible to constructor parameters. Remaining properties
        /// on the source are mapped to properties on the created instance or ignored if none matches.
        /// TryCreateInstance is very liberal and attempts to convert values that are not otherwise
        /// considered compatible, such as between strings and enums or numbers, Guids and byte[], etc.
        /// </summary>
        /// <param name="targetType">The targetType of which an instance should be created.</param>
        /// <param name="parameterNames">The names of the supplied parameters.</param>
        /// <param name="parameterValues">The values of the supplied parameters.</param>
        /// <returns>An instance of targetType <paramref name="targetType"/>.</returns>
        public static object TryCreateInstance( this Type targetType, string[] parameterNames, object[] parameterValues )
        {
        	var names = parameterNames ?? new string[ 0 ];
			var values = parameterValues ?? new object[ 0 ];
			if( names.Length != values.Length )
			{
				throw new ArgumentException( "Mismatching name and value arrays (must be of identical length)." );
			}
            var parameterTypes = new Type[ names.Length ];
			for( int i = 0; i < names.Length; i++ )
            {
                object value = values[ i ];
                parameterTypes[ i ] = value != null ? value.GetType() : null;
            }
            return targetType.TryCreateInstance( names, parameterTypes, values );
        }

        /// <summary>
        /// Creates an instance of the given <paramref name="targetType"/> using the supplied parameter information as input.
        /// This method will try to determine the least-cost route to constructing the instance, which
        /// implies mapping as many properties as possible to constructor parameters. Remaining properties
        /// on the source are mapped to properties on the created instance or ignored if none matches.
        /// TryCreateInstance is very liberal and attempts to convert values that are not otherwise
        /// considered compatible, such as between strings and enums or numbers, Guids and byte[], etc.
        /// </summary>
        /// <param name="targetType">The targetType of which an instance should be created.</param>
        /// <param name="parameterNames">The names of the supplied parameters.</param>
        /// <param name="parameterTypes">The types of the supplied parameters.</param>
        /// <param name="parameterValues">The values of the supplied parameters.</param>
        /// <returns>An instance of targetType <paramref name="targetType"/>.</returns>
        public static object TryCreateInstance( this Type targetType, string[] parameterNames, Type[] parameterTypes,
                                                object[] parameterValues )
        {
        	var names = parameterNames ?? new string[ 0 ];
        	var types = parameterTypes ?? new Type[ 0 ];
			var values = parameterValues ?? new object[ 0 ];
			if( names.Length != values.Length || names.Length != types.Length )
			{
				throw new ArgumentException( "Mismatching name, targetType and value arrays (must be of identical length)." );
			}
            MethodMap map = targetType.PrepareInvoke( names, types, values );
            return map.Invoke( values );
        }
        #endregion

        #region Constructor Lookup (Single)
        /// <summary>
        /// Gets the constructor corresponding to the supplied <paramref name="parameterTypes"/> on the
        /// given <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type to reflect on.</param>
        /// <param name="parameterTypes">The types of the constructor parameters in order.</param>
        /// <returns>The matching constructor or null if no match was found.</returns>
        public static ConstructorInfo Constructor( this Type targetType, params Type[] parameterTypes )
        {
            return targetType.Constructor( Flags.InstanceAnyVisibility, parameterTypes );
        }

        /// <summary>
        /// Gets the constructor matching the given <paramref name="bindingFlags"/> and corresponding to the 
        /// supplied <paramref name="parameterTypes"/> on the given <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type to reflect on.</param>
        /// <param name="bindingFlags">The search criteria to use when reflecting.</param>
        /// <param name="parameterTypes">The types of the constructor parameters in order.</param>
        /// <returns>The matching constructor or null if no match was found.</returns>
        public static ConstructorInfo Constructor( this Type targetType, Flags bindingFlags, params Type[] parameterTypes )
        {
            return targetType.GetConstructor( bindingFlags, null, parameterTypes, null );
        }
        #endregion

        #region Constructor Lookup (Multiple)
        /// <summary>
        /// Gets all public and non-public constructors (that are not abstract) on the given <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The targetType to reflect on.</param>
        /// <returns>A list of matching constructors. This value will never be null.</returns>
        public static IList<ConstructorInfo> Constructors( this Type targetType )
        {
            return targetType.Constructors( Flags.InstanceAnyVisibility );
        }

        /// <summary>
        /// Gets all constructors matching the given <paramref name="bindingFlags"/> (and that are not abstract)
        /// on the given <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The targetType to reflect on.</param>
        /// <param name="bindingFlags">The search criteria to use when reflecting.</param>
        /// <returns>A list of matching constructors. This value will never be null.</returns>
        public static IList<ConstructorInfo> Constructors( this Type targetType, Flags bindingFlags )
        {
            return targetType.GetConstructors( bindingFlags ); //.Where( ci => !ci.IsAbstract ).ToList();
        }
        #endregion
    }
}