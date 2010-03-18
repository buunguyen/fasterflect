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
using FasterflectTest.SampleModel.Animals.Enumerations;

namespace FasterflectTest.SampleModel.Animals
{
	internal class Elephant : Mammal
	{
		#pragma warning disable 0169, 0649
		public int MethodInvoked { get; private set; }
		#pragma warning restore 0169, 0649

		#region Constructors
		public Elephant() : base( Climate.Hot, MovementCapabilities.Land )
		{
		}
		#endregion

		#region Methods
		public void Eat()
		{
			MethodInvoked = 1;
		}
		public void Eat( string food )
		{
			MethodInvoked = 2;
		}
		public void Eat( int count )
		{
			MethodInvoked = 3;
		}
		public void Eat( int count, string food )
		{
			MethodInvoked = 4;
		}
		public void Eat( double count, string food, bool isHay )
		{
			MethodInvoked = 5;
		}

		public void Roar( int count )
		{
			MethodInvoked = 10;
		}	
		public void Roar( int count, int volume )
		{
			MethodInvoked = 11;
		}	
		#endregion
	}
}
