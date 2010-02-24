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

using System.Collections.Generic;
using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
    public static class MethodInfoExtensions
    {
        #region Access
        /// <summary>
        /// Invokes the static method identified by <paramref name="methodInfo"/> with no argument.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this MethodInfo methodInfo )
        {
            return methodInfo.DelegateForStaticInvoke()();
        }

        /// <summary>
        /// Invokes the static method identified by <paramref name="methodInfo"/> with <paramref name="parameters"/>
        /// as arguments.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this MethodInfo methodInfo, params object[] parameters )
        {
            return methodInfo.DelegateForStaticInvoke()( parameters );
        }

        /// <summary>
        /// Invokes the instance method identified by <paramref name="methodInfo"/> on the object
        /// <paramref name="target"/> with no argument.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this MethodInfo methodInfo, object target )
        {
            return methodInfo.DelegateForInvoke()( target );
        }

        /// <summary>
        /// Invokes the instance method identified by <paramref name="methodInfo"/> on the object
        /// <paramref name="target"/> with <paramref name="parameters"/> as arguments.
        /// </summary>
        /// <returns>The return value of the method.</returns>
        /// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
        public static object Invoke( this MethodInfo methodInfo, object target, params object[] parameters )
        {
            return methodInfo.DelegateForInvoke()( target, parameters );
        }

        /// <summary>
        /// Creates a delegate which can invoke the static method identified by <paramref name="methodInfo"/>.
        /// </summary>
        public static StaticMethodInvoker DelegateForStaticInvoke( this MethodInfo methodInfo )
        {
            return (StaticMethodInvoker) new MethodInvocationEmitter( methodInfo, Flags.StaticAnyVisibility ).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can invoke the instance method identified by <paramref name="methodInfo"/>.
        /// </summary>
        public static MethodInvoker DelegateForInvoke( this MethodInfo methodInfo )
        {
            return (MethodInvoker) new MethodInvocationEmitter( methodInfo, Flags.InstanceAnyVisibility ).GetDelegate();
        }
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
        #endregion
    }
}