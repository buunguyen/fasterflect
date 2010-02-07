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
    public class FieldTest
	{
		#region Test Data
		private enum Color
        {
            Red, Green, Blue
        }

        private class PersonClass
        {
			#pragma warning disable 0169, 0649
            private static int counter;
            private int age;
            public PersonClass peer;
            internal Color[] favoriteColors;
			#pragma warning restore 0169, 0649
        }

        private struct PersonStruct
        {
			#pragma warning disable 0169, 0649
            private static int counter;
            private int age;
            public PersonClass peer; // can't use PersonStruct here (infinite initialization)
            internal Color[] favoriteColors;
			#pragma warning restore 0169, 0649
        }

        private static readonly List<Type> TypeList = new List<Type>
                {
                    typeof(PersonClass), 
                    typeof(PersonStruct)
                };
		#endregion

		#region Field Access
		[TestMethod]
        public void Test_set_get_static_fields()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.SetField("counter", 1);
                                     Assert.AreEqual(1, type.GetField<int>("counter"));
                                 });
        }

        [TestMethod]
        public void Test_set_and_get_fields()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance().CreateHolderIfValueType();
                var peer = new PersonClass();
                var favColors = new[] { Color.Blue, Color.Red };
                target.SetField("age", 10)
                      .SetField("peer", peer)
                      .SetField("favoriteColors", favColors);
                Assert.AreEqual(10, target.GetField<int>("age"));
                Assert.AreSame(peer, target.GetField<PersonClass>("peer"));
                Assert.AreSame(favColors, target.GetField<Color[]>("favoriteColors"));
            });
        }

        [TestMethod]
        public void Test_set_and_get_static_fields_via_sample()
        {
            TypeList.ForEach(type =>
            {
                type.SetFields(new { counter = 2 });
                Assert.AreEqual(2, type.GetField<int>("counter"));
            });
        }

        [TestMethod]
        public void Test_set_and_get_static_fields_via_sample_with_filter()
        {
            TypeList.ForEach(type =>
            {
                var currentValue = type.GetField<int>( "counter" );
                type.SetFields(new { counter = (currentValue + 1) }, "non_exist");
                Assert.AreEqual(currentValue, type.GetField<int>("counter"));
            });
        }

        [TestMethod]
        public void Test_set_and_get_fields_via_sample()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance().CreateHolderIfValueType();
                var peer = new PersonClass();
                var favColors = new[] { Color.Blue, Color.Red };
                target.SetFields(new {age = 10, peer, favoriteColors = favColors});
                Assert.AreEqual(10, target.GetField<int>("age"));
                Assert.AreSame(peer, target.GetField<PersonClass>("peer"));
                Assert.AreSame(favColors, target.GetField<Color[]>("favoriteColors"));
            });
        }

        [TestMethod]
        public void Test_set_and_get_fields_via_sample_with_filter()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance().CreateHolderIfValueType();
                var peer = new PersonClass();
                var favColors = new[] { Color.Blue, Color.Red };
                target.SetFields(new { age = 10, peer, favoriteColors = favColors }, "age");
                Assert.AreEqual(10, target.GetField<int>("age"));
                Assert.AreSame(null, target.GetField<PersonClass>("peer"));
                Assert.AreSame(null, target.GetField<Color[]>("favoriteColors"));
            });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void Test_set_not_existent_field()
        {
            TypeList.ForEach(type => type.CreateInstance().SetField("not_exist", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void Test_set_not_existent_static_field()
        {
            TypeList.ForEach(type => type.SetField("not_exist", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void Test_get_not_existent_field()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().GetField<int>("not_exist"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void Test_get_not_existent_static_field()
        {
            TypeList.ForEach(type => type.GetField<int>("not_exist"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Test_set_invalid_value_type()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().SetField("peer", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Test_get_invalid_type()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().GetField<string>("age"));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_set_null_to_value_type()
        {
            TypeList.ForEach(type => type.CreateInstance().SetField("age", null));
		}
		#endregion

		#region Field Lookup
		#region Single Field
		//public static FieldInfo Field<T>( this Type type, string name )
		//public static FieldInfo Field<T>( this Type type, string name, BindingFlags flags )
		//public static FieldInfo FieldDeclared<T>( this Type type, string name )
		//public static FieldInfo FieldDeclared<T>( this Type type, string name, BindingFlags flags )
		#endregion

		#region Multiple Fields
		//public static IList<FieldInfo> Fields( this Type type )
		//public static IList<FieldInfo> Fields( this Type type, BindingFlags flags )
		//public static IList<FieldInfo> FieldsDeclared( this Type type )
		//public static IList<FieldInfo> FieldsDeclared( this Type type, BindingFlags flags )
		#endregion
		#endregion
	}
}
