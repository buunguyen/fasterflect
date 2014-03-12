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

namespace Fasterflect.Probing
{
	/// <summary>
	/// Collects methods into a pool and allows invocation of the best match given a set of parameters.
	/// </summary>
	public class MethodDispatcher
	{
		/// <summary>
		/// The list of candidate methods for invocations through this dispatcher instance.
		/// </summary>
		private readonly List<MethodBase> methodPool = new List<MethodBase>();

		/// <summary>
		/// This field is used to cache the best match for a given parameter set (as represented by the SourceInfo class).
		/// </summary>
		private readonly Cache<SourceInfo, MethodMap> mapCache = new Cache<SourceInfo, MethodMap>();

		#region Constructors
		/// <summary>
		/// Use this constructor in conjuction with the AddMethod to manually populate the pool of available methods.
		/// </summary>
		public MethodDispatcher()
		{
		}

		/// <summary>
		/// Use this constructor to automatically populate the pool of available methods with all instance methods 
		/// found on the given type.
		/// </summary>
		public MethodDispatcher( Type type )
		{
			type.Methods().ForEach( methodPool.Add );
		}
		#endregion

		/// <summary>
		/// Add a method to the list of available methods for this method dispatcher.
		/// </summary>
		/// <param name="method">The method to add to the pool of invocation candidates.</param>
		public void AddMethod( MethodInfo method )
		{
			//if( method.IsStatic )
			//{
			//	throw new ArgumentException( "Method dispatching currently only supports instance methods.", method.Name );
			//}
			if( method.IsAbstract )
			{
				throw new ArgumentException( "Method dispatching does not support abstract methods.", method.Name );
			}
			methodPool.Add( method );
		}
			
		/// <summary>
		/// Invoke the best available match for the supplied parameters. If no method can be called 
		/// using the supplied parameters, an exception is thrown.
		/// </summary>
		/// <param name="obj">The object on which to invoke a method.</param>
		/// <param name="mustUseAllParameters">Specifies whether all supplied parameters must be used in the
		/// invocation. Unless you know what you are doing you should pass true for this parameter.</param>
		/// <param name="sample">The object whose public properties will be used as parameters.</param>
		/// <returns>The return value of the invocation.</returns>
		public object Invoke( object obj, bool mustUseAllParameters, object sample )
		{
			if( obj == null || sample == null )
			{
				throw new ArgumentException( "Missing or invalid argument: " + (obj == null ? "obj" : "sample") );
			}
			var sourceInfo = SourceInfo.CreateFromType( sample.GetType() );

			// check to see if we already have a map for best match
			MethodMap map = mapCache.Get( sourceInfo );
			object[] values = sourceInfo.GetParameterValues( sample );
			if( map == null )
			{
				string[] names = sourceInfo.ParamNames;
				Type[] types = sourceInfo.ParamTypes;
				if( names.Length != values.Length || names.Length != types.Length )
				{
					throw new ArgumentException( "Mismatching name, type and value arrays (must be of identical length)." );
				}
				map = MapFactory.DetermineBestMethodMatch( methodPool, mustUseAllParameters, names, types, values );
				mapCache.Insert( sourceInfo, map );
			}
			bool isStatic = obj is Type;
			return isStatic ? map.Invoke( values ) : map.Invoke( obj, values );
		}

		/// <summary>
		/// Invoke the best available match for the supplied parameters. 
		/// If no method can be called using the supplied parameters, an exception is thrown.
		/// </summary>
		/// <param name="obj">The object on which to invoke a method.</param>
		/// <param name="mustUseAllParameters">Specifies whether all supplied parameters must be used in the
		/// invocation. Unless you know what you are doing you should pass true for this parameter.</param>
		/// <param name="parameters">A dictionary of parameter name/value pairs.</param>
		/// <returns>The return value of the invocation.</returns>
		public object Invoke( object obj, bool mustUseAllParameters, Dictionary<string, object> parameters )
		{
			if( obj == null || parameters == null )
			{
				throw new ArgumentException( "Missing or invalid argument: " + (obj == null ? "obj" : "parameters") );
			}
			string[] names = parameters.Keys.ToArray() ?? new string[0];
			object[] values = parameters.Values.ToArray() ?? new object[0];
			Type[] types = values.ToTypeArray() ?? new Type[0];
			bool isStatic = obj is Type;
			var type = isStatic ? obj as Type : obj.GetType();
			var sourceInfo = new SourceInfo( type, names, types );
			// check to see if we already have a map for best match
			MethodMap map = mapCache.Get( sourceInfo );
			if( map == null )
			{
				map = MapFactory.DetermineBestMethodMatch( methodPool, mustUseAllParameters, names, types, values );
				mapCache.Insert( sourceInfo, map );
			}
			return isStatic ? map.Invoke( values ) : map.Invoke( obj, values );
		}
	}
}