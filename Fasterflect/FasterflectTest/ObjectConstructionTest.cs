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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class ObjectConstructionTest
	{
		#region Sample Reflection Classes
		private class Person
        {
            public int? Id { get; set; }
            public DateTime Birthday { get; set; }
            public string Name { get; set; }
            public int Age { get { return DateTime.Now.Year - Birthday.Year + (DateTime.Now.DayOfYear >= Birthday.DayOfYear ? 1 : 0); } }
        	public int ConstructorInstanceUsed { get; private set; }

            public Person()
            {
            	ConstructorInstanceUsed = 1;
            }

        	public Person( int id )
        	{
        		Id = id;
            	ConstructorInstanceUsed = 2;
        	}

        	public Person( DateTime birthday, string name )
        	{
        		Birthday = birthday;
        		Name = name;
            	ConstructorInstanceUsed = 3;
        	}

        	public Person( int id, DateTime birthday, string name )
        	{
        		Id = id;
        		Birthday = birthday;
        		Name = name;
            	ConstructorInstanceUsed = 4;
        	}
		}

		private enum Department { Development, Marketing, Sales }

		private class Employee : Person
		{
			public string Initials { get; private set; }
			public Department Department { get; private set; }

			public Employee( int id, string initials ) : base( id )
			{
				Initials = initials;
			}
		}
		#endregion

		[TestMethod]
        public void TestCreateInstanceWithMatchingEmptyArgumentShouldInvokeConstructor1()
        {
        	Type type = typeof(Person);
        	Person person = type.TryCreateInstance( new { } ) as Person;
			Assert.IsNotNull( person );
			Assert.AreEqual( 1, person.ConstructorInstanceUsed );
			Assert.IsNull( person.Id );
			Assert.IsNull( person.Name );
			Assert.AreEqual( DateTime.MinValue, person.Birthday );
        }

		[TestMethod]
        public void TestCreateInstanceWithMatchingSingleArgumentShouldInvokeConstructor2()
        {
        	Type type = typeof(Person);
        	Person person = type.TryCreateInstance( new { Id = 42 } ) as Person;
			Verify( person, 2, 42, DateTime.MinValue, null );
        }

		[TestMethod]
        public void TestCreateInstanceWithMatchingDoubleArgumentShouldInvokeConstructor3()
        {
        	Type type = typeof(Person);
			DateTime birthday = new DateTime( 1973, 1, 27 );
        	Person person = type.TryCreateInstance( new { Birthday = birthday, Name = "Arthur Dent" } ) as Person;
			Verify( person, 3, null, birthday, "Arthur Dent" );
        }

		[TestMethod]
        public void TestCreateInstanceWithMatchingCompleteArgumentShouldInvokeConstructor4()
        {
        	Type type = typeof(Person);
			DateTime birthday = new DateTime( 1973, 1, 27 );
        	Person person = type.TryCreateInstance( new { Id = 42, Birthday = birthday, Name = "Arthur Dent" } ) as Person;
			Verify( person, 4, 42, birthday, "Arthur Dent" );
        }

		[TestMethod]
        public void TestCreateInstanceWithPartialMatchShouldInvokeConstructor1AndSetProperty()
        {
        	Type type = typeof(Person);
			DateTime birthday = new DateTime( 1973, 1, 27 );
        	Person person = type.TryCreateInstance( new { Name = "Arthur Dent" } ) as Person;
			Verify( person, 1, null, DateTime.MinValue, "Arthur Dent" );
        	person = type.TryCreateInstance( new { Birthday = birthday } ) as Person;
			Verify( person, 1, null, birthday, null );
        }

		[TestMethod]
        public void TestCreateInstanceWithPartialMatchShouldInvokeConstructor4AndIgnoreExtraArgs()
		{
			Type type = typeof(Person);
			DateTime birthday = new DateTime( 1973, 1, 27 );
			Person person = type.TryCreateInstance( new { Id = 42, Birthday = birthday, Name = "Arthur Dent", Origin = "Earth" } ) as Person;
			Verify( person, 4, 42, birthday, "Arthur Dent" );
		}

    	private static void Verify( Person person, int constructorInstanceUsed, int? id, DateTime birthday, string name )
        {
			Assert.IsNotNull( person );
			Assert.AreEqual( constructorInstanceUsed, person.ConstructorInstanceUsed );
			if( id.HasValue )
				Assert.AreEqual( id.Value, person.Id.Value );
			else
				Assert.IsNull( person.Id );
			Assert.AreEqual( birthday, person.Birthday );
			if( name == null )
				Assert.IsNull( person.Name );
			else
				Assert.AreEqual( name, person.Name );
        }

        [TestMethod]
        public void TestCreateInstanceWithInvalidArgumentTypeShouldUseConstructor1()
        {
        	Type type = typeof(Person);
        	Person person = type.TryCreateInstance( new { Id = "Incompatible Argument Type" } ) as Person;
			Assert.IsNotNull( person );
			Assert.IsNull( person.Id );
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void TestCreateInstanceWithUnmatchingArgumentShouldThrow()
        {
        	Type type = typeof(Employee);
        	type.TryCreateInstance( new { Id = 42 } );
        }
    }
}
