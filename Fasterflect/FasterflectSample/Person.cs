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

namespace FasterflectSample
{
    class Person
    {
        private int id;
        private int milesTraveled;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Name { get; private set; }
        private static int InstanceCount;

        public Person() : this(0) {}

        public Person(int id) : this(id, string.Empty) { }

        public Person(int id, string name)
        {
            Id = id;
            Name = name;
            InstanceCount++;
        }

        public char this[int index]
        {
            get { return Name[index]; }
        }

        private void Walk(int miles) 
        {
            milesTraveled += miles;
        }

        private static void IncreaseInstanceCount()
        {
            InstanceCount++;
        }

        private static int GetInstanceCount()
        {
            return InstanceCount;
        }
    }
}
