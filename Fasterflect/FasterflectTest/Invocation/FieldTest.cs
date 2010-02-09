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
            _( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetFieldValue( "totalPeopleCreated" ) + 1;
                   type.SetFieldValue( "totalPeopleCreated", totalPeopleCreated );
                   VerifyFields( type, new { totalPeopleCreated } );
               } );
        }

        [TestMethod]
        public void TestSetStaticFieldBySample()
        {
            _( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetFieldValue( "totalPeopleCreated" ) + 1;
                   type.SetFields( new { totalPeopleCreated } );
                   VerifyFields( type, new { totalPeopleCreated } );
               } );
        }

        [TestMethod]
        public void TestAccessStaticFieldViaFieldInfo()
        {
            _( ( Type type ) =>
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
            _( ( object person ) =>
               {
                   var name = (string) person.GetFieldValue( "name" ) + " updated";
                   person.SetFieldValue( "name", name );
                   VerifyFields( person, new { name } );
               } );
        }

        [TestMethod]
        public void TestAccessInstanceFieldViaFieldInfo()
        {
            _( ( object person ) =>
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
            _( ( object person ) =>
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
            _( ( object person ) =>
               {
                   person.SetFields( new { name = "John", age = 20, metersTravelled = 120d } );
                   VerifyFields( person, new { name = "John", age = 20, metersTravelled = 120d } );
               } );
        }

        [TestMethod]
        public void TestSetInstanceFieldsBySampleWithFilter()
        {
            _( ( object person ) =>
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
            _( ( object person ) => person.SetFieldValue( "not_exist", null ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingFieldException) )]
        public void TestSetNotExistentStaticField()
        {
            _( ( Type type ) => type.SetFieldValue( "not_exist", null ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingFieldException) )]
        public void TestGetNotExistentField()
        {
            _( ( object person ) => person.GetFieldValue( "not_exist" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingFieldException) )]
        public void TestGetNotExistentStaticField()
        {
            _( ( Type type ) => type.GetFieldValue( "not_exist" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(InvalidCastException) )]
        public void TestSetInvalidValueType()
        {
            _( ( object person ) => person.SetFieldValue( "metersTravelled", 1 ) );
        }

        [TestMethod]
        [ExpectedException( typeof(NullReferenceException) )]
        public void TestSetNullToValueType()
        {
            _( ( object person ) => person.SetFieldValue( "age", null ) );
        }
    }
}