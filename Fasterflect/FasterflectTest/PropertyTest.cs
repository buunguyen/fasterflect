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

        #region Access
        #region Direct
        [TestMethod]
        public void Test_set_and_get_static_properties()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.SetPropertyValue("Counter", 1);
                                     Assert.AreEqual(1, type.GetPropertyValue("Counter"));
                                 });
        }

        [TestMethod]
        public void Test_set_and_get_properties()
        {
            TypeList.ForEach(type =>
                                {
                                    var target = type.CreateInstance().CreateHolderIfValueType();
                                    target.SetPropertyValue("Age", 10);
                                    Assert.AreEqual(10, target.GetPropertyValue("Age"));
                                    target.SetPropertyValue("IsGeek", true);
                                });
        }

        [TestMethod]
        public void Test_set_and_get_static_properties_via_sample()
        {
            TypeList.ForEach(type =>
            {
                type.SetProperties(new { Counter = 5 });
                Assert.AreEqual(5, type.GetPropertyValue("Counter"));
            });
        }

        [TestMethod]
        public void Test_set_and_get_static_properties_via_sample_with_filter()
        {
            TypeList.ForEach(type =>
            {
                var currentValue = (int)type.GetPropertyValue("Counter");
                type.SetProperties(new { Counter = (currentValue + 1) }, "not_exist");
                Assert.AreEqual(currentValue, type.GetPropertyValue("Counter"));
            });
        }

        [TestMethod]
        public void Test_set_and_get_properties_via_sample()
        {
            TypeList.ForEach(type =>
                                {
                                    var target = type.CreateInstance().CreateHolderIfValueType();
                                    target.SetProperties(new { Age = 10, Name = "John" });
                                    Assert.AreEqual(10, target.GetPropertyValue("Age"));
                                    Assert.AreEqual("John", target.GetPropertyValue("Name"));
                                });
        }

        [TestMethod]
        public void Test_set_and_get_properties_via_sample_with_filter()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance().CreateHolderIfValueType();
                target.SetProperties(new { Age = 10, Name = "John" }, "Age");
                Assert.AreEqual(10, target.GetPropertyValue("Age"));
                Assert.AreSame(null, target.GetPropertyValue("Name"));
            });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_use_non_existent_setter()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance();
                target.SetPropertyValue("LikeMusic", target);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_use_non_existent_getter()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().GetPropertyValue("IsGeek"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_set_non_existent_property()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().SetPropertyValue("NotExist", true));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_set_non_existent_static_property()
        {
            TypeList.ForEach(type => type.SetPropertyValue("NotExist", true));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_get_non_existent_property()
        {
            TypeList.ForEach(type => type.GetPropertyValue("NotExist"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void Test_get_non_existent_static_property()
        {
            TypeList.ForEach(type => type.GetPropertyValue("NotExist"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Test_set_invalid_value_type()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().SetPropertyValue("Name", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_set_null_to_value_type()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().SetPropertyValue("Age", null));
        }
        #endregion

        #region PropertyInfo
        [TestMethod]
        public void Test_set_and_get_static_properties_via_property_info()
        {
            TypeList.ForEach(type =>
            {
                var propInfo = type.Property("Counter", Flags.StaticCriteria);
                var value = (int)propInfo.GetValue() + 1;
                propInfo.SetValue(value);
                Assert.AreEqual(value, propInfo.GetValue());
            });
        }

        [TestMethod]
        public void Test_set_and_get_properties_via_property_info()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance().CreateHolderIfValueType();
                var age = type.Property("Age");
                var isGeek = type.Property("IsGeek");
                age.SetValue(target, 10);
                isGeek.SetValue(target, true);
                Assert.AreEqual(10, age.GetValue(target));
            });
        }
        #endregion
        #endregion
    }
}
