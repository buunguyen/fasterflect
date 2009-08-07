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

namespace FasterflectTest
{
    enum Color
    {
        Red, Green, Blue
    }
    
    internal class Person
    {
        private int id;
        private static int miles;
        private static int Miles
        {
            get { return miles; }
            set { miles = value; }
        }
        protected object this[string s]
        {
            get { return null; }
            set { }
        }
        public int Id { get; set; }
        public int Age { private get; set; }
        internal Person() { }
        internal Person(int id) { this.id = id; }
        private int GetId() { return id; }
        private void SetId(int id) { this.id = id; }
        private static void SetMiles(int m) { miles = m; }
        private static int GetMiles() { return miles; }
        private static int GetMiles(int offset) { return offset + miles; }
        private object GetItself(object data) { return data; }
        private void Run() { }
        private void Run(float speed) { }
        private static void Generate() { }
        private static void Generate(Person sample) { }
    }
}
