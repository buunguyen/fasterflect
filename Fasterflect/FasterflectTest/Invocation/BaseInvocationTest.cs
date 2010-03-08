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

using System;
using FasterflectTest.Common;
using FasterflectTest.SampleModel.People;

namespace FasterflectTest.Invocation
{
    public abstract class BaseInvocationTest : BaseTest
    {
        protected static readonly Type EmployeeType = typeof(Employee);
        protected static readonly Type PersonType = typeof(Person);
        protected static readonly Type PersonStructType = typeof(PersonStruct);

    	protected BaseInvocationTest() : base( new [] { PersonType, PersonStructType } )
    	{
    	}

        protected BaseInvocationTest(Type classType, Type structType)
            : base(new []{classType, structType})
        {
        }
    }
}