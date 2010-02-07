using System;

namespace FasterflectTest.SampleModel.Enumerations
{
	[Flags]
	internal enum MovementCapabilities
	{
		Land = 1,
		Water = 2,
		Air = 4
	}
}
