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
    public class StructTest
    {
        internal struct Animal
        {
            internal int id;
            private static int miles;
            private static int Miles
            {
                get { return miles; }
                set { miles = value; }
            }

            public object this[string s]
            {
                get { return null; }
                set { }
            }
            public int Id { get; set; }
            public int Age { private get; set; }

            public Animal(int id)
                : this()
            {
                this.id = id;
            }

            internal Animal(out int i, out string s)
                : this()
            {
                this.id = id;
                i = 1;
                s = "changed";
            }

            private int GetId() { return id; }
            private void SetId(int id) { this.id = id; }
            private static void SetMiles(int m) { miles = m; }
            private static int GetMiles() { return miles; }
            private static int GetMiles(int offset) { return offset + miles; }
            private object GetItself(object data) { return data; }
            private void Run() { }
            private void Run(float speed) { }
            private static void Generate() { }
            private static void Generate(Animal sample) { }

            private int Update(int i, ref int j, int k, out string s)
            {
                j = 2;
                s = "changed";
                return id = i + k;
            }

            private void Update(out int i, out string s, int j)
            {
                id = i = j;
                s = "changed";
            }
        }

        private Reflector reflector;
        private Animal target;

        [TestInitialize()]
        public void TestInitialize()
        {
            reflector = Reflector.Create();
            target = new Animal();
        }

        [TestMethod]
        public void test_invoke_static_members_of_struct()
        {
            var type = typeof(Animal);

            type.Invoke("SetMiles", new[] { typeof(int) }, new object[] { 2 });
            Assert.AreEqual(2, typeof(Animal).Invoke<int>("GetMiles"));
            Assert.AreEqual(5, typeof(Animal).Invoke<int>("GetMiles", new[] { typeof(int) }, new object[] { 3 }));

            type.SetField("miles", 3);
            Assert.AreEqual(3, type.GetField<int>("miles"));

            type.SẹtFields(new { miles = 4 });
            Assert.AreEqual(4, type.GetField<int>("miles"));

            type.SetProperty("Miles", 5);
            Assert.AreEqual(5, type.GetProperty<int>("Miles"));

            type.SetProperties(new { Miles = 6 });
            Assert.AreEqual(6, type.GetProperty<int>("Miles"));

            object obj = type.Construct();
            Assert.IsInstanceOfType(obj, type);

            obj = type.Construct(new[] { typeof(int) }, new object[] { 2 });
            Assert.AreEqual(2, ((Animal)obj).id);

            type.Invoke("Generate");
            type.Invoke("Generate", new[] { type }, new[] { type.Construct() });
        }

        [TestMethod] // struct instance support is being worked on
        public void test_invoke_instance_members_of_struct()
        {
            var wrapper = target.CreateHolderIfValueType();
            wrapper.SetField("id", 3);
            Assert.AreEqual(3, wrapper.GetField<int>("id"));

            wrapper.SetFields(new { id = 4 });
            Assert.AreEqual(4, wrapper.GetField<int>("id"));

            wrapper.SetProperty("Id", 4);
            Assert.AreEqual(4, wrapper.GetProperty<int>("Id"));

            wrapper.SetProperties(new { Id = 5 });
            Assert.AreEqual(5, wrapper.GetProperty<int>("Id"));

            wrapper.SetIndexer(new[] { typeof(string), typeof(object) }, new object[] { "a", null });

            wrapper.GetIndexer<object>(new[] { typeof(string) }, new object[] { "a" });

            wrapper.Invoke("SetId", new[] { typeof(int) }, new object[] { 4 });
            Assert.AreEqual(4, wrapper.Invoke<int>("GetId"));

            wrapper.Invoke("Run");
            wrapper.Invoke("Run", new[] { typeof(float) }, new object[] { 4.4f });

            var tmp = new object();
            Assert.AreSame(tmp, wrapper.Invoke<object>("GetItself", new[] { typeof(object) }, new[] { tmp }));
        }

        [TestMethod]
        public void test_use_constructor_with_byref_params()
        {
            var parameters = new object[] { 0, "original" };
            var obj = reflector.Construct(typeof(Animal), new[]
                                                    {
                                                        typeof(int).MakeByRefType(),
                                                        typeof(string).MakeByRefType()
                                                    }, parameters);
            Assert.IsNotNull(obj);
            Assert.AreEqual(1, parameters[0]);
            Assert.AreEqual("changed", parameters[1]);
        }

        [TestMethod]
        public void test_invoke_method_with_ref_params()
        {
            var parameters = new object[] { 1, 1, 3, "original" };
            var wrapper = target.CreateHolderIfValueType();
            var result = wrapper.Invoke<int>("Update",
                new[] { typeof(int), typeof(int).MakeByRefType(), 
                    typeof(int), typeof(string).MakeByRefType() }, parameters);
            Assert.AreEqual(4, result);
            Assert.AreEqual(4, wrapper.GetField<int>("id"));
            Assert.AreEqual(2, parameters[1]);
            Assert.AreEqual("changed", parameters[3]);
        }

        [TestMethod]
        public void test_invoke_method_with_ref_params_without_returning()
        {
            var parameters = new object[] { 1, 1, 3, "original" };
            var wrapper = target.CreateHolderIfValueType();
            wrapper.Invoke("Update",
                new[] { typeof(int), typeof(int).MakeByRefType(), 
                    typeof(int), typeof(string).MakeByRefType() }, parameters);
            Assert.AreEqual(2, parameters[1]);
            Assert.AreEqual(4, wrapper.GetField<int>("id"));
            Assert.AreEqual("changed", parameters[3]);
        }

        [TestMethod]
        public void test_invoke_no_ret_method_with_ref_params()
        {
            var parameters = new object[] { 1, "original", 2 };
            var wrapper = target.CreateHolderIfValueType();
            wrapper.Invoke("Update",
                new[] { typeof(int).MakeByRefType(), 
                        typeof(string).MakeByRefType(),
                        typeof(int)}, parameters);
            Assert.AreEqual(2, parameters[0]);
            Assert.AreEqual(2, wrapper.GetField<int>("id"));
            Assert.AreEqual("changed", parameters[1]);
        }

        [TestMethod]
        public void test_cache_api()
        {
            var type = typeof (Animal);
            StaticAttributeGetter getCount = type.DelegateForGetStaticField("miles");
            StaticAttributeSetter setCount = type.DelegateForSetStaticField("miles");
            setCount(10);
            Assert.AreEqual(10, getCount());

            ConstructorInvoker ctor = type.DelegateForConstruct(new[] { typeof(int) });
            var animal = (Animal)ctor(2);
            Assert.AreEqual(2, animal.id);

            var wrapper = ctor(0).CreateHolderIfValueType();
            AttributeSetter idSetter = type.DelegateForSetProperty("Id");
            AttributeGetter idGetter = type.DelegateForGetProperty("Id");
            idSetter(wrapper, 20);
            Assert.AreEqual(20, idGetter(wrapper));
            idSetter(wrapper, 12);
            Assert.AreEqual(12, idGetter(wrapper));
        }
    }
}
