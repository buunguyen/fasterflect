using System;
using FasterflectTest.SampleModel.Enumerations;

namespace FasterflectTest.SampleModel
{
	internal abstract class Mammal : Animal
	{
		protected Mammal( Climate climateRequirements, MovementCapabilities movementCapabilities ) : base( climateRequirements, movementCapabilities )
		{
		}

		protected Mammal( int id, Climate climateRequirements, MovementCapabilities movementCapabilities ) : base( id, climateRequirements, movementCapabilities )
		{
		}
	}
}
