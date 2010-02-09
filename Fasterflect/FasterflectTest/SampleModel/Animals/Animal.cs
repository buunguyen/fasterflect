using System;
using System.ComponentModel;
using System.Diagnostics;
using FasterflectTest.SampleModel.Attributes;
using FasterflectTest.SampleModel.Enumerations;

namespace FasterflectTest.SampleModel
{
	[DebuggerDisplay("ID={id}, Type={GetType()}")]
	internal abstract class Animal
	{
		private static int nextId = 1;
		[Code("ID")]
		private readonly int id;
		[DefaultValue(null)]
		private DateTime? birthDay;

		public int ID { get { return id; } }
		public DateTime? Birthday { get { return birthDay; } set { birthDay = value; } }
		public Climate ClimateRequirements { get; private set; }
		[Code("Movement")]
		public MovementCapabilities MovementCapabilities { get; private set; }

		public static int LastID { get { return nextId-1; } }

		protected Animal( Climate climateRequirements, MovementCapabilities movementCapabilities )
		{
			id = nextId++;
			ClimateRequirements = climateRequirements;
			MovementCapabilities = movementCapabilities;
		}

		protected Animal( int id, Climate climateRequirements, MovementCapabilities movementCapabilities )
		{
			this.id = id;
			ClimateRequirements = climateRequirements;
			MovementCapabilities = movementCapabilities;
		}
	}
}
