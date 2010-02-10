#region License

// Copyright 2010 Buu Nguyen, Morten Mertner
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

namespace FasterflectTest.Model
{
    public class Employee : Person, ISwimmable
    {
        private int employeeId;

        public int EmployeeId
        {
            get { return employeeId; }
            set { employeeId = value; }
        }

        public Employee[] Subordinates { get; private set; }

        public Employee( string name, int age ) : base( name, age )
        {
            employeeId = GetTotalPeopleCreated();
        }

        public Employee() : this( string.Empty, 0 )
        {
        }

        public Employee( Employee[] subordinates ) : this( string.Empty, 0 )
        {
            Subordinates = subordinates;
        }

        void ISwimmable.Swim( double meters )
        {
            metersTravelled += meters;
        }
    }
}