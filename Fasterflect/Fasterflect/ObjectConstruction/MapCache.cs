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
using Fasterflect.Caching;

namespace Fasterflect.ObjectConstruction
{
	#region MapCache without CacheStore
	internal class MapCache
	{
		/// <summary>
		/// This field is used to cache information on objects used as parameters for object construction, which
		/// improves performance for subsequent instantiations of the same type using a compatible source type.
		/// </summary>
		private readonly Dictionary<Type, SourceInfo> sources = new Dictionary<Type, SourceInfo>();
		private readonly object sourceLock = new object();

		/// <summary>
		/// This field contains a dictionary mapping from a particular constructor to all known parameter sets,
		/// each with an associated MethodMap responsible for creating instances of the type using the given
		/// constructor and parameter set.
		/// </summary>
		private readonly Dictionary<long, MethodMap> maps = new Dictionary<long, MethodMap>();
		private readonly object mapLock = new object();

		#region Map Cache Methods
		public MethodMap GetMap(Type type, int parameterHashCode)
		{
			lock (mapLock)
			{
				long key = ((long) type.GetHashCode()) << 32 + parameterHashCode;
				MethodMap map;
				return maps.TryGetValue(key, out map) ? map : null;
			}
		}

		public void AddMap(Type type, int parameterHashCode, MethodMap map)
		{
			lock (mapLock)
			{
				long key = ((long) type.GetHashCode()) << 32 + parameterHashCode;
				maps[ key ] = map;
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
	}
	#endregion

	#region MapCache with CacheStore
	internal class MapCacheWithCacheStore
	{
		/// <summary>
		/// This field is used to cache information on objects used as parameters for object construction, which
		/// improves performance for subsequent instantiations of the same type using a compatible source type.
		/// </summary>
		private readonly CacheStore<Type, SourceInfo> sources = new CacheStore<Type, SourceInfo>( LockStrategy.Monitor );

		/// <summary>
		/// This field contains a dictionary mapping from a particular constructor to all known parameter sets,
		/// each with an associated MethodMap responsible for creating instances of the type using the given
		/// constructor and parameter set.
		/// </summary>
		private readonly CacheStore<long, MethodMap> maps = new CacheStore<long, MethodMap>( LockStrategy.Monitor );

		#region Map Cache Methods
		public MethodMap GetMap( Type type, int parameterHashCode )
		{
			long key = ((long) type.GetHashCode()) << 32 + parameterHashCode;
			return maps.Get( key );
		}

		public void AddMap( Type type, int parameterHashCode, MethodMap map )
		{
			long key = ((long) type.GetHashCode()) << 32 + parameterHashCode;
			maps.Insert( key, map, CacheStrategy.Temporary );
		}
		#endregion

		#region SourceInfo Cache Methods
		public SourceInfo GetSourceInfo( Type type )
		{
			return sources.Get( type );
		}

		public void AddSourceInfo( Type type, SourceInfo sourceInfo )
		{
			sources.Insert( type, sourceInfo, CacheStrategy.Temporary );
		}
		#endregion
	}
	#endregion
}