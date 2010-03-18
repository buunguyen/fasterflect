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
using System.Text;

namespace Fasterflect.Probing
{
	/// <summary>
	/// Collects methods into a pool and allows invocation of the best match given a set of parameters.
	/// </summary>
	public class MethodDispatcher
	{
		private readonly List<MethodInfo> methodPool = new List<MethodInfo>();

		/// <summary>
		/// Add a method to the list of available methods for this method dispatcher.
		/// </summary>
		/// <param name="method">The method to add to the pool of invocation candidates.</param>
		public void AddMethod( MethodInfo method )
		{
			if( method.IsStatic )
				throw new ArgumentException( "Method dispatching currently only supports instance methods.", method.Name );
			if( method.IsAbstract )
				throw new ArgumentException( "Method dispatching does not support abstract methods.", method.Name );
			methodPool.Add( method );
		}

		/// <summary>
		/// Invoke the best available match for the supplied parameters. 
		/// If no method can be called using the supplied parameters, an exception is thrown.
		/// </summary>
		/// <param name="obj">The object on which to invoke a method.</param>
		/// <param name="mustUseAllParameters">Specifies whether all supplied parameters must be used in the
		/// invocation. Unless you know what you are doing you should pass true for this parameter.</param>
		/// <param name="sample">The object whose public properties will be used as parameters.</param>
		/// <returns>The return value of the invocation.</returns>
		public object Invoke( object obj, bool mustUseAllParameters, object sample )
		{
            Type sourceType = sample.GetType();
            var sourceInfo = new SourceInfo( sourceType );
        	bool isStatic = obj is Type;
        	var names = sourceInfo.ParamNames;
			var types = sourceInfo.ParamTypes;
            var values = sourceInfo.GetParameterValues( sample );
			if( names.Length != values.Length || names.Length != types.Length )
			{
                throw new ArgumentException( "Mismatching name, type and value arrays (must be of identical length)." );
			}
            MethodMap map = MapFactory.DetermineBestMethodMatch( methodPool, mustUseAllParameters, names, types, values );
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
        	bool isStatic = obj is Type;
        	var names = parameters.Keys.ToArray() ?? new string[ 0 ];
			var values = parameters.Values.ToArray() ?? new object[ 0 ];
        	var types = values.ToTypeArray() ?? new Type[ 0 ];
			if( names.Length != values.Length || names.Length != types.Length )
			{
                throw new ArgumentException( "Mismatching name, type and value arrays (must be of identical length)." );
			}
            MethodMap map = MapFactory.DetermineBestMethodMatch( methodPool, mustUseAllParameters, names, types, values );
            return isStatic ? map.Invoke( values ) : map.Invoke( obj, values );
		}

		#region ToString Formatters
		/// <summary>
		/// Calling this method returns a string with all entries from the supplied Hashtable.
		/// </summary>
		private static string Format( Dictionary<string, object> source, string itemDelimiter, string pairDelimiter,
		                             int pairsPerLine )
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			foreach( string key in source.Keys )
			{
				sb.AppendFormat( "{0}{1}{2}", key, itemDelimiter, source[ key ] );
				if( ++count < source.Count )
					sb.Append( pairDelimiter );
				if( pairsPerLine > 0 && count % pairsPerLine == 0 )
					sb.Append( Environment.NewLine );
			}
			return sb.ToString();
		}

		/// <summary>
		/// Calling this method returns a string with all entries from the supplied Hashtable.
		/// </summary>
		private static string Format( Dictionary<string, object> source, string itemDelimiter, string pairDelimiter )
		{
			return Format( source, itemDelimiter, pairDelimiter, 0 );
		}
		#endregion
	}
}