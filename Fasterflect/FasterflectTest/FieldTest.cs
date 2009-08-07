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
    public class FieldTest
    {
        private class Person
        {
            private static int counter;
            private int age;
            protected string name;
            public Person peer;
            internal Color[] favoriteColors;
        }

        private Reflector reflector;
        private object target;

        [TestInitialize()]
        public void TestInitialize()
        {
            reflector = Reflector.Create();
            target = new Person();
        }

        [TestMethod]
        public void test_set_and_get_static_fields()
        {
            reflector.SetField(typeof(Person), "counter", 1);
            Assert.AreEqual(1, reflector.GetField<int>(typeof(Person), "counter"));

            reflector.SetFields(typeof(Person), new { counter = 2 });
            Assert.AreEqual(2, reflector.GetField<int>(typeof(Person), "counter"));
        }

        [TestMethod]
        public void test_set_and_get_fields()
        {
            var peer = new Person();
            var favColors = new[] {Color.Blue, Color.Red};
            reflector.SetField(target, "age", 10)
                .SetField(target, "name", "John")
                .SetField(target, "peer", peer)
                .SetField(target, "favoriteColors", favColors);
            Assert.AreEqual(10, reflector.GetField<int>(target, "age"));
            Assert.AreEqual("John", reflector.GetField<string>(target, "name"));
            Assert.AreSame(peer, reflector.GetField<Person>(target, "peer"));
            Assert.AreSame(favColors, reflector.GetField<Color[]>(target, "favoriteColors"));
        }

        [TestMethod]
        public void test_set_and_get_fields_via_sample()
        {
            var peer = new Person();
            var favColors = new[] { Color.Blue, Color.Red };
            reflector.SetFields(target, new {age = 10, name = "John", peer, favoriteColors = favColors});
            Assert.AreEqual(10, reflector.GetField<int>(target, "age"));
            Assert.AreEqual("John", reflector.GetField<string>(target, "name"));
            Assert.AreSame(peer, reflector.GetField<Person>(target, "peer"));
            Assert.AreSame(favColors, reflector.GetField<Color[]>(target, "favoriteColors"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void test_set_not_existent_field()
        {
            reflector.SetField(target, "not_exist", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void test_set_not_existent_static_field()
        {
            reflector.SetField(typeof(Person), "not_exist", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void test_get_not_existent_field()
        {
            reflector.GetField<int>(target, "not_exist");
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void test_get_not_existent_static_field()
        {
            reflector.GetField<int>(target, "not_exist");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void test_set_invalid_value_type()
        {
            reflector.SetField(target, "name", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void test_get_invalid_type()
        {
            reflector.GetField<string>(target, "age");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void test_set_null_to_value_type()
        {
            reflector.SetField(target, "age", null);
        }
    }
}
