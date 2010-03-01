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
using System.Collections.ObjectModel;
using System.Linq;

namespace FasterflectTest.SampleModel.Animals
{
	internal sealed class Zoo : Collection<Animal>
	{
		private const int FirstId = 1000;
		private static int nextId = FirstId;
		private readonly string name;
		private string alias;

		public Zoo( string name )
		{
			this.name = name;
			alias = name;
		}

		public IEnumerable<T> Animals<T>()
		{
			return this.Where( a => a is T ).Cast<T>();
		}

		public void RegisterClass( string @class, string _section, string __name, int size )
		{
		}

		public string Name
		{
			get { return name; }
		}
		public string Alias
		{
			set { alias = value; }
		}

		public static int NextId
		{
			get { return ++nextId; }
		}
	}
}
