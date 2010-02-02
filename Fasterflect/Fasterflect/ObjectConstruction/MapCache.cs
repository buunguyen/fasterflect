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

namespace Fasterflect.ObjectConstruction
{
	internal class MapCache
	{
		/// <summary>
		/// This field is used to cache information on objects used as parameters for object construction, which
		/// improves performance for subsequent instantiations of the same type using a compatible source type.
		/// </summary>
		private static readonly Dictionary<Type, SourceInfo> sources = new Dictionary<Type, SourceInfo>();

		/// <summary>
		/// This field contains a dictionary mapping from a particular constructor to all known parameter sets,
		/// each with an associated MethodMap responsible for creating instances of the type using the given
		/// constructor and parameter set.
		/// </summary>
		private static readonly Dictionary<Type, Dictionary<int, MethodMap>> maps =
			new Dictionary<Type, Dictionary<int, MethodMap>>();

		#region Map Cache Methods

		public MethodMap GetMap(Type type, int parameterHashCode)
		{
			lock (mapLock)
			{
				Dictionary<int, MethodMap> parameterMaps;
				if (maps.TryGetValue(type, out parameterMaps))
				{
					MethodMap map;
					return parameterMaps.TryGetValue(parameterHashCode, out map) ? map : null;
				}
			}
			return null;
		}

		public void AddMap(Type type, int parameterHashCode, MethodMap map)
		{
			lock (mapLock)
			{
				Dictionary<int, MethodMap> parameterMaps;
				if (maps.TryGetValue(type, out parameterMaps))
				{
					parameterMaps[parameterHashCode] = map;
				}
				else
				{
					maps[type] = new Dictionary<int, MethodMap> {{parameterHashCode, map}};
				}
			}
		}

		#endregion

		#region SourceInfo Cache Methods

		public SourceInfo GetSourceInfo(Type type)
		{
			lock (sourceLock)
			{
				SourceInfo result;
				return sources.TryGetValue(type, out result) ? result : null;
			}
		}

		public void AddSourceInfo(Type type, SourceInfo sourceInfo)
		{
			lock (sourceLock)
			{
				sources[type] = sourceInfo;
			}
		}

		#endregion

		private readonly object mapLock = new object();
		private readonly object sourceLock = new object();
	}
}