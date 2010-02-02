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
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class MethodTest
    {
        private class PersonClass
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

            private PersonClass AddPeer(PersonClass person)
            {
                return person;
            }

            private void DoNothing() {}
            private int JustGet() { return 1; }
        }

        private struct PersonStruct
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
            private void DoNothing() { }
            private int JustGet() { return 1; }
        }

        private class Employee : PersonClass { }

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

        private static readonly List<Type> TypeList = new List<Type>
                {
                    typeof(PersonClass), 
                    typeof(PersonStruct)
                };

        [TestMethod]
        public void Test_invoke_with_co_variant_return_and_param_type()
        {
            var type = typeof (PersonClass);
            var target = type.CreateInstance();
            var employee = new Employee();
            var result = target.Invoke<Employee>("AddPeer",
                new[] { typeof(Employee) }, new object[] { employee });
            Assert.AreSame(employee, result);
        }

        [TestMethod]
        public void Test_invoke_method_with_ref_params()
        {
            var parameters = new object[] { 1, 1, 3, "original" };
            var result = typeof(Utils).Invoke<int>("Update",
                new[] { typeof(int), typeof(int).MakeByRefType(), 
                typeof(int), typeof(string).MakeByRefType() }, parameters);
            Assert.AreEqual(4, result);
            Assert.AreEqual(2, parameters[1]);
            Assert.AreEqual("changed", parameters[3]);
        }

        [TestMethod]
        public void Test_invoke_method_with_ref_params_without_returning()
        {
            var parameters = new object[] { 1, 1, 3, "original" };
            typeof(Utils).Invoke("Update",
                new[] { typeof(int), typeof(int).MakeByRefType(), 
                    typeof(int), typeof(string).MakeByRefType() }, parameters);
            Assert.AreEqual(2, parameters[1]);
            Assert.AreEqual("changed", parameters[3]);
        }

        [TestMethod]
        public void Test_invoke_no_ret_method_with_ref_params()
        {
            var parameters = new object[] { 1, "original", 2 };
            typeof(Utils).Invoke("Update",
                new[] { typeof(int).MakeByRefType(), 
                        typeof(string).MakeByRefType(),
                        typeof(int)}, parameters);
            Assert.AreEqual(2, parameters[0]);
            Assert.AreEqual("changed", parameters[1]);
        }

        [TestMethod]
        public void Test_invoke_static_methods()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.Invoke("SetStartingId", 10);
                                     var nextId = type.Invoke<int>("GetNextId");
                                     Assert.AreEqual(11, nextId);
                                 });
        }

        [TestMethod]
        public void Test_invoke_methods_without_specifying_return_type()
        {
            TypeList.ForEach(type =>
            {
                type.Invoke("SetStartingId", 10);
                var result = type.Invoke("GetNextId");
                Assert.AreSame(type, result);
            });
        }

        [TestMethod]
        public void Test_invoke_static_methods_with_args()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.Invoke("SetStartingId", 10);
                                     var nextId = type.Invoke<int>("GetNextId", 5);
                                     Assert.AreEqual(16, nextId);
                                 });
        }

        [TestMethod]
        public void Test_invoke_ret_static_methods()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.SetField("milesTraveled", 0);
                                     type.Invoke("IncreaseMiles", 5)
                                         .Invoke("IncreaseMiles", 10)
                                         .Invoke("IncreaseMiles", 20);
                                     Assert.AreEqual(35, type.GetField<int>("milesTraveled"));
                                 });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void Test_invoke_non_existent_static_method()
        {
            TypeList.ForEach(type => type.Invoke("NotExist"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void Test_invoke_non_existent_instance_method()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance().CreateHolderIfValueType();
                target.Invoke("NotExist");
            });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void Test_invoke_non_existent_instance_method_with_return_type()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance().CreateHolderIfValueType();
                target.Invoke<int>("NotExist");
            });
        }

        [TestMethod]
        public void Test_invoke_instance_methods()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.SetField("milesTraveled", 0);
                                     var target = type.CreateInstance().CreateHolderIfValueType();
                                     target.Invoke("DoNothing");
                                     Assert.AreEqual(1, target.Invoke<int>("JustGet"));

                                     int[] miles = {1, 10, 20, 100};
                                     foreach (var mile in miles)
                                     {
                                         target.Invoke("Walk", mile);
                                     }
                                     var totalMiles = target.Invoke<int>("GetMilesTraveled", 5);
                                     Assert.AreEqual(miles.Sum() + 5, totalMiles);
                                 });
        }

        [TestMethod]
        public void Test_invoke_ret_instance_methods()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.SetField("milesTraveled", 0);
                                     var target = type.CreateInstance().CreateHolderIfValueType();
                                     target.Invoke("GetMilesTraveled", new[] { typeof(int) }, 5)
                                           .Invoke("GetMilesTraveled", new[] {typeof (int)}, 10)
                                           .Invoke("GetMilesTraveled", new[] {typeof (int)}, 20);
                                     var totalMiles = type.GetField<int>("milesTraveled");
                                     Assert.AreEqual(35, totalMiles);
                                 });
        }
    }
}
