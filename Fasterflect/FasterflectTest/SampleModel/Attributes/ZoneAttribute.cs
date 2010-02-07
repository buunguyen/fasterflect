using System;
using FasterflectTest.SampleModel.Enumerations;

namespace FasterflectTest.SampleModel.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class ZoneAttribute : Attribute 
	{
		public Zone Zone { get; set; }

		public ZoneAttribute( Zone zone )
		{
			Zone = zone;
		}
	}
}
