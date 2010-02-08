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
using Fasterflect.Common;
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
        #region Direct
        [TestMethod]
        public void Test_set_get_static_fields()
        {
            TypeList.ForEach(type =>
                                 {
                                     type.SetFieldValue("counter", 1);
                                     Assert.AreEqual(1, type.GetFieldValue("counter"));
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
                target.SetFieldValue("age", 10)
                      .SetFieldValue("peer", peer)
                      .SetFieldValue("favoriteColors", favColors);
                Assert.AreEqual(10, target.GetFieldValue("age"));
                Assert.AreSame(peer, target.GetFieldValue("peer"));
                Assert.AreSame(favColors, target.GetFieldValue("favoriteColors"));
            });
        }

        [TestMethod]
        public void Test_set_and_get_static_fields_via_sample()
        {
            TypeList.ForEach(type =>
            {
                type.SetFields(new { counter = 2 });
                Assert.AreEqual(2, type.GetFieldValue("counter"));
            });
        }

        [TestMethod]
        public void Test_set_and_get_static_fields_via_sample_with_filter()
        {
            TypeList.ForEach(type =>
            {
                var currentValue = (int)type.GetFieldValue( "counter" );
                type.SetFields(new { counter = (currentValue + 1) }, "non_exist");
                Assert.AreEqual(currentValue, type.GetFieldValue("counter"));
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
                Assert.AreEqual(10, target.GetFieldValue("age"));
                Assert.AreSame(peer, target.GetFieldValue("peer"));
                Assert.AreSame(favColors, target.GetFieldValue("favoriteColors"));
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
                Assert.AreEqual(10, target.GetFieldValue("age"));
                Assert.AreSame(null, target.GetFieldValue("peer"));
                Assert.AreSame(null, target.GetFieldValue("favoriteColors"));
            });
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void Test_set_not_existent_field()
        {
            TypeList.ForEach(type => type.CreateInstance().SetFieldValue("not_exist", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void Test_set_not_existent_static_field()
        {
            TypeList.ForEach(type => type.SetFieldValue("not_exist", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void Test_get_not_existent_field()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().GetFieldValue("not_exist"));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void Test_get_not_existent_static_field()
        {
            TypeList.ForEach(type => type.GetFieldValue("not_exist"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Test_set_invalid_value_type()
        {
            TypeList.ForEach(type => type.CreateInstance().CreateHolderIfValueType().SetFieldValue("peer", 1));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_set_null_to_value_type()
        {
            TypeList.ForEach(type => type.CreateInstance().SetFieldValue("age", null));
        }
        #endregion

        #region FieldInfo
        [TestMethod]
        public void Test_set_get_static_fields_via_field_info()
        {
            TypeList.ForEach(type =>
            {
                var fieldInfo = type.Field("counter", Flags.StaticCriteria);
                var value = (int)fieldInfo.GetValue() + 1;
                fieldInfo.SetValue(value);
                Assert.AreEqual(value, fieldInfo.GetValue());
            });
        }

        [TestMethod]
        public void Test_set_and_get_fields_via_field_info()
        {
            TypeList.ForEach(type =>
            {
                var target = type.CreateInstance().CreateHolderIfValueType();
                var peer = new PersonClass();
                var favColors = new[] { Color.Blue, Color.Red };
                var ageInfo = type.Field("age");
                var peerInfo = type.Field("peer");
                var favoriteColorsInfo = type.Field("favoriteColors");
                FieldInfoExtensions.SetValue( ageInfo, target, 10);
                FieldInfoExtensions.SetValue( peerInfo, target, peer);
                FieldInfoExtensions.SetValue( favoriteColorsInfo, target, favColors);
                Assert.AreEqual(10, FieldInfoExtensions.GetValue( ageInfo, target));
                Assert.AreSame(peer, FieldInfoExtensions.GetValue( peerInfo, target));
                Assert.AreSame(favColors, FieldInfoExtensions.GetValue( favoriteColorsInfo, target));
            });
        }
        #endregion
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
