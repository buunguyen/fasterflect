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
using System.Xml;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using Fasterflect;
using Fasterflect.ObjectConstruction;
using FasterflectTest.SampleModel.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.ObjectConstruction
{
	[TestClass]
	public class TryCreateInstanceTest
	{
		#region TryCreateInstance Constructor Matching Tests
		[TestMethod]
		public void TestTryCreateInstanceWithMatchingEmptyArgumentShouldInvokeConstructor1()
		{
			Lion animal = typeof(Lion).TryCreateInstance( new { } ) as Lion;
			Verify( animal, 1, Animal.LastID, "Simba", null );

			animal = typeof(Lion).TryCreateInstance( null ) as Lion;
			Verify( animal, 1, Animal.LastID, "Simba", null );
			animal = typeof(Lion).TryCreateInstance( new Dictionary<string,object>() ) as Lion;
			Verify( animal, 1, Animal.LastID, "Simba", null );

			animal = typeof(Lion).TryCreateInstance( null, null ) as Lion;
			Verify( animal, 1, Animal.LastID, "Simba", null );
			animal = typeof(Lion).TryCreateInstance( new string[ 0 ], new object[ 0 ] ) as Lion;
			Verify( animal, 1, Animal.LastID, "Simba", null );

			animal = typeof(Lion).TryCreateInstance( null, null, null ) as Lion;
			Verify( animal, 1, Animal.LastID, "Simba", null );
			animal = typeof(Lion).TryCreateInstance( new string[ 0 ], new Type[ 0 ], new object[ 0 ] ) as Lion;
			Verify( animal, 1, Animal.LastID, "Simba", null );
		}

		[TestMethod]
		public void TestTryCreateInstanceWithMatchingSingleArgumentShouldInvokeConstructor2()
		{
			Lion animal = typeof(Lion).TryCreateInstance( new { Name = "Scar" } ) as Lion;
			Verify( animal, 2, Animal.LastID, "Scar", null );

			animal = typeof(Lion).TryCreateInstance( new Dictionary<string,object> { { "Name", "Scar" } } ) as Lion;
			Verify( animal, 2, Animal.LastID, "Scar", null );

			animal = typeof(Lion).TryCreateInstance( new [] { "Name" }, new object[] { "Scar" } ) as Lion;
			Verify( animal, 2, Animal.LastID, "Scar", null );

			animal = typeof(Lion).TryCreateInstance( new [] { "Name" }, new [] { typeof(string) }, new object[] { "Scar" } ) as Lion;
			Verify( animal, 2, Animal.LastID, "Scar", null );
		}

		[TestMethod]
		public void TestTryCreateInstanceWithMatchingSingleArgumentShouldInvokeConstructor3()
		{
			Lion animal = typeof(Lion).TryCreateInstance( new { Id = 42 } ) as Lion;
			Verify( animal, 3, 42, "Simba", null );

			animal = typeof(Lion).TryCreateInstance( new Dictionary<string,object> { { "Id", 42 } } ) as Lion;
			Verify( animal, 3, 42, "Simba", null );

			animal = typeof(Lion).TryCreateInstance( new [] { "Id" }, new object[] { 42 } ) as Lion;
			Verify( animal, 3, 42, "Simba", null );

			animal = typeof(Lion).TryCreateInstance( new [] { "Id" }, new [] { typeof(string) }, new object[] { 42 } ) as Lion;
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
				Assert.AreEqual( birthday, animal.BirthDay );
			else
				Assert.IsNull( animal.BirthDay );
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

		#region TryCreateInstance with XML input
		#region Book class
		private class Book
		{
			#pragma warning disable 0169, 0649
			private int _id;
			public string Author { get; private set; }
			public string Title { get; private set; }
			public double Rating { get; private set; }
			#pragma warning restore 0169, 0649

			public Book( string author, string title, double rating )
			{
				Author = author;
				Title = title;
				Rating = rating;
			}
		}
		#endregion

		[TestMethod]
		public void TestConvertFromString()
		{
			XElement xml = new XElement( "Books",
				new XElement( "Book",
					new XAttribute( "id", 1 ),
					new XAttribute( "author", "Douglad Adams" ),
					new XAttribute( "title", "The Hitchhikers Guide to the Galaxy" ),
					new XAttribute( "rating", 4.8 )
				),
				new XElement( "Book",
					new XAttribute( "id", 2 ),
					new XAttribute( "author", "Iain M. Banks" ),
					new XAttribute( "title", "The Player of Games" ),
					new XAttribute( "rating", 4.9 )
				),
				new XElement( "Book",
					new XAttribute( "id", 3 ),
					new XAttribute( "author", "Raymond E. Feist" ),
					new XAttribute( "title", "Magician" ),
					new XAttribute( "rating", 4.2 )
				)
			);

			// now lets try to create instances of the Book class using these values
			var data = from book in xml.Elements("Book")
			           select new { id=book.Attribute("id"), author=book.Attribute("author"),
			                        title=book.Attribute("title"), rating=book.Attribute("rating") };
			
			IList<Book> books = data.Select( b => typeof(Book).TryCreateInstance( b ) as Book ).ToList();
			Assert.AreEqual( 3, books.Count );
			
			Assert.AreEqual( 1, books[ 0 ].GetFieldValue( "_id" ) );
			Assert.AreEqual( "Douglad Adams", books[ 0 ].Author );
			Assert.AreEqual( "The Hitchhikers Guide to the Galaxy", books[ 0 ].Title );
			Assert.AreEqual( 4.8, books[ 0 ].Rating );
			
			Assert.AreEqual( 2, books[ 1 ].GetFieldValue( "_id" ) );
			Assert.AreEqual( "Iain M. Banks", books[ 1 ].Author );
			Assert.AreEqual( "The Player of Games", books[ 1 ].Title );
			Assert.AreEqual( 4.9, books[ 1 ].Rating );
			
			Assert.AreEqual( 3, books[ 2 ].GetFieldValue( "_id" ) );
			Assert.AreEqual( "Raymond E. Feist", books[ 2 ].Author );
			Assert.AreEqual( "Magician", books[ 2 ].Title );
			Assert.AreEqual( 4.2, books[ 2 ].Rating );
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

			Assert.AreEqual( sample1.GetHashCode(), sample2.GetHashCode() );
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
			Array.ForEach( infos, i => { hashes[ index++ ] = i.GetHashCode(); } );
			return hashes;
		}
		#endregion
	}
}