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
using System.Reflection;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Lookup
{
    [TestClass]
    public class ConstructorTest
	{
		#region Sample Classes
		private class PersonClass
        {
            public int Age;
            public int? Id;
            public string Name;
            public object Data;
            public PersonClass Peer;
            public PersonClass[] Peers;

            private PersonClass() { }
            internal PersonClass(out int i, out string s)
            {
                i = 1;
                s = "changed";
            }
            internal PersonClass(int age) { Age = age; }
            internal PersonClass(int? id) { Id = id; }
            protected PersonClass(string name) { Name = name; }
            protected PersonClass(object data) { Data = data; }
            public PersonClass(PersonClass peer) { Peer = peer; }
            public PersonClass(PersonClass[] peers) { Peers = peers; }
            internal PersonClass(int age, int? id, string name, PersonClass peer, PersonClass[] peers)
            {
                Age = age;
                Id = id;
                Name = name;
                Peer = peer;
                Peers = peers;
            }
        }

        private class PersonStruct
        {
            public int Age;
            public int? Id;
            public string Name;
            public object Data;
            public PersonClass Peer;
            public PersonClass[] Peers;

            private PersonStruct() { }
            internal PersonStruct(out int i, out string s)
            {
                i = 1;
                s = "changed";
            }
            internal PersonStruct(int age) { Age = age; }
            internal PersonStruct(int? id) { Id = id; }
            protected PersonStruct(string name) { Name = name; }
            protected PersonStruct(object data) { Data = data; }
            public PersonStruct(PersonClass peer) { Peer = peer; }
            public PersonStruct(PersonClass[] peers) { Peers = peers; }
            internal PersonStruct(int age, int? id, string name, PersonClass peer, PersonClass[] peers)
            {
                Age = age;
                Id = id;
                Name = name;
                Peer = peer;
                Peers = peers;
            }
        }

        class Employee : PersonClass
        {
            internal Employee(int age) : base(age) { }
        }

        private static readonly List<Type> TypeList = new List<Type>
                {
                    typeof(PersonClass), 
                    typeof(PersonStruct)
                };
		#endregion

		#region Constructor Lookup Tests
		[TestMethod]
		public void TestFindAllConstructorsOnPersonClassShouldFindNine()
		{
			IList<ConstructorInfo> constructors = typeof(PersonClass).Constructors().ToList();
			Assert.IsNotNull( constructors );
			Assert.AreEqual( 9, constructors.Count );
		}

		[TestMethod]
		public void TestFindSpecificConstructorOnPersonClassShouldReturnNullForNoMatch()
		{
			Type[] paramTypes = new[] { typeof(int), typeof(int) };
			Assert.IsNull( typeof(PersonClass).Constructor( paramTypes ) );
		}

		[TestMethod]
		public void TestFindSpecificConstructorOnPersonClassShouldReturnSingleForMatch()
		{
			Type[] paramTypes = new[] { typeof(int) };
			ConstructorInfo constructor = typeof(PersonClass).Constructor( paramTypes );
			Assert.IsNotNull( constructor );
			Assert.AreEqual( "age", constructor.GetParameters()[ 0 ].Name );
			
			paramTypes = new[] { typeof(int?) };
			constructor = typeof(PersonClass).Constructor( paramTypes );
			Assert.IsNotNull( constructor );
			Assert.AreEqual( "id", constructor.GetParameters()[ 0 ].Name );
		}

		[TestMethod]
		public void TestFindSpecificConstructorsOnPersonClass()
		{
			IList<ConstructorInfo> constructors = typeof(PersonClass).Constructors().ToList();
			Assert.IsNotNull( constructors );
			Assert.AreEqual( 9, constructors.Count );
		}

		[TestMethod]
		public void TestConstructorLookupOnEmployeeShouldFindOne()
		{
			IList<ConstructorInfo> constructors = typeof(Employee).Constructors().ToList();
			Assert.IsNotNull( constructors );
			Assert.AreEqual( 1, constructors.Count );
		}
		#endregion
    }
}
