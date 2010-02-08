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
using Fasterflect;
using Fasterflect.ObjectConstruction;
using FasterflectTest.SampleModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.ObjectConstruction
{
	[TestClass]
	public class TryCreateInstanceTest
	{
		#region TryCreateInstance Tests
		[TestMethod]
		public void TestTryCreateInstanceWithMatchingEmptyArgumentShouldInvokeConstructor1()
		{
			Lion animal = typeof(Lion).TryCreateInstance( new { } ) as Lion;
			Verify( animal, 1, Animal.LastID, "Simba", null );
		}

		[TestMethod]
		public void TestTryCreateInstanceWithMatchingSingleArgumentShouldInvokeConstructor2()
		{
			Lion animal = typeof(Lion).TryCreateInstance( new { Name = "Scar" } ) as Lion;
			Verify( animal, 2, Animal.LastID, "Scar", null );
		}

		[TestMethod]
		public void TestTryCreateInstanceWithMatchingSingleArgumentShouldInvokeConstructor3()
		{
			Lion animal = typeof(Lion).TryCreateInstance( new { Id = 42 } ) as Lion;
			Verify( animal, 3, 42, "Simba", null );
		}

		[TestMethod]
		public void TestTryCreateInstanceWithMatchingDoubleArgumentShouldInvokeConstructor4()
		{
			Lion animal = typeof(Lion).TryCreateInstance( new { Id = 42, Name = "Scar" } ) as Lion;
			Verify( animal, 4, 42, "Scar", null );
		}

		[TestMethod]
		public void TestTryCreateInstanceWithPartialMatchShouldInvokeConstructor3AndSetProperty()
		{
			DateTime? birthday = new DateTime( 1973, 1, 27 );
			Lion animal = typeof(Lion).TryCreateInstance( new { Id = 42, Birthday = birthday  } ) as Lion;
			Verify( animal, 3, 42, "Simba", birthday );
		}

		[TestMethod]
		public void TestTryCreateInstanceWithPartialMatchShouldInvokeConstructor4AndIgnoreExtraArgs()
		{
			DateTime? birthday = new DateTime( 1973, 1, 27 );
			Lion animal = typeof(Lion).TryCreateInstance( new { Id = 42, Name = "Scar", Birthday = birthday, Dummy = 0 } ) as Lion;
			Verify( animal, 4, 42, "Scar", birthday );
		}

		[TestMethod]
		public void TestTryCreateInstanceWithConvertibleArgumentTypeShouldUseConstructor3()
		{
			Lion animal = typeof(Lion).TryCreateInstance( new { Id = "2" } ) as Lion;
			Verify( animal, 3, 2, "Simba", null );
		}

		private static void Verify( Lion animal, int constructorInstanceUsed, int id, string name, DateTime? birthday )
		{
			Assert.IsNotNull( animal );
			Assert.AreEqual( constructorInstanceUsed, animal.ConstructorInstanceUsed );
			Assert.AreEqual( id, animal.ID );
			Assert.AreEqual( name, animal.Name );
			if( birthday.HasValue )
				Assert.AreEqual( birthday, animal.Birthday );
			else
				Assert.IsNull( animal.Birthday );
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestTryCreateInstanceWithInvalidArgumentTypeShouldThrow()
		{
			typeof(Lion).TryCreateInstance( new { Id = "Incompatible Argument Type" } );
		}

		[TestMethod]
		[ExpectedException(typeof(MissingMethodException))]
		public void TestTryCreateInstanceWithoutMatchShouldThrow()
		{
			typeof(Giraffe).TryCreateInstance( new { Id = 42 } );
		}
		#endregion

		#region Test Hash Code
		[TestMethod]
		public void TestParameterHashGenerator_SameTypeShouldGiveIdenticalHash()
		{
			object source1 = new { Id = 42 };
			SourceInfo sample1 = new SourceInfo( source1.GetType() );

			object source2 = new { Id = 5 };
			SourceInfo sample2 = new SourceInfo( source2.GetType() );

			Assert.AreEqual( sample1.HashCode, sample2.HashCode );
		}

		[TestMethod]
		public void TestParameterHashGenerator_DifferentTypeShouldGiveUniqueHash()
		{
			int[] hashes = GetSampleHashes(); 
			CollectionAssert.AllItemsAreUnique( hashes );
		}

		private static int[] GetSampleHashes()
		{
			var sources = new object[]
			              {
			              	new { }, new { Id = 42 }, new { Name = "Scar" }, new { Id = 42, Name = "Scar" },
			              	new { Id = 42, Birthday = DateTime.Now },
			              	new { Id = 42, Name = "Scar", Birthday = DateTime.Now, Dummy = 0 }
			              };
			int index = 0;
			var infos = new SourceInfo[ sources.Length ];
			Array.ForEach( sources, s => { infos[ index++ ] = new SourceInfo( s.GetType() ); } );
			index = 0;
			int[] hashes = new int[ sources.Length ];
			Array.ForEach( infos, i => { hashes[ index++ ] = i.HashCode; } );
			return hashes;
		}
		#endregion
	}
}