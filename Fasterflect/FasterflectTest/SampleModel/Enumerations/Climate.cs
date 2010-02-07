using System;
using FasterflectTest.SampleModel.Attributes;

namespace FasterflectTest.SampleModel.Enumerations
{
	[Flags]
	[Code("Temperature")]
	internal enum Climate
	{
		[Code("Hot")]
		Hot = 1,
		[Code("Cold")]
		Cold = 2,
		[Code("Any")]
		Any = Hot | Cold
	}
}
