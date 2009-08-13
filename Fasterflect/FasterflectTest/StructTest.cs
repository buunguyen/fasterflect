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

using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class StructTest
    {
        internal struct Animal
        {
            private int id;
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
            Assert.AreEqual(2, obj.GetField<int>("id"));
            Assert.AreEqual(2, obj.Invoke<int>("GetId"));

            type.Invoke("Generate");
            type.Invoke("Generate", new[] { type }, new[] { type.Construct() });
        }

        // [TestMethod] // struct instance support is being worked on
        public void test_invoke_instance_members_of_struct()
        {
            object obj = target;
            reflector.SetField(obj, "id", 3);
            Assert.AreEqual(3, target.GetField<int>("id"));

            target.SetField("id", 3);
            Assert.AreEqual(3, target.GetField<int>("id"));

            target.SetFields(new { id = 4 });
            Assert.AreEqual(4, target.GetField<int>("id"));

            target.SetProperty("Id", 4);
            Assert.AreEqual(4, target.GetProperty<int>("Id"));

            target.SetProperties(new { Id = 5 });
            Assert.AreEqual(5, target.GetProperty<int>("Id"));

            reflector.SetIndexer(target, new[] { typeof(string), typeof(object) }, new object[] { "a", null });

            target.GetIndexer<object>(new[] { typeof(string) }, new object[] { "a" });

            target.Invoke("SetId", new[] { typeof(int) }, new object[] { 4 });
            Assert.AreEqual(4, target.Invoke<int>("GetId"));

            target.Invoke("Run");
            target.Invoke("Run", new[] { typeof(float) }, new object[] { 4.4f });

            var tmp = new object();
            Assert.AreSame(tmp, target.Invoke<object>("GetItself", new[] { typeof(object) }, new[] { tmp }));
        }
    }
}
