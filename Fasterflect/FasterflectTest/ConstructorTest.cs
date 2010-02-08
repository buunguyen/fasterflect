#region License
// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
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

namespace FasterflectTest
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

		#region CreateInstance Tests
		[TestMethod]
        public void Test_use_constructor_with_byref_params()
        {
            TypeList.ForEach(type =>
            {
                var parameters = new object[] { 0, "original" };
                var obj = type.CreateInstance(new[] {
                                                    typeof(int).MakeByRefType(),
                                                    typeof(string).MakeByRefType()
                                               }, parameters);
                Assert.IsNotNull(obj);
                Assert.AreEqual(1, parameters[0]);
                Assert.AreEqual("changed", parameters[1]);
            });
        }

        [TestMethod]
        public void Test_create_instances()
        {
            TypeList.ForEach(type =>
            {
                var obj = type.CreateInstance();
                Assert.IsNotNull(obj);

                obj = type.CreateInstance(new[] { typeof(int) }, 1);
                Assert.IsNotNull(obj);
                Assert.AreEqual(1, obj.GetFieldValue("Age"));

                obj = type.CreateInstance(new[] { typeof(int?) }, 1);
                Assert.IsNotNull(obj);
                Assert.AreEqual(1, obj.GetFieldValue("Id"));

                obj = type.CreateInstance(new[] { typeof(int?) }, new object[]{null});
                Assert.IsNotNull(obj);
                Assert.AreEqual(null, obj.GetFieldValue( "Id" ));

                obj = type.CreateInstance(new[] { typeof(string) }, "Jane");
                Assert.IsNotNull(obj);
                Assert.AreEqual("Jane", obj.GetFieldValue("Name"));

                obj = type.CreateInstance(new[] { typeof(object) }, type);
                Assert.IsNotNull(obj);
                Assert.AreSame(type, obj.GetFieldValue("Data"));

                obj = type.CreateInstance(new[] { typeof(object) }, 1.1d);
                Assert.IsNotNull(obj);
                Assert.AreEqual(1.1d, obj.GetFieldValue("Data"));

                var peer = new PersonClass(1);
                obj = type.CreateInstance(new[] { typeof(PersonClass) }, peer);
                Assert.IsNotNull(obj);
                Assert.AreSame(peer, obj.GetFieldValue("Peer"));

                var peers = new[] { new PersonClass(1), new PersonClass(1) };
                obj = type.CreateInstance(new[] { typeof(PersonClass).MakeArrayType() },
                    new object[] { peers });
                Assert.IsNotNull(obj);
                Assert.AreSame(peers, obj.GetFieldValue("Peers"));
            });
        }

        [TestMethod]
        public void Test_create_instances_via_ctor_info()
        {
            TypeList.ForEach( type =>
              {
                  var ctorInfo = type.Constructor();
                  Assert.IsNotNull( ctorInfo.CreateInstance() );

                  ctorInfo = type.Constructor( new[] { typeof(int) } );
                  var obj = ctorInfo.CreateInstance(1);
                  Assert.IsNotNull( obj );
                  Assert.AreEqual( 1, obj.GetFieldValue( "Age" ) );
              });
        }

        [TestMethod]
        public void Test_invoke_with_co_variant_return_and_param_type()
        {
            var peer = new Employee(1);
            var obj = typeof(PersonClass).CreateInstance(new[] { typeof(Employee) }, peer);
            Assert.AreSame(peer, obj.GetFieldValue("Peer"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Test_pass_invalid_data_type()
        {
            TypeList.ForEach(type => type.CreateInstance(new[] { typeof(int) }, "string"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_pass_none_existant_constructor()
        {
            TypeList.ForEach(type => type.CreateInstance(new[] { typeof(int), typeof(int) }, 1, 2));
        }
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
