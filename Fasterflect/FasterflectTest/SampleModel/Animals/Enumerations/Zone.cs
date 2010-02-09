using System;

namespace FasterflectTest.SampleModel.Enumerations
{
	[Flags]
	internal enum Zone
	{
		Arctic = 1,
		Ocean = 1 << 1,
		Savannah = 1 << 2,
		Jungle = 1 << 3,
		Plains = 1 << 4, 
		Woods = 1 << 5,
	}
}
