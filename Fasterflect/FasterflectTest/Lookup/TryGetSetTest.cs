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

using Fasterflect;
using FasterflectTest.SampleModel.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Lookup
{
    [TestClass]
    public class TryGetSetTest 
	{
        [TestMethod]
		public void TestTryGetSetField()
        {
			Lion lion = new Lion( 42, "Scar" );
			// tryget
			Assert.IsNull( lion.TryGetFieldValue( "name" ) );
			Assert.IsNull( lion.TryGetFieldValue( "ID" ) );
			Assert.AreEqual( 42, lion.TryGetFieldValue( "id" ) );
			Assert.AreEqual( 42, lion.TryGetFieldValue( "ID", Flags.InstanceCriteria | Flags.IgnoreCase ) );
        	// tryset
			Assert.IsFalse( lion.TrySetFieldValue( "missing", false ) );
			Assert.IsTrue( lion.TrySetFieldValue( "id", 43 ) );
			Assert.AreEqual( 43, lion.ID );
        }

        [TestMethod]
		public void TestTryGetSetProperty()
        {
			Lion lion = new Lion( 42, "Scar" );
			// tryget
			Assert.IsNull( lion.TryGetPropertyValue( "missing" ) );
			Assert.AreEqual( 42, lion.TryGetPropertyValue( "ID" ) );
			Assert.AreEqual( "Scar", lion.TryGetPropertyValue( "Name" ) );
        	// tryset
			Assert.IsFalse( lion.TrySetPropertyValue( "missing", false ) );
			Assert.IsTrue( lion.TrySetPropertyValue( "Name", "Simba" ) );
			Assert.AreEqual( "Simba", lion.Name );
        }

        [TestMethod]
		public void TestTryGetSetMember()
        {
			Lion lion = new Lion( 42, "Scar" );
			// tryget
			Assert.IsNull( lion.TryGetValue( "missing" ) );
			Assert.AreEqual( 42, lion.TryGetValue( "id" ) );
			Assert.AreEqual( "Scar", lion.TryGetValue( "Name" ) );
        	// tryset
			Assert.IsFalse( lion.TrySetValue( "missing", false ) );
			Assert.IsTrue( lion.TrySetValue( "id", 43 ) );
			Assert.AreEqual( 43, lion.ID );
			Assert.IsTrue( lion.TrySetValue( "ID", 44, Flags.InstanceCriteria | Flags.IgnoreCase ) );
			Assert.IsTrue( lion.TrySetValue( "Name", "Simba" ) );
			Assert.AreEqual( 44, lion.ID );
			Assert.AreEqual( "Simba", lion.Name );
        }
 	}
}
