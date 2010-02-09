using System;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class PropertyTest : BaseInvocationTest
    {
        [TestMethod]
        public void TestAccessStaticProperties()
        {
            _( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetPropertyValue( "TotalPeopleCreated" ) + 1;
                   type.SetPropertyValue( "TotalPeopleCreated", totalPeopleCreated );
                   VerifyProperties( type, new { totalPeopleCreated } );
               } );
        }

        [TestMethod]
        public void TestSetStaticPropertyBySample()
        {
            _( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetPropertyValue( "TotalPeopleCreated" ) + 1;
                   type.SetProperties( new { TotalPeopleCreated = totalPeopleCreated } );
                   VerifyProperties( type, new { totalPeopleCreated } );
               } );
        }

        [TestMethod]
        public void TestAccessStaticPropertyViaPropertyInfo()
        {
            _( ( Type type ) =>
               {
                   var propInfo = type.Property( "TotalPeopleCreated", Flags.StaticCriteria );
                   var totalPeopleCreated = (int) propInfo.Get() + 1;
                   propInfo.Set( totalPeopleCreated );
                   VerifyProperties( type, new { totalPeopleCreated } );
               } );
        }

        [TestMethod]
        public void TestAccessInstanceProperties()
        {
            _( ( object person ) =>
               {
                   var name = (string) person.GetPropertyValue( "Name" ) + " updated";
                   person.SetPropertyValue( "Name", name );
                   VerifyProperties( person, new { name } );
               } );
        }

        [TestMethod]
        public void TestAccessInstancePropertyViaPropertyInfo()
        {
            _( ( object person ) =>
               {
                   var propInfo = person.UnwrapIfWrapped().GetType().Property( "Name" );
                   var name = (string) propInfo.Get( person ) + " updated";
                   propInfo.Set( person, name );
                   VerifyProperties( person, new { name } );
               } );
        }

        [TestMethod]
        public void TestChainInstancePropertySetters()
        {
            _( ( object person ) =>
               {
                   person.SetPropertyValue( "Name", "John" )
                       .SetPropertyValue( "Age", 20 )
                       .SetPropertyValue( "MetersTravelled", 120d );
                   VerifyProperties( person, new { Name = "John", Age = 20, MetersTravelled = 120d } );
               } );
        }

        [TestMethod]
        public void TestSetInstancePropertiesBySample()
        {
            _( ( object person ) =>
               {
                   person.SetProperties( new { Name = "John", Age = 20, MetersTravelled = 120d } );
                   VerifyProperties( person, new { Name = "John", Age = 20, MetersTravelled = 120d } );
               } );
        }

        [TestMethod]
        public void TestSetInstancePropertiesBySampleWithFilter()
        {
            _( ( object person ) =>
               {
                   var currentAge = (int) person.GetPropertyValue( "Age" );
                   person.SetProperties( new { Name = "John", Age = 20, MetersTravelled = 120d }, "Name",
                                         "MetersTravelled" );
                   VerifyProperties( person, new { age = currentAge } );
               } );
        }

        [TestMethod]
        public void TestAccessStaticPropertiesViaSubclassType()
        {
            var totalPeopleCreated = (int) EmployeeType.GetPropertyValue( "TotalPeopleCreated" ) + 1;
            EmployeeType.SetPropertyValue( "TotalPeopleCreated", totalPeopleCreated );
            VerifyProperties( EmployeeType, new { totalPeopleCreated } );
        }

        [TestMethod]
        public void TestAccessInstancePropertiesViaSubclassType()
        {
            var employee = EmployeeType.CreateInstance();
            employee.SetProperties( new { EmployeeId = 5, Name = "John", Age = 20, MetersTravelled = 120d } );
            VerifyProperties( employee, new { EmployeeId = 5, Name = "John", Age = 20, MetersTravelled = 120d } );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingMemberException) )]
        public void TestSetNotExistentProperty()
        {
            _( ( object person ) => person.SetPropertyValue( "not_exist", null ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingMemberException) )]
        public void TestSetNotExistentStaticProperty()
        {
            _( ( Type type ) => type.SetPropertyValue( "not_exist", null ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingMemberException) )]
        public void TestGetNotExistentProperty()
        {
            _( ( object person ) => person.GetPropertyValue( "not_exist" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingMemberException) )]
        public void TestGetNotExistentStaticProperty()
        {
            _( ( Type type ) => type.GetPropertyValue( "not_exist" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(InvalidCastException) )]
        public void TestSetInvalidValueType()
        {
            _( ( object person ) => person.SetPropertyValue( "MetersTravelled", 1 ) );
        }

        [TestMethod]
        [ExpectedException( typeof(NullReferenceException) )]
        public void TestSetNullToValueType()
        {
            _( ( object person ) => person.SetPropertyValue( "Age", null ) );
        }
    }
}