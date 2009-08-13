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
    public class ExtensionApiTest
    {
        [TestMethod]
        public void test_uses_of_ext_api_for_class()
        {
            var type = typeof (Person);

            type.Invoke("SetMiles", new[]{ typeof(int) }, new object[] { 2 });
            Assert.AreEqual(2, type.Invoke<int>("GetMiles"));
            Assert.AreEqual(5, type.Invoke<int>("GetMiles", new[] { typeof(int) }, new object[] { 3 }));

            type.SetField("miles", 3);
            Assert.AreEqual(3, type.GetField<int>("miles"));

            type.SẹtFields(new {miles = 4});
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

            obj.SetIndexer(new[] { typeof(string), typeof(object) }, new object[] { "a", null });
            obj.GetIndexer<object>(new[] { typeof(string) }, new object[] { "a" });

            obj.SetField("id", 3);
            Assert.AreEqual(3, obj.GetField<int>("id"));

            obj.SetFields(new {id = 4});
            Assert.AreEqual(4, obj.GetField<int>("id"));

            obj.SetProperty("Id", 4);
            Assert.AreEqual(4, obj.GetProperty<int>("Id"));

            obj.SetProperties(new { Id = 5 });
            Assert.AreEqual(5, obj.GetProperty<int>("Id"));

            obj.Invoke("SetId", new[] { typeof(int) }, new object[] { 4 });
            Assert.AreEqual(4, obj.Invoke<int>("GetId"));

            obj.Invoke("Run");
            obj.Invoke("Run", new[] { typeof(float) }, new object[] { 4.4f });

            var tmp = new object();
            Assert.AreSame(tmp, obj.Invoke<object>("GetItself", new[] { typeof(object) }, new[] { tmp }));

            typeof (Person).Invoke("Generate");
            type.Invoke("Generate", new[] { type }, new[] { type.Construct() });
        }
    }
}
