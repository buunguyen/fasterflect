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
using FasterflectTest.SampleModel.Animals.Enumerations;
using FasterflectTest.SampleModel.Animals.Interfaces;

namespace FasterflectTest.SampleModel.Animals
{
	internal class Snake : Reptile, ISwim, IBite
	{
		public Snake() : base( Climate.Hot, MovementCapabilities.Land )
		{
			HasDeadlyBite = true;
		}

		// regular member
		public bool HasDeadlyBite { get; private set; } 

		// ISwim
		void ISwim.Move( double distance )
		{
			SwimDistance += distance;
		}
		public virtual double SwimDistance { get; private set; } 

		// ISlide
		public override void Move( [DefaultValue(100d)] double distance )
		{
			SlideDistance += distance;
		}
		public override double SlideDistance { get; protected set; } 

		// IBite
		public bool Bite( Animal animal )
		{
			return HasDeadlyBite;
		}
	}
}
