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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fasterflect;
using Fasterflect.Probing;
using FasterflectTest.SampleModel.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Probing
{
	[TestClass]
	public class TryCallMethodTest
	{
		[TestMethod]
		public void TestTryCallWithEmptyArgumentShouldInvokeMethod1()
		{
			var obj = new Elephant();
			obj.TryCallMethod( "Eat", true, new { } );
			Assert.AreEqual( 1, obj.MethodInvoked );
			// check that we also work when passing in unused parameters
			obj.TryCallMethod( "Eat", false, new { size=1 } );
			Assert.AreEqual( 1, obj.MethodInvoked );
		}

		[TestMethod]
		public void TestTryCallWithFoodArgumentShouldInvokeMethod2()
		{
			var obj = new Elephant();
			obj.TryCallMethod( "Eat", true, new { food="hay" } );
			Assert.AreEqual( 2, obj.MethodInvoked );
		}

		[TestMethod]
		public void TestTryCallWithCountArgumentsShouldInvokeMethod3()
		{
			var obj = new Elephant();
			obj.TryCallMethod( "Eat", true, new { count=2 } );
			Assert.AreEqual( 3, obj.MethodInvoked );
		}

		[TestMethod]
		public void TestTryCallWithCountAndFoodArgumentsShouldInvokeMethod4()
		{
			var obj = new Elephant();
			obj.TryCallMethod( "Eat", true, new { count=2, food="hay" } );
			Assert.AreEqual( 4, obj.MethodInvoked );
		}

		[TestMethod]
		public void TestTryCallWithCountAndFoodAndIsHayArgumentsShouldInvokeMethod5()
		{
			var obj = new Elephant();
			obj.TryCallMethod( "Eat", true, new { count=2.0, food="hay", isHay=true } );
			Assert.AreEqual( 5, obj.MethodInvoked );
			// try with argument that must be type converted
			obj.TryCallMethod( "Eat", true, new { count=2, food="hay", isHay=true } );
			Assert.AreEqual( 5, obj.MethodInvoked );
		}

		[TestMethod]
		[ExpectedException(typeof(MissingMethodException))]
		public void TestTryCallWithNonMatchShouldThrow()
		{
			var obj = new Elephant();
			obj.TryCallMethod( "Eat", true, new { size=1 } );
		}
	}
}