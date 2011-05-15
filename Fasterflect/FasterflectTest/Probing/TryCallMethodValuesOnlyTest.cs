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
using System.Reflection;
using Fasterflect;
using FasterflectTest.SampleModel.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Probing
{
	[TestClass]
	public class TryCallMethodValuesOnlyTest
	{
		// Elephant method overload summary
		// 1: public void Eat()
		// 2: public void Eat( string food )
		// 3: public void Eat( int count )
		// 4: public void Eat( int count, string food )
		// 5: public void Eat( double count, string food, bool isHay )

		[TestMethod]
		public void TestTryCallWithValuesOnlyAndFixedParameterOrdering()
		{
            var obj = new Elephant();
            obj.TryCallMethodWithValues("Eat",  1, "foo");
			Assert.AreEqual( 4, obj.MethodInvoked );
            obj.TryCallMethodWithValues("Eat",  1.0, "foo", false);
            Assert.AreEqual(5, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat",  "foo");
            Assert.AreEqual(2, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat",  'f');
            Assert.AreEqual(2, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat",  null); // this invokes 1 and not 2 because null implies the params array parameter is null == no arguments
			Assert.AreEqual( 1, obj.MethodInvoked );
            obj.TryCallMethodWithValues("Eat",  1, null);
			Assert.AreEqual( 4, obj.MethodInvoked );
            obj.TryCallMethodWithValues("Eat",  1, null, false);
            Assert.AreEqual(5, obj.MethodInvoked);
            Assert.AreEqual(5, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Accept",  'c');
            Assert.AreEqual(12, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Accept",  "a");
            Assert.AreEqual(12, obj.MethodInvoked);
            obj.TryCallMethodWithValues("AcceptParams");
            Assert.AreEqual(13, obj.MethodInvoked);
            obj.TryCallMethodWithValues("AcceptParams",  null);
            Assert.AreEqual(13, obj.MethodInvoked);
            obj.TryCallMethodWithValues("AcceptParams",  1);
            Assert.AreEqual(13, obj.MethodInvoked);
            obj.TryCallMethodWithValues("AcceptParams",  1, "str");
            Assert.AreEqual(13, obj.MethodInvoked);
		}

        [TestMethod]
        public void TestTryCallWithValuesOnlyAndFixedParameterOrderingOnString()
        {
            Assert.AreEqual("abc", "abc ".TryCallMethodWithValues("TrimEnd"));
            Assert.AreEqual("abc", "abc ".TryCallMethodWithValues("TrimEnd", null));
            Assert.AreEqual("ab", "abc ".TryCallMethodWithValues("TrimEnd", ' ', 'c'));
        }
	}
}