#region License

// Copyright 2010 Buu Nguyen, Morten Mertner
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/

#endregion

using System;
using System.Linq;
using Fasterflect.Probing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FasterflectTest.SampleModel.Animals.Enumerations;

namespace FasterflectTest.Probing
{
	[TestClass]
	public class TypeConverterTest
	{
		#region Enum Conversions
		[TestMethod]
		public void TestConvertFromEnum()
		{
			Assert.AreEqual( 2, TypeConverter.Get( typeof(int), Climate.Cold ) );
			Assert.AreEqual( 2f, TypeConverter.Get( typeof(float), Climate.Cold ) );
			Assert.AreEqual( 2d, TypeConverter.Get( typeof(double), Climate.Cold ) );
			Assert.AreEqual( "Cold", TypeConverter.Get( typeof(string), Climate.Cold ) );
		}

		[TestMethod]
		public void TestConvertToEnum()
		{
			Assert.AreEqual( Climate.Cold, TypeConverter.Get( typeof(Climate), 2 ) );
			Assert.AreEqual( Climate.Cold, TypeConverter.Get( typeof(Climate), "2" ) );
			Assert.AreEqual( Climate.Cold, TypeConverter.Get( typeof(Climate), "Cold" ) );
			Assert.AreEqual( Climate.Cold, TypeConverter.Get( typeof(Climate), (object) "Cold" ) );
		}
		#endregion

		#region Guid Conversions
		[TestMethod]
		public void TestConvertWithEmptyGuid()
		{
			string emptyGuidString = string.Empty.PadRight( 16, '\0' );
			string textualGuid = string.Empty.PadRight( 32, '0' );
			var emptyGuidBuffer = new byte[16];
			Assert.AreEqual( Guid.Empty, TypeConverter.Get( typeof(Guid), (object) emptyGuidString ) );
			Assert.AreEqual( Guid.Empty, TypeConverter.Get( typeof(Guid), emptyGuidBuffer ) );
			Assert.AreEqual( Guid.Empty, TypeConverter.Get( typeof(Guid), textualGuid ) );
		}

		[TestMethod]
		public void TestConvertWithNonEmptyGuid()
		{
			Guid guid = Guid.NewGuid();
			string binaryStringGuid = TypeConverter.GuidToBinaryString( guid );
			string readableStringGuid = guid.ToString();
			byte[] binaryGuid = guid.ToByteArray();
			// test direct to guid
			Assert.AreEqual( guid, TypeConverter.Get( typeof(Guid), binaryStringGuid ) );
			Assert.AreEqual( guid, TypeConverter.Get( typeof(Guid), readableStringGuid ) );
			Assert.AreEqual( guid, TypeConverter.Get( typeof(Guid), binaryGuid ) );
			// test direct from guid
			Assert.AreEqual( binaryStringGuid, TypeConverter.Get( typeof(string), guid ) );
			Assert.IsTrue( binaryGuid.SequenceEqual( (byte[]) TypeConverter.Get( typeof(byte[]), guid ) ) );
		}
		#endregion
	}
}