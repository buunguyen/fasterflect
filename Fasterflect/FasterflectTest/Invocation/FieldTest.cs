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
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class FieldTest : BaseInvocationTest
    {
        [TestMethod]
        public void TestAccessStaticFields()
        {
            RunWith( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetFieldValue( "totalPeopleCreated" ) + 1;
                   type.SetFieldValue( "totalPeopleCreated", totalPeopleCreated );
                   VerifyFields( type, new { totalPeopleCreated } );
               } );
        }

        [TestMethod]
        public void TestSetStaticFieldBySample()
        {
            RunWith( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetFieldValue( "totalPeopleCreated" ) + 1;
                   type.SetFields( new { totalPeopleCreated } );
                   VerifyFields( type, new { totalPeopleCreated } );
               } );
        }

        [TestMethod]
        public void TestAccessStaticFieldViaFieldInfo()
        {
            RunWith( ( Type type ) =>
               {
                   var fieldInfo = type.Field( "totalPeopleCreated", Flags.StaticCriteria );
                   var totalPeopleCreated = (int) fieldInfo.Get() + 1;
                   fieldInfo.Set( totalPeopleCreated );
                   VerifyFields( type, new { totalPeopleCreated } );
               } );
        }

        [TestMethod]
        public void TestAccessInstanceFields()
        {
            RunWith( ( object person ) =>
               {
                   var name = (string) person.GetFieldValue( "name" ) + " updated";
                   person.SetFieldValue( "name", name );
                   VerifyFields( person, new { name } );
               } );
        }

        [TestMethod]
        public void TestAccessInstanceFieldViaFieldInfo()
        {
            RunWith( ( object person ) =>
               {
                   var fieldInfo = person.UnwrapIfWrapped().GetType().Field( "name" );
                   var name = (string) fieldInfo.Get( person ) + " updated";
                   fieldInfo.Set( person, name );
                   VerifyFields( person, new { name } );
               } );
        }

        [TestMethod]
        public void TestChainInstanceFieldSetters()
        {
            RunWith( ( object person ) =>
               {
                   person.SetFieldValue( "name", "John" )
                       .SetFieldValue( "age", 20 )
                       .SetFieldValue( "metersTravelled", 120d );
                   VerifyFields( person, new { name = "John", age = 20, metersTravelled = 120d } );
               } );
        }

        [TestMethod]
        public void TestSetInstanceFieldsBySample()
        {
            RunWith( ( object person ) =>
               {
                   person.SetFields( new { name = "John", age = 20, metersTravelled = 120d } );
                   VerifyFields( person, new { name = "John", age = 20, metersTravelled = 120d } );
               } );
        }

        [TestMethod]
        public void TestSetInstanceFieldsBySampleWithFilter()
        {
            RunWith( ( object person ) =>
               {
                   var currentAge = (int) person.GetFieldValue( "age" );
                   person.SetFields( new { name = "John", age = currentAge + 10, metersTravelled = 120d }, "name",
                                     "metersTravelled" );
                   VerifyFields( person, new { age = currentAge } );
               } );
        }

        [TestMethod]
        public void TestAccessStaticFieldsViaSubclassType()
        {
            var totalPeopleCreated = (int) EmployeeType.GetFieldValue( "totalPeopleCreated" ) + 1;
            EmployeeType.SetFieldValue( "totalPeopleCreated", totalPeopleCreated );
            VerifyFields( EmployeeType, new { totalPeopleCreated } );
        }

        [TestMethod]
        public void TestAccessInstanceFieldsViaSubclassType()
        {
            var employee = EmployeeType.CreateInstance();
            employee.SetFields( new { employeeId = 5, name = "John", age = 20, metersTravelled = 120d } );
            VerifyFields( employee, new { employeeId = 5, name = "John", age = 20, metersTravelled = 120d } );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingFieldException) )]
        public void TestSetNotExistentField()
        {
            RunWith( ( object person ) => person.SetFieldValue( "not_exist", null ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingFieldException) )]
        public void TestSetNotExistentStaticField()
        {
            RunWith( ( Type type ) => type.SetFieldValue( "not_exist", null ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingFieldException) )]
        public void TestGetNotExistentField()
        {
            RunWith( ( object person ) => person.GetFieldValue( "not_exist" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingFieldException) )]
        public void TestGetNotExistentStaticField()
        {
            RunWith( ( Type type ) => type.GetFieldValue( "not_exist" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(InvalidCastException) )]
        public void TestSetInvalidValueType()
        {
            RunWith( ( object person ) => person.SetFieldValue( "metersTravelled", 1 ) );
        }

        [TestMethod]
        [ExpectedException( typeof(NullReferenceException) )]
        public void TestSetNullToValueType()
        {
            RunWith( ( object person ) => person.SetFieldValue( "age", null ) );
        }
    }
}