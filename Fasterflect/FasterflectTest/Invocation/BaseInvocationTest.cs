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
using System.Linq;
using Fasterflect;
using FasterflectTest.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    public abstract class BaseInvocationTest
    {
        protected static readonly Type EmployeeType = typeof(Employee);
        protected static readonly Type PersonType = typeof(Person);
        protected static readonly Type PersonStructType = typeof(PersonStruct);
        protected static readonly Type[] Types = { PersonType, PersonStructType };

        protected static void VerifyProperties( Type type, object sample )
        {
            var properties = sample.GetType().Properties();
            properties.ForEach(
                                  propInfo =>
                                  Assert.AreEqual( propInfo.Get( sample ),
                                                   type.GetPropertyValue( propInfo.Name.FirstCharUpper() ) ) );
        }

        protected static void VerifyProperties( object obj, object sample )
        {
            var properties = sample.GetType().Properties();
            properties.ForEach(
                                  propInfo =>
                                  Assert.AreEqual( propInfo.Get( sample ),
                                                   obj.GetPropertyValue( propInfo.Name.FirstCharUpper() ) ) );
        }

        protected static void VerifyFields( Type type, object sample )
        {
            var properties = sample.GetType().Properties();
            properties.ForEach(
                                  propInfo =>
                                  Assert.AreEqual( propInfo.Get( sample ), type.GetFieldValue( propInfo.Name ) ) );
        }

        protected static void VerifyFields( object obj, object sample )
        {
            var properties = sample.GetType().Properties();
            properties.ForEach( propInfo =>
                                Assert.AreEqual( propInfo.Get( sample ), obj.GetFieldValue( propInfo.Name ) ) );
        }

        protected void RunWith( Action<Type> assertionAction )
        {
            Types.ForEach( assertionAction );
        }

        protected void RunWith( Action<object> assertionAction )
        {
            Types.Select( t => t.CreateInstance().WrapIfValueType() ).ForEach( assertionAction );
        }
    }
}