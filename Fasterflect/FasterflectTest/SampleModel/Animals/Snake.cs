using System;
using FasterflectTest.SampleModel.Attributes;
using FasterflectTest.SampleModel.Enumerations;

namespace FasterflectTest.SampleModel
{
	internal class Snake : Reptile
	{
		public Snake() : base( Climate.Hot, MovementCapabilities.Land )
		{
		}
	}
}
