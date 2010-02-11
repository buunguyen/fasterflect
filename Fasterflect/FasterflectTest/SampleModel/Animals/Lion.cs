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
using System.ComponentModel;
using System.Reflection;
using Fasterflect;
using FasterflectTest.SampleModel.Attributes;
using FasterflectTest.SampleModel.Enumerations;

namespace FasterflectTest.SampleModel.Animals
{
	[Zone(Zone.Savannah)]
	[Serializable]
	internal class Lion : Mammal
	{
		#pragma warning disable 0169, 0649
		[Code("Field")]
		private DateTime lastMealTime = DateTime.MinValue;

		[Code("ReadWrite Property")]
		[DefaultValue("Simba")]
		public string Name { get; private set; }

		[Code("ReadOnly Property")]
		public bool IsHungry { get { return DateTime.Now.AddHours( -12 ) > lastMealTime; } }

		public int ConstructorInstanceUsed { get; private set; }
		#pragma warning restore 0169, 0649

		#region Constructors
		public Lion() : this( typeof(Lion).Property( "Name" ).Attribute<DefaultValueAttribute>().Value.ToString() )
		{
			ConstructorInstanceUsed = 1;
		}

		public Lion( string name ) : base( Climate.Hot, MovementCapabilities.Land )
		{
			Name = name;
			ConstructorInstanceUsed = 2;
		}

		public Lion( int id ) : this( id, typeof(Lion).Property( "Name" ).Attribute<DefaultValueAttribute>().Value.ToString() )
		{
			ConstructorInstanceUsed = 3;
		}

		public Lion( int id, string name ) : base( id, Climate.Hot, MovementCapabilities.Land )
		{
			Name = name;
			ConstructorInstanceUsed = 4;
		}
		#endregion
	}
}
