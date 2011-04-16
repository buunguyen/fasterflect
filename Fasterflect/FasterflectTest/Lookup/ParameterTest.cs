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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FasterflectTest.SampleModel.Animals;

namespace FasterflectTest.Lookup
{
    [TestClass]
    public class ParameterTest
	{
		[TestMethod]
		public void TestFindParameterDefaultValue()
        {
			var method = typeof(Snake).Method( "Move" );
			Assert.IsNotNull( method );
			var parameters = method.Parameters();
			Assert.IsNotNull( parameters );
			Assert.AreEqual( 1, parameters.Count );
			var parameter = parameters[ 0 ];
			Assert.IsTrue( parameter.HasDefaultValue() );
			Assert.AreEqual( 100d, parameter.DefaultValue() );
		}

		[TestMethod]
		public void TestParameterHasName()
        {
			// Zoo.RegisterClass( string @class, string _section, string __name, int size )
			var method = typeof(Zoo).Method( "RegisterClass" );
			Assert.IsNotNull( method );
			var parameters = method.Parameters();
			Assert.IsNotNull( parameters );
			Assert.AreEqual( 4, parameters.Count );
			Assert.IsTrue( parameters[ 0 ].HasName( "class" ) );
			Assert.IsTrue( parameters[ 0 ].HasName( "_class" ) );
			Assert.IsTrue( parameters[ 1 ].HasName( "section" ) );
			Assert.IsTrue( parameters[ 1 ].HasName( "_section" ) );
			Assert.IsFalse( parameters[ 1 ].HasName( "__section" ) );
			Assert.IsFalse( parameters[ 2 ].HasName( "name" ) );
			Assert.IsFalse( parameters[ 2 ].HasName( "_name" ) );
			Assert.IsTrue( parameters[ 2 ].HasName( "__name" ) );
		}

		[TestMethod]
		public void TestParameterIsNullable()
        {
			var method = typeof(Snake).Method( "Move" );
			Assert.IsNotNull( method );
			var parameters = method.Parameters();
			Assert.IsNotNull( parameters );
			Assert.AreEqual( 1, parameters.Count );
			Assert.IsFalse( parameters[ 0 ].IsNullable() );

			method = typeof(Snake).Method( "Bite" );
			Assert.IsNotNull( method );
			parameters = method.Parameters();
			Assert.IsNotNull( parameters );
			Assert.AreEqual( 1, parameters.Count );
			Assert.IsTrue( parameters[ 0 ].IsNullable() );
		}
	}
}
