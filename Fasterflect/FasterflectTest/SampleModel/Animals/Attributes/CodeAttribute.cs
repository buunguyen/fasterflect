using System;

namespace FasterflectTest.SampleModel.Attributes
{
	[AttributeUsage(AttributeTargets.All)]
	internal class CodeAttribute : Attribute 
	{
		public string Code { get; set; }

		public CodeAttribute( string code )
		{
			Code = code;
		}
	}
}
