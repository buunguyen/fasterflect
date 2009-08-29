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
using System.Linq;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class MethodTest
    {
        private class Person
        {
            private static int nextId;
            private static int milesTraveled;
            private static int GetNextId()
            {
                return ++nextId;
            }
            private static int GetNextId(int offset)
            {
                return ++nextId + offset;
            }
            private static void SetStartingId(int id)
            {
                nextId = id;
            }
            private void Walk(int miles)
            {
                milesTraveled += miles;
            }
            private int GetMilesTraveled()
            {
                return milesTraveled;
            }
            private int GetMilesTraveled(int increase)
            {
                milesTraveled += increase;
                return milesTraveled;
            }
            private static int IncreaseMiles(int miles)
            {
                milesTraveled += miles;
                return milesTraveled;
            }

            private Person AddPeer(Person person)
            {
                return person;
            }
        }

        private class Employee : Person
        {
            
        }

        class Utils
        {
            private static int Update(int i, ref int j, int k, out string s)
            {
                j = 2;
                s = "changed";
                return i + k;
            }

            private static void Update(out int i, out string s, int j)
            {
                i = j;
                s = "changed";
            }
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
        public void test_invoke_with_co_variant_return_and_param_type()
        {
            var employee = new Employee();
            var result = reflector.Invoke<Employee>(target, "AddPeer",
                new[] { typeof(Employee) }, new object[] { employee });
            Assert.AreSame(employee, result);
        }

        [TestMethod]
        public void test_invoke_method_with_ref_params()
        {
            var parameters = new object[] { 1, 1, 3, "original"};
            var result = reflector.Invoke<int>(typeof(Utils), "Update", 
                new[] { typeof(int), typeof(int).MakeByRefType(), 
                    typeof(int), typeof(string).MakeByRefType() }, parameters);
            Assert.AreEqual(4, result);
            Assert.AreEqual(2, parameters[1]);
            Assert.AreEqual("changed", parameters[3]);
        }

        [TestMethod]
        public void test_invoke_method_with_ref_params_without_returning()
        {
            var parameters = new object[] { 1, 1, 3, "original" };
            reflector.Invoke(typeof(Utils), "Update",
                new[] { typeof(int), typeof(int).MakeByRefType(), 
                    typeof(int), typeof(string).MakeByRefType() }, parameters);
            Assert.AreEqual(2, parameters[1]);
            Assert.AreEqual("changed", parameters[3]);
        }

        [TestMethod]
        public void test_invoke_no_ret_method_with_ref_params()
        {
            var parameters = new object[] { 1, "original", 2 };
            reflector.Invoke(typeof(Utils), "Update",
                new[] { typeof(int).MakeByRefType(), 
                        typeof(string).MakeByRefType(),
                        typeof(int)}, parameters);
            Assert.AreEqual(2, parameters[0]);
            Assert.AreEqual("changed", parameters[1]);
        }

        [TestMethod]
        public void test_invoke_static_methods()
        {
            reflector.Invoke(typeof(Person), "SetStartingId", new[] { typeof(int) }, new object[] { 10 });
            var nextId = reflector.Invoke<int>(typeof(Person), "GetNextId"); 
            Assert.AreEqual(11, nextId);
        }

        [TestMethod]
        public void test_invoke_static_methods_with_args()
        {
            reflector.Invoke(typeof(Person), "SetStartingId", new[] { typeof(int) }, new object[] { 10 });
            var nextId = reflector.Invoke<int>(typeof(Person), "GetNextId", new[] { typeof(int) }, new object[] { 5 });
            Assert.AreEqual(16, nextId);
        }

        [TestMethod]
        public void test_invoke_ret_static_methods()
        {
            reflector.SetField(typeof (Person), "milesTraveled", 0);
            reflector.Invoke(typeof(Person), "IncreaseMiles", new[] { typeof(int) }, new object[] { 5 })
                     .Invoke(typeof(Person), "IncreaseMiles", new[] { typeof(int) }, new object[] { 10 })
                     .Invoke(typeof(Person), "IncreaseMiles", new[] { typeof(int) }, new object[] { 20 });
            Assert.AreEqual(35, reflector.GetField<int>(typeof(Person), "milesTraveled"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void test_invoke_non_existent_static_method()
        {
            reflector.Invoke(typeof(Person), "NotExist");
        }

        [TestMethod]
        public void test_invoke_instance_methods()
        {
            reflector.SetField(typeof(Person), "milesTraveled", 0);
            int[] miles = {1, 10, 20, 100};
            foreach (var mile in miles)
            {
                reflector.Invoke(target, "Walk", new[] { typeof(int) }, new object[] { mile });
            }
            var totalMiles = reflector.Invoke<int>(target, "GetMilesTraveled");
            Assert.AreEqual(miles.Sum(), totalMiles);
        }

        [TestMethod]
        public void test_invoke_ret_instance_methods()
        {
            reflector.SetField(typeof(Person), "milesTraveled", 0);
            reflector.Invoke(target, "GetMilesTraveled", new[] { typeof(int) }, new object[] { 5 })
                     .Invoke(target, "GetMilesTraveled", new[] { typeof(int) }, new object[] { 10 })
                     .Invoke(target, "GetMilesTraveled", new[] { typeof(int) }, new object[] { 20 });
            var totalMiles = reflector.GetField<int>(typeof(Person), "milesTraveled"); ;
            Assert.AreEqual(35, totalMiles);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void test_invoke_non_existent_instance_method()
        {
            reflector.Invoke(target, "NotExist");
        }
    }
}
