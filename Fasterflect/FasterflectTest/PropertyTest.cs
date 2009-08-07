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
    /// <summary>
    /// Summary description for PropertyTest
    /// </summary>
    [TestClass]
    public class PropertyTest
    {
        private class Person
        {
            private static int counter;
            public static int Counter { 
                private get
                {
                    return counter;
                }
                set 
                { 
                    counter = value;
                }
            }
            public int Age { private get; set; }
            public string Name { get; private set; }
            public bool IsGeek { set { } }
            public Person Peer { get { return null; } }
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
        public void test_set_and_get_static_properties()
        {
            reflector.SetProperty(typeof(Person), "Counter", 1);
            Assert.AreEqual(1, reflector.GetProperty<int>(typeof(Person), "Counter"));

            reflector.SetProperties(typeof(Person), new { Counter = 2 });
            Assert.AreEqual(2, reflector.GetProperty<int>(typeof(Person), "Counter"));
        }

        [TestMethod]
        public void test_set_and_get_properties()
        {
            reflector.SetProperty(target, "Age", 10);
            reflector.SetProperty(target, "IsGeek", true);
            Assert.AreEqual(10, reflector.GetProperty<int>(target, "Age"));
        }

        [TestMethod]
        public void test_set_and_get_properties_via_sample()
        {
            reflector.SetProperties(target, new {Age = 10, Name = "John" });
            Assert.AreEqual(10, reflector.GetProperty<int>(target, "Age"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void test_use_non_existent_setter()
        {
            reflector.SetProperty(target, "Peer", new Person());
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void test_use_non_existent_getter()
        {
            reflector.GetProperty<bool>(target, "IsGeek");
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void test_set_non_existent_property()
        {
            reflector.SetProperty(target, "NotExist", true);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void test_set_non_existent_static_property()
        {
            reflector.SetProperty(typeof(Person), "NotExist", true);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void test_get_non_existent_property()
        {
            reflector.GetProperty<int>(target, "NotExist");
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void test_get_non_existent_static_property()
        {
            reflector.GetProperty<int>(typeof(Person), "NotExist");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void test_set_invalid_value_type()
        {
            reflector.SetProperty(target, "Name", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void test_get_invalid_type()
        {
            reflector.GetProperty<string>(target, "Age");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void test_set_null_to_value_type()
        {
            reflector.SetProperty(target, "Age", null);
        }
    }
}
