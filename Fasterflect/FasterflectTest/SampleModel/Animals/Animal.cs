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
using System.Diagnostics;
using FasterflectTest.SampleModel.Attributes;
using FasterflectTest.SampleModel.Enumerations;

namespace FasterflectTest.SampleModel.Animals
{
	[DebuggerDisplay("ID={id}, Type={GetType()}")]
	internal abstract class Animal
	{
		private static int nextId = 1;
		[Code("ID")]
		private readonly int id;
		[DefaultValue(null)]
		private DateTime? birthDay;

		public int ID { get { return id; } }
		public DateTime? BirthDay { get { return birthDay; } set { birthDay = value; } }
		public Climate ClimateRequirements { get; private set; }
		[Code("Movement")]
		public MovementCapabilities MovementCapabilities { get; private set; }

		public static int LastID { get { return nextId-1; } }

		protected Animal( Climate climateRequirements, MovementCapabilities movementCapabilities )
		{
			id = nextId++;
			ClimateRequirements = climateRequirements;
			MovementCapabilities = movementCapabilities;
		}

		protected Animal( int id, Climate climateRequirements, MovementCapabilities movementCapabilities )
		{
			this.id = id;
			ClimateRequirements = climateRequirements;
			MovementCapabilities = movementCapabilities;
		}
	}
}
