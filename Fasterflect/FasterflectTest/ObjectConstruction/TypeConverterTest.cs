#region License
// Copyright 2010 Morten Mertner, Buu Nguyen (http://www.buunguyen.net/blog)
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
using Fasterflect.ObjectConstruction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.ObjectConstruction
{
	[TestClass]
	public class TypeConverterTest
	{
		#region Sample Reflection Classes
		private enum TestEnum
		{
			ElementOne = 5,
		}
		#endregion

		#region Enum Conversions
		[TestMethod]
		public void TestConvertFromEnum()
		{
			Assert.AreEqual( 5, TypeConverter.Get( typeof(int), TestEnum.ElementOne ) );
			Assert.AreEqual( 5f, TypeConverter.Get( typeof(float), TestEnum.ElementOne ) );
			Assert.AreEqual( 5d, TypeConverter.Get( typeof(double), TestEnum.ElementOne ) );
			Assert.AreEqual( "ElementOne", TypeConverter.Get( typeof(string), TestEnum.ElementOne ) );
		}

		[TestMethod]
		public void TestConvertToEnum()
		{
			Assert.AreEqual( TestEnum.ElementOne, TypeConverter.Get( typeof(TestEnum), 5 ) );
			Assert.AreEqual( TestEnum.ElementOne, TypeConverter.Get( typeof(TestEnum), "5" ) );
			Assert.AreEqual( TestEnum.ElementOne, TypeConverter.Get( typeof(TestEnum), "ElementOne" ) );
			Assert.AreEqual( TestEnum.ElementOne, TypeConverter.Get( typeof(TestEnum), (object) "ElementOne" ) );
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