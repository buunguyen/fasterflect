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
    public class IndexerTest
    {
        private class CountryClass 
        {
            private string[] cities ;
            public CountryClass(int capacity)
            {
                cities = new string[capacity];
            }
            private CountryClass this[CountryClass country]
            {
                get { return country; }
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

        private struct CountryStruct
        {
            private string[] cities ;
            public CountryStruct(int capacity)
            {
                cities = new string[capacity];
            }

            private CountryStruct this[CountryStruct country]
            {
                get { return country; }
                set { }
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

        private static readonly List<Type> TypeList = new List<Type>
                {
                    typeof(CountryStruct), 
                    typeof(CountryStruct)
                };

        [TestMethod()]
        public void Test_indexer()
        {
            TypeList.ForEach(type =>
             {
                 var target = type.Construct(100).CreateHolderIfValueType();
                 target.SetIndexer(10, "John");
                 Assert.AreEqual("John", target.GetIndexer<string>(10));
                 target.SetIndexer(new[] { typeof(int), typeof(int), typeof(string) }, 1, 2, "Jane");
                 Assert.AreEqual("Jane", target.GetIndexer<string>(new[] { typeof(int), typeof(int) }, 1, 2));
                 var other = type.Construct(1);
                 var result = target.GetIndexer<object>(other);
                 Assert.AreEqual(other, result);
             });
        }
    }
}
