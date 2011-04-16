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
using System.Dynamic;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Services
{
    [TestClass]
    public class CloneTest
	{
		#region Sample Reflection Classes
        private class Dynamic
        {
            public dynamic number;
            public dynamic person;
            public dynamic expando;

            public Dynamic()
            {
            }

            public Dynamic( dynamic number, dynamic person, dynamic expando )
            {
                this.number = number;
                this.person = person;
                this.expando = expando;
            }
        }

		private class Person
        {
			private DateTime lastModified = DateTime.Now;
			private Type myType = typeof(Person);
			public int Id { get; set; }
            public DateTime Birthday { get; set; }
            public string Name { get; set; }
            public int Age { get { return DateTime.Now.Year - Birthday.Year + (DateTime.Now.DayOfYear >= Birthday.DayOfYear ? 1 : 0); } }
			public DateTime LastModified { get { return lastModified; } }
            public string[] Roles { get; private set; }
            public Dictionary<string,Person> Family { get; private set; }

			public Person()
			{
			}
			public Person( int id, DateTime birthday, string name )
        	{
        		Id = id;
        		Birthday = birthday;
        		Name = name;
				Roles = new [] { "User", "Tester" };
				Family = new Dictionary<string, Person>();
        	}
		}

		private class Employee : Person
		{
			private string initials;
						
			public string Initials { get { return initials; } }
			public Employee Manager { get; set; }

			public Employee()
			{
			}

			public Employee( int id, DateTime birthday, string name, string initials ) : base( id, birthday, name )
			{
				this.initials = initials;
			}

			public Employee( int id, DateTime birthday, string name, string initials, Employee manager ) : base( id, birthday, name )
			{
				this.initials = initials;
				Manager = manager;
			}
		}
		#endregion

		#region DeepClone
		[TestMethod]
        public void TestDeepCloneSimpleObject()
        {
			DateTime birthday = new DateTime( 1973, 1, 27 );
			Person person = new Person( 42, birthday, "Arthur Dent" );
			Person clone = person.DeepClone();
			Verify( person, clone );
        }

		[TestMethod]
		public void TestDeepCloneWithSelfReference()
		{
			DateTime birthday = new DateTime( 1973, 1, 27 );
			Employee employee = new Employee( 42, birthday, "Arthur Dent", "AD" );
			employee.Manager = employee;
			Employee clone = employee.DeepClone();
			Verify( employee, clone );
			Assert.AreEqual( employee.Initials, clone.Initials );
			Assert.AreNotSame( employee.Manager, clone.Manager );
			Verify( employee.Manager, clone.Manager );
			Assert.AreEqual( employee.Manager.Initials, clone.Manager.Initials );
		}

		[TestMethod]
		public void TestDeepCloneWithCyclicObjectGraph()
		{
			DateTime birthday = new DateTime( 1973, 1, 27 );
			Employee manager = new Employee( 1, birthday, "Ford Prefect", "FP" );
			manager.Manager = manager;
			Employee employee = new Employee( 2, birthday, "Arthur Dent", "AD", manager );
			
			Employee clone = employee.DeepClone();
			Verify( employee, clone );
			Verify( employee.Manager, clone.Manager );
			Verify( employee.Manager.Manager, clone.Manager.Manager );
			Assert.AreNotSame( employee.Manager, clone.Manager );
			Assert.AreSame( clone.Manager, clone.Manager.Manager );
		}
        #endregion

        #region Dynamic

        [TestMethod]
        public void TestDynamic()
        {
            var person = new Person(42, DateTime.Now, "Arthur Dent");
            dynamic expando = new ExpandoObject();
            expando.person = person;
            expando.number = 15;
            var dynamic = new Dynamic(15, person, expando);
            expando.dynamic = dynamic;
            var clone = dynamic.DeepClone();
            Verify(dynamic, clone);
            Verify(dynamic.expando.dynamic, clone.expando.dynamic);
        }

        private static void Verify(dynamic dynamic, dynamic clone)
        {
            Assert.AreEqual(dynamic.number, clone.number);
            Verify(dynamic.person, clone.person);
            Assert.AreEqual(dynamic.expando.number, clone.expando.number);
            Verify((Person)dynamic.expando.person, (Person)clone.expando.person);
        }
        #endregion

		#region Verify Helpers
		private static void Verify( Person person, Person clone )
		{
			Assert.IsNotNull( clone );
			Assert.AreNotSame( person, clone );
			Assert.AreEqual( person.Id, clone.Id );
			Assert.AreEqual( person.Birthday, clone.Birthday );
			Assert.AreEqual( person.Name, clone.Name );
			Assert.AreEqual( person.LastModified, clone.LastModified );
			CollectionAssert.AreEqual( person.Roles, clone.Roles );
		}
		private static void Verify( Employee employee, Employee clone )
		{
			Verify( employee as Person, clone as Person );
			Assert.AreEqual( employee.Initials, clone.Initials );
			Assert.AreNotSame( employee.Manager, clone.Manager );
		}
		#endregion
    }
}
