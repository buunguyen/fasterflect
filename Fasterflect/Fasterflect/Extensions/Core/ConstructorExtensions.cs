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
        /// Invokes the no-arg constructor on the given <paramref name="targetType"/>.
        /// </summary>
        public static object CreateInstance( this Type targetType )
        {
            return DelegateForCreateInstance( targetType )();
        }

        /// <summary>
        /// Invokes a constructor whose parameter types are inferred from <paramref name="parameters" /> 
        /// on the given <paramref name="targetType"/> with <paramref name="parameters" /> being the arguments.
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
        /// Invokes the no-arg constructor matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/>.
        /// </summary>
        public static object CreateInstance( this Type targetType, Flags bindingFlags )
        {
            return DelegateForCreateInstance( targetType, bindingFlags )();
        }

        /// <summary>
        /// Invokes a constructor whose parameter types are inferred from <paramref name="parameters" /> and
        /// matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/> 
        /// with <paramref name="parameters" /> being the arguments.
        /// </summary>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="CreateInstance(Type, Flags, Type[], object[])"/>
        public static object CreateInstance( this Type targetType, Flags bindingFlags, params object[] parameters )
        {
            return DelegateForCreateInstance( targetType, bindingFlags, parameters.GetTypeArray() )( parameters );
        }

        /// <summary>
        /// Invokes a constructor whose parameter types are <paramref name="parameterTypes" /> and
        /// matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/> 
        /// with <paramref name="parameters" /> being the arguments.
        /// </summary>
        public static object CreateInstance( this Type targetType, Flags bindingFlags, Type[] parameterTypes,
                                             params object[] parameters )
        {
            return DelegateForCreateInstance( targetType, bindingFlags, parameterTypes )( parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the no-arg constructor on the given <paramref name="targetType"/>.
        /// </summary>
        public static ConstructorInvoker DelegateForCreateInstance( this Type targetType )
        {
            return DelegateForCreateInstance( targetType, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the constructor whose parameter types are <paramref name="parameterTypes" />
        /// on the given <paramref name="targetType"/>. 
        /// </summary>
        public static ConstructorInvoker DelegateForCreateInstance( this Type targetType, params Type[] parameterTypes )
        {
            return DelegateForCreateInstance( targetType, Flags.AllInstance, parameterTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the no-arg constructor matching <paramref name="bindingFlags"/>
        /// on the given <paramref name="targetType"/>.
        /// </summary>
        public static ConstructorInvoker DelegateForCreateInstance( this Type targetType, Flags bindingFlags )
        {
            return DelegateForCreateInstance( targetType, bindingFlags, Type.EmptyTypes );
        }

        /// <summary>
        /// Creates a delegate which can invoke the constructor whose parameter types are <paramref name="parameterTypes" />
        /// and matching <paramref name="bindingFlags"/> on the given <paramref name="targetType"/>. 
        /// </summary>
        public static ConstructorInvoker DelegateForCreateInstance( this Type targetType, Flags bindingFlags,
                                                                    params Type[] parameterTypes )
        {
            return
                (ConstructorInvoker) new CtorInvocationEmitter( targetType, bindingFlags, parameterTypes ).GetDelegate();
        }
        #endregion

        #region Constructor Invocation (TryCreateInstance)
        /// <summary>
        /// Creates an instance of the given <paramref name="type"/> using the public properties of the 
        /// supplied <paramref name="source"/> object as input.
        /// This method will try to determine the least-cost route to constructing the instance, which
        /// implies mapping as many properties as possible to constructor parameters. Remaining properties
        /// on the source are mapped to properties on the created instance or ignored if none matches.
        /// TryCreateInstance is very liberal and attempts to convert values that are not otherwise
        /// considered compatible, such as between strings and enums or numbers, Guids and byte[], etc.
        /// </summary>
        /// <returns>An instance of type <paramref name="type"/>.</returns>
        public static object TryCreateInstance( this Type type, object source )
        {
            Type sourceType = source.GetType();
            SourceInfo sourceInfo = sourceInfoCache.Get( sourceType );
            if( sourceInfo == null )
            {
                sourceInfo = new SourceInfo( sourceType );
                sourceInfoCache.Insert( sourceType, sourceInfo );
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
        /// TryCreateInstance is very liberal and attempts to convert values that are not otherwise
        /// considered compatible, such as between strings and enums or numbers, Guids and byte[], etc.
        /// </summary>
        /// <returns>An instance of type <paramref name="type"/>.</returns>
        public static object TryCreateInstance( this Type type, IDictionary<string, object> parameters )
        {
            string[] names = parameters.Keys.ToArray();
            object[] values = parameters.Values.ToArray();
            return type.TryCreateInstance( names, values );
        }

        /// <summary>
        /// Creates an instance of the given <paramref name="type"/> using the supplied parameter information as input.
        /// Parameter types are inferred from the supplied <paramref name="parameterValues"/> and as such these
        /// should not be null.
        /// This method will try to determine the least-cost route to constructing the instance, which
        /// implies mapping as many properties as possible to constructor parameters. Remaining properties
        /// on the source are mapped to properties on the created instance or ignored if none matches.
        /// TryCreateInstance is very liberal and attempts to convert values that are not otherwise
        /// considered compatible, such as between strings and enums or numbers, Guids and byte[], etc.
        /// </summary>
        /// <param name="type">The type of which an instance should be created.</param>
        /// <param name="parameterNames">The names of the supplied parameters.</param>
        /// <param name="parameterValues">The values of the supplied parameters.</param>
        /// <returns>An instance of type <paramref name="type"/>.</returns>
        public static object TryCreateInstance( this Type type, string[] parameterNames, object[] parameterValues )
        {
            var parameterTypes = new Type[parameterValues.Length];
            for( int i = 0; i < parameterNames.Length; i++ )
            {
                object value = parameterValues[ i ];
                parameterTypes[ i ] = value != null ? value.GetType() : null;
            }
            return type.TryCreateInstance( parameterNames, parameterTypes, parameterValues );
        }

        /// <summary>
        /// Creates an instance of the given <paramref name="type"/> using the supplied parameter information as input.
        /// This method will try to determine the least-cost route to constructing the instance, which
        /// implies mapping as many properties as possible to constructor parameters. Remaining properties
        /// on the source are mapped to properties on the created instance or ignored if none matches.
        /// TryCreateInstance is very liberal and attempts to convert values that are not otherwise
        /// considered compatible, such as between strings and enums or numbers, Guids and byte[], etc.
        /// </summary>
        /// <param name="type">The type of which an instance should be created.</param>
        /// <param name="parameterNames">The names of the supplied parameters.</param>
        /// <param name="parameterTypes">The types of the supplied parameters.</param>
        /// <param name="parameterValues">The values of the supplied parameters.</param>
        /// <returns>An instance of type <paramref name="type"/>.</returns>
        public static object TryCreateInstance( this Type type, string[] parameterNames, Type[] parameterTypes,
                                                object[] parameterValues )
        {
            MethodMap map = type.PrepareInvoke( parameterNames, parameterTypes, parameterValues );
            return map.Invoke( parameterValues );
        }
        #endregion

        #region Constructor Lookup (Single)
        /// <summary>
        /// Find the constructor corresponding to the supplied <paramref name="parameterTypes"/> on the
        /// given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <param name="parameterTypes">The types of the constructor parameters in order.</param>
        /// <returns>The matching constructor or null if no match was found.</returns>
        public static ConstructorInfo Constructor( this Type type, params Type[] parameterTypes )
        {
            return type.Constructor( Flags.InstanceCriteria, parameterTypes );
        }

        /// <summary>
        /// Find the constructor matching the given <paramref name="bindingFlags"/> and corresponding to the 
        /// supplied <paramref name="parameterTypes"/> on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <param name="bindingFlags">The search criteria to use when reflecting.</param>
        /// <param name="parameterTypes">The types of the constructor parameters in order.</param>
        /// <returns>The matching constructor or null if no match was found.</returns>
        public static ConstructorInfo Constructor( this Type type, Flags bindingFlags, params Type[] parameterTypes )
        {
            return type.GetConstructor( bindingFlags, null, parameterTypes, null );
        }
        #endregion

        #region Constructor Lookup (Multiple)
        /// <summary>
        /// Find all public and non-public constructors (that are not abstract) on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <returns>A list of matching constructors. This value will never be null.</returns>
        public static IList<ConstructorInfo> Constructors( this Type type )
        {
            return type.Constructors( Flags.InstanceCriteria );
        }

        /// <summary>
        /// Find all constructors matching the given <paramref name="bindingFlags"/> (and that are not abstract)
        /// on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <param name="bindingFlags">The search criteria to use when reflecting.</param>
        /// <returns>A list of matching constructors. This value will never be null.</returns>
        public static IList<ConstructorInfo> Constructors( this Type type, Flags bindingFlags )
        {
            return type.GetConstructors( bindingFlags ); //.Where( ci => !ci.IsAbstract ).ToList();
        }
        #endregion
    }
}