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

        #region Method Lookup
        #region Single Method
        /// <summary>
        /// Find the public or non-public instance method with the given <paramref name="name"/> on the
        /// given <paramref name="type"/>. 
        /// Use the <seealso href="Method"/> method if you wish to include base types in the search.
        /// </summary>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading an exception will be raised.</returns>
        public static MethodInfo MethodDeclared( this Type type, string name )
        {
            return type.MethodDeclared( name, Flags.InstanceCriteria );
        }

        /// <summary>
        /// Find the method with the given <paramref name="name"/> and matching <paramref name="flags"/>
        /// on the given <paramref name="type"/>.
        /// Use the <seealso href="Method"/> method if you wish to include base types in the search.
        /// </summary>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading an exception will be raised.</returns>
        public static MethodInfo MethodDeclared( this Type type, string name, BindingFlags flags )
        {
            return type.GetMethod( name, flags );
        }

		/// <summary>
        /// Find the public or non-public instance method with the given <paramref name="name"/> on the
        /// given <paramref name="type"/>. 
        /// Use the <seealso href="MethodDeclared"/> method if you do not wish to include base types in the search.
        /// </summary>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading an exception will be raised.</returns>
        public static MethodInfo Method( this Type type, string name )
        {
            return type.Method( name, Flags.InstanceCriteria );
        }

        /// <summary>
        /// Find the method with the given <paramref name="name"/> and matching <paramref name="flags"/>
        /// on the given <paramref name="type"/>.
        /// Use the <seealso href="MethodDeclared"/> method if you do not wish to include base types in the search.
        /// </summary>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading an exception will be raised.</returns>
        public static MethodInfo Method( this Type type, string name, BindingFlags flags )
        {
            return type.Methods( flags ).FirstOrDefault( m => m.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
        }

        /// <summary>
        /// Find the method with the given <paramref name="name"/> and matching <paramref name="flags"/>
        /// on the given <paramref name="target"/> object, where the parameter types correspond in order with the
        /// given <paramref name="paramTypes"/>.
        /// Use the <seealso href="MethodDeclared"/> method if you do not wish to include base types in the search.
        /// </summary>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading an exception will be raised.</returns>
        public static MethodInfo Method( this object target, string name, BindingFlags flags, Type[] paramTypes )
        {
        	return target.GetTypeAdjusted().Method( name, flags, paramTypes );
        }
 
        /// <summary>
        /// Find the method with the given <paramref name="name"/> and matching <paramref name="flags"/>
        /// on the given <paramref name="type"/> where the parameter types correspond in order with the
        /// given <paramref name="paramTypes"/>.
        /// Use the <seealso href="MethodDeclared"/> method if you do not wish to include base types in the search.
        /// </summary>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading an exception will be raised.</returns>
        public static MethodInfo Method( this Type type, string name, BindingFlags flags, Type[] paramTypes )
        {
        	IList<MethodInfo> methods = type.Methods( flags, name );
        	bool findAnyOverload = paramTypes == null; // || paramTypes.Length == 0;
			if( findAnyOverload )
			{
				return methods.Count > 0 ? methods[ 0 ] : null;
			}
        	foreach( var method in methods )
        	{
        		var methodParameterTypes = method.Parameters().Select( p => p.ParameterType ).ToList();
				if( methodParameterTypes.Count != paramTypes.Length )
					continue;
				for( int i=0; i<methodParameterTypes.Count; i++ )
				{
					if( methodParameterTypes[ i ].IsAssignableFrom( paramTypes[ i ] ) )
						continue;
				}
        		return method;
        	}
        	return null; // methods.FirstOrDefault( m => m.Parameters().Select( p => p.ParameterType ).SequenceEqual( paramTypes ) );
        }
        #endregion

        #region Multiple Methods
		#region MethodsDeclared
        /// <summary>
        /// Find all public and non-public instance methods declared on the given <paramref name="type"/>.
        /// </summary>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> MethodsDeclared( this Type type )
        {
            return type.MethodsDeclared( Flags.InstanceCriteria, null );
        }

        /// <summary>
        /// Find all methods declared on the given <paramref name="type"/> that match the specified <paramref name="flags"/>. 
        /// </summary>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> MethodsDeclared( this Type type, BindingFlags flags )
        {
            return type.MethodsDeclared( flags, null );
        }

        /// <summary>
        /// Find all public and non-public instance methods declared on the given <paramref name="type"/> with the
        /// given <paramref name="name"/>. If <c>null</c> is passed in the <paramref name="name"/>
        /// parameter then no name filtering will be applied.
        /// </summary>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> MethodsDeclared( this Type type, string name )
        {
            return type.MethodsDeclared( Flags.InstanceCriteria, name );
        }

        /// <summary>
        /// Find all methods declared on the given <paramref name="type"/> with the given <paramref name="name"/> that 
        /// match the specified <paramref name="flags"/>. If <c>null</c> is passed in the <paramref name="name"/>
        /// parameter then no name filtering will be applied.
        /// </summary>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> MethodsDeclared( this Type type, BindingFlags flags, string name )
        {
            return type.GetMethods( flags ).Where( m => name == null || 
												   m.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) || 
												   m.Name.EndsWith( "."+name, StringComparison.OrdinalIgnoreCase ) ).ToList();
        }
		#endregion

		#region Methods
       	/// <summary>
        /// Find all public and non-public instance methods on the given <paramref name="type"/>.
        /// </summary>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type )
        {
            return type.Methods( Flags.InstanceCriteria, null );
        }

        /// <summary>
        /// Find all methods on the given <paramref name="type"/> that match the specified <paramref name="flags"/>. 
        /// </summary>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, BindingFlags flags )
        {
            return type.Methods( flags, null );
        }

        /// <summary>
        /// Find all public and non-public instance methods on the given <paramref name="type"/> with the
        /// given <paramref name="name"/>. If <c>null</c> is passed in the <paramref name="name"/>
        /// parameter then no name filtering will be applied.
        /// </summary>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, string name )
        {
            return type.Methods( Flags.InstanceCriteria, name );
        }

        /// <summary>
        /// Find all methods on the given <paramref name="type"/> with the given <paramref name="name"/> that 
        /// match the specified <paramref name="flags"/>. If <c>null</c> is passed in the <paramref name="name"/>
        /// parameter then no name filtering will be applied.
        /// </summary>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods( this Type type, BindingFlags flags, string name )
        {
            // as we recurse below, reset flags to only include declared fields (avoid duplicates in result)
            flags |= BindingFlags.DeclaredOnly;
            flags -= BindingFlags.FlattenHierarchy;
            var methods = new List<MethodInfo>( type.MethodsDeclared( flags, name ) );
            Type baseType = type.BaseType;
            while( baseType != null && baseType != typeof(object) )
            {
                methods.AddRange( baseType.MethodsDeclared( flags, name ) );
                baseType = baseType.BaseType;
            }
            return methods;
        }
        #endregion
        #endregion
		#endregion

		#region Method Parameter Lookup
		/// <summary>
        /// Finds all parameters for the given <paramref name="method"/>.
        /// </summary>
        /// <returns>The list of parameters for the method. This value will never be null.</returns>
        public static IList<ParameterInfo> Parameters( this MethodBase method )
        {
            return method.GetParameters();
        }

        /// <summary>
        /// Determines whether null can be assigned to the given <paramref name="parameter"/>.
        /// </summary>
        /// <returns>True if null can be assigned, false otherwise.</returns>
        public static bool IsNullable( this ParameterInfo parameter )
        {
            return ! parameter.ParameterType.IsValueType || parameter.ParameterType.IsSubclassOf( typeof(Nullable) );
        }

        /// <summary>
        /// Determines whether the given <paramref name="parameter"/> has the given <paramref name="name"/>.
        /// The comparison uses OrdinalIgnoreCase and allows for a leading underscore in either name
        /// to be ignored.
        /// </summary>
        /// <returns>True if the name is considered identical, false otherwise.</returns>
        public static bool HasName( this ParameterInfo parameter, string name )
        {
            string parameterName = parameter.Name.StartsWith( "_" ) ? parameter.Name.Substring( 1 ) : parameter.Name;
            name = name.StartsWith( "_" ) ? name.Substring( 1 ) : name;
            return parameterName.Equals( name, StringComparison.OrdinalIgnoreCase );
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
        /// <returns>The default value if one could be obtained and converted into the type of the parameter,
        /// and null otherwise.</returns>
        public static object DefaultValue( this ParameterInfo parameter )
        {
            var defaultValue = parameter.Attribute<DefaultValueAttribute>();
            return defaultValue != null
                       ? ObjectConstruction.TypeConverter.Get( parameter.ParameterType, defaultValue.Value )
                       : null;
        }
        #endregion
    }
}