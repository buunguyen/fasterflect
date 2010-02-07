using System;
using FasterflectTest.SampleModel.Attributes;
using FasterflectTest.SampleModel.Enumerations;

namespace FasterflectTest.SampleModel
{
	[Zone(Zone.Savannah)]
	internal class Giraffe : Mammal
	{
		public Giraffe( int id, Climate climateRequirements, MovementCapabilities movementCapabilities ) : base( id, climateRequirements, movementCapabilities )
		{
		}

		public Giraffe( Climate climateRequirements, MovementCapabilities movementCapabilities ) : base( climateRequirements, movementCapabilities )
		{
		}
	}
}
