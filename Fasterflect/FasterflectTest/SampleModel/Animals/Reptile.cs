using System;
using FasterflectTest.SampleModel.Enumerations;

namespace FasterflectTest.SampleModel
{
	internal abstract class Reptile : Animal
	{
		protected Reptile( Climate climateRequirements, MovementCapabilities movementCapabilities ) : base( climateRequirements, movementCapabilities )
		{
		}

		protected Reptile( int id, Climate climateRequirements, MovementCapabilities movementCapabilities ) : base( id, climateRequirements, movementCapabilities )
		{
		}
	}
}
