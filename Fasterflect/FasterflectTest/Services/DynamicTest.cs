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
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FasterflectTest.SampleModel.People;

namespace FasterflectTest.Services
{
#if DOT_NET_4
    [TestClass]
    public class DynamicTest
	{
		[TestMethod]
		public void TestDynamicWrapper()
		{
			Person original = new Person( "Bruce Lee", 25 );
			dynamic wrapper = new DynamicWrapper( original );
			Assert.AreEqual( original.Name, wrapper.Name );
			Assert.AreEqual( original.Age, wrapper.Age );
			double distance;
			original.Walk( 10d, out distance );
			Assert.AreEqual( 10d, distance );
			wrapper.Walk( 10d, out distance );
			//Assert.AreEqual( 20d, distance );
			Assert.AreEqual( 20d, original.TryGetFieldValue( "metersTravelled" ) );
		}

		[TestMethod]
		public void TestDynamicBuilder()
		{
			dynamic obj = new DynamicBuilder();
			obj.Value = 1;
			obj.GetMessage = new Func<string>( () => "Value = " + obj.Value );
			// verify
			Assert.AreEqual( "Value = 1", obj.GetMessage() );
			// verify that we still work after updating member
			obj.Value = 5;
			Assert.AreEqual( "Value = 5", obj.GetMessage() );
		}
	}
#endif
}
