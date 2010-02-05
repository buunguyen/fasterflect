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
using System.Reflection;

namespace Fasterflect.ObjectConstruction
{
	/// <summary>
	/// Helper class for producing invocation maps that describe how to create an instance of an object
	/// given a set of parameters. Maps are cached to speed up subsequent requests.
	/// </summary>
	internal static class MapFactory
	{
		private static readonly MapCacheSlow cache = new MapCacheSlow();

		#region Map Construction
		public static MethodMap PrepareInvoke(this Type type, string[] paramNames, Type[] paramTypes)
		{
			int hash = MethodMap.GetParameterHash(paramNames, paramTypes);
			MethodMap map = cache.GetMap(type, hash);
			if (map == null)
			{
				map = DetermineBestConstructorMatch(type, paramNames, paramTypes);
				cache.AddMap(type, hash, map);
			}
			return map;
		}
		#endregion

		#region Map Construction Helpers
		private static MethodMap DetermineBestConstructorMatch(Type type, string[] paramNames, Type[] paramTypes)
		{
			MethodMap bestMap = null;
			foreach (ConstructorInfo ci in type.Constructors())
			{
				MethodMap map = CreateMap(ci, paramNames, paramTypes, true);
				if (map != null && map.IsValid)
				{
					bool isBetter = bestMap == null;
					isBetter |= map.IsPerfectMatch;
					isBetter |= bestMap != null && map.Cost < bestMap.Cost;
					isBetter &= map.IsValid;
					if (isBetter)
					{
						bestMap = map;
					}
				}
			}
			if (bestMap != null)
			{
				bestMap.InitializeInvoker();
				// TODO warn user that code has non-optimal performance
				//if( best.Cost > 0 )
				//	Check.LogInfo( "Best available constructor for type {0} is non-optimal (cost {1}).", typeMap.Type, best.Cost );
				return bestMap;
			}
			// TODO return improved error message when no valid constructor could be found
			//StringBuilder sb = new StringBuilder();
			//sb.AppendFormat( "No constructor found for type {0} using parameters:{1}",
			//                 type.Name, Environment.NewLine );
			//sb.AppendFormat( "{0}{1}", TypeFormatter.Format( parameters, "=", ", ", 3 ), Environment.NewLine );
			throw new MissingMethodException(type.Name, ".ctor");
		}

		private static MethodMap CreateMap(MethodBase method, string[] paramNames, Type[] paramTypes,
		                                   bool allowUnusedParameters)
		{
			if (method.IsConstructor)
				return new ConstructorMap(method as ConstructorInfo, paramNames, paramTypes, allowUnusedParameters);
			return new MethodMap(method, paramNames, paramTypes, allowUnusedParameters);
		}
		#endregion

		#region SourceInfo Lookups

		internal static SourceInfo GetSourceInfo(Type sourceType)
		{
			return cache.GetSourceInfo(sourceType);
		}

		internal static void AddSourceInfo(Type sourceType, SourceInfo sourceInfo)
		{
			cache.AddSourceInfo(sourceType, sourceInfo);
		}

		#endregion
	}
}