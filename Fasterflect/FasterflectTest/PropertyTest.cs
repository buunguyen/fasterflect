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
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class PropertyTest
    {
        private class PersonClass
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
            public bool LikeMusic { get { return true; } }
            public PersonClass Peer { get; set; }
        }

        private struct PersonStruct
        {
            private static int counter;
            public static int Counter
            {
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
        }

        private static readonly List<Type> TypeList = new List<Type>
                {
                    typeof(PersonClass), 
                    typeof(PersonStruct)
                };

        [TestMethod]
        public void Test_set_and_get_static_properties()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.SetProperty("Counter", 1);
                                     Assert.AreEqual(1, type.GetProperty<int>("Counter"));

                                     type.SetProperties(new { Counter = 2 });
                                     Assert.AreEqual(2, type.GetProperty<int>("Counter"));
                                 });
        }

        [TestMethod]
        public void Test_set_and_get_properties()
        {
            TypeList.ForEach(type =>
                                {
                                    var target = type.CreateInstance().CreateHolderIfValueType();
                                    target.SetProperty("Age", 10);
                                    Assert.AreEqual(10, target.GetProperty<int>("Age"));
                                    target.SetProperty("IsGeek", true);
                                });
        }

        [TestMethod]
        public void Test_set_and_get_properties_via_sample()
        {
            TypeList.ForEach(type =>
                                {
                                    var target = type.CreateInstance().CreateHolderIfValueType();
                                    target.SetProperties(new { Age = 10, Name = "John" });
                                    Assert.AreEqual(10, target.GetProperty<int>("Age"));
                                });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_use_non_existent_setter()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance();
                target.SetProperty("LikeMusic", target);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_use_non_existent_getter()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().GetProperty<bool>("IsGeek"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_set_non_existent_property()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().SetProperty("NotExist", true));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_set_non_existent_static_property()
        {
            TypeList.ForEach(type => type.SetProperty("NotExist", true));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_get_non_existent_property()
        {
            TypeList.ForEach(type => type.GetProperty<int>("NotExist"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_get_non_existent_static_property()
        {
            TypeList.ForEach(type => type.GetProperty<int>("NotExist"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Test_set_invalid_value_type()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().SetProperty("Name", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Test_get_invalid_type()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().GetProperty<string>("Age"));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_set_null_to_value_type()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().SetProperty("Age", null));
        }
    }
}
