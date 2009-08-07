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
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class ConstructorTest
    {
        private class Person
        {
            public int Age;
            public int? Id;
            public string Name;
            public object Data;
            public Person Peer;
            public Person[] Peers;

            private Person(){}
            internal Person(int age) { Age = age; }
            internal Person(int? id) { Id = id; }
            protected Person(string name) { Name = name; }
            protected Person(object data) { Data = data; }
            public Person(Person peer) { Peer = peer; }
            public Person(Person[] peers) { Peers = peers; }
            internal Person(int age, int? id, string name, Person peer, Person[] peers)
            {
                Age = age;
                Id = id;
                Name = name;
                Peer = peer;
                Peers = peers;
            }
        }

        private Reflector reflector;

        [TestInitialize()]
        public void TestInitialize()
        {
            reflector = Reflector.Create();
        }

        [TestMethod]
        public void test_create_instances()
        {
            var obj = reflector.Construct(typeof (Person));
            Assert.IsNotNull(obj);

            obj = reflector.Construct(typeof(Person), new []{typeof(int)}, new object[]{1});
            Assert.IsNotNull(obj);
            Assert.AreEqual(1, reflector.GetField<int>(obj, "Age"));

            obj = reflector.Construct(typeof(Person), new[] { typeof(int?) }, new object[] { null });
            Assert.IsNotNull(obj);
            Assert.AreEqual(null, reflector.GetField<int?>(obj, "Id"));

            obj = reflector.Construct(typeof(Person), new[] { typeof(string) }, new object[] { "Jane" });
            Assert.IsNotNull(obj);
            Assert.AreEqual("Jane", reflector.GetField<string>(obj, "Name"));

            obj = reflector.Construct(typeof(Person), new[] { typeof(object) }, new object[] { reflector });
            Assert.IsNotNull(obj);
            Assert.AreSame(reflector, reflector.GetField<object>(obj, "Data"));

            obj = reflector.Construct(typeof(Person), new[] { typeof(object) }, new object[] { 1.1d }); 
            Assert.IsNotNull(obj);
            Assert.AreEqual(1.1d, reflector.GetField<double>(obj, "Data"));

            var peer = new Person(1);
            obj = reflector.Construct(typeof(Person), new[] { typeof(Person) },
                new object[] { peer });
            Assert.IsNotNull(obj);
            Assert.AreSame(peer, reflector.GetField<Person>(obj, "Peer"));

            var peers = new[] {new Person(1), new Person(1)};
            obj = reflector.Construct(typeof(Person), new[] { typeof(Person).MakeArrayType() },
                new object[] { peers });
            Assert.IsNotNull(obj);
            Assert.AreSame(peers, reflector.GetField<Person[]>(obj, "Peers"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void test_use_not_existance_constructor()
        {
            reflector.Construct(typeof(Person), new[]{typeof(double)}, new object[]{1.1});
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void test_pass_invalid_data_type()
        {
            reflector.Construct(typeof (Person), new[] {typeof (int)}, new object[] {"string"});
        }
    }
}
