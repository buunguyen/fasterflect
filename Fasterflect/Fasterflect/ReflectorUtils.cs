#region License

// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
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
using Fasterflect.Emitter;

namespace Fasterflect
{
	internal static class ReflectorUtils
	{
		public static Type GetTypeAdjusted(this object obj)
		{
			var wrapper = obj as ValueTypeHolder;
			return wrapper == null
			       	? obj.GetType()
			       	: wrapper.Value.GetType();
		}

		public static Type[] GetTypeArray(this object[] objects)
		{
			/*
             * Readable code with a LINQ query, but it's pretty slow
             * return objects.Select(o => o.GetType()).ToArray();
             */
			var types = new Type[objects.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = objects[i].GetType();
			}
			return types;
		}
	}
}