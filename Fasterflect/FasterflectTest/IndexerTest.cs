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
    public class IndexerTest
    {
        private class Region {}
        private class Country : Region
        {
            private string[] cities = new string[100];
            private Region this[Region region]
            {
                get { return region;}
                set {}
            }
            private string this[int pos]
            {
                get
                {
                    return cities[pos];
                }
                set
                {
                    cities[pos] = value;
                }
            }
            private string this[int part1, int part2]
            {
                get
                {
                    return cities[part1 + part2];
                }
                set
                {
                    cities[part1 + part2] = value;
                }
            }
        }

        private Reflector reflector;
        private object target;

        [TestInitialize()]
        public void TestInitialize()
        {
            reflector = Reflector.Create();
            target = new Country();
        }

        [TestMethod()]
        public void test_indexer()
        {
            reflector.SetIndexer(target, new[] { typeof(int), typeof(string) }, new object[] { 10, "John" });
            Assert.AreEqual("John", reflector.GetIndexer<string>(target, new[] { typeof(int) }, new object[] { 10 }));

            reflector.SetIndexer(target, new[] { typeof(int), typeof(int), typeof(string) }, new object[] { 1, 2, "Jane" });
            Assert.AreEqual("Jane", reflector.GetIndexer<string>(target, new[] { typeof(int), typeof(int) }, new object[] { 1, 2 }));
        }

        [TestMethod]
        public void test_invoke_with_co_variant_return_and_param_type()
        {
            var country = new Country();
            var result = reflector.GetIndexer<Country>(target,
                new[] { typeof(Country) }, new object[] { country });
            Assert.AreSame(country, result);
        }
    }
}
