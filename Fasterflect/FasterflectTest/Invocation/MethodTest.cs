using System;
using System.Linq;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class MethodTest : BaseInvocationTest
    {
        [TestMethod]
        public void TestInvokeInstanceMethod()
        {
            RunWith( ( object person ) =>
               {
                   var elements = new[] { 1d, 2d, 3d, 4d, 5d };
                   elements.ForEach( element => person.Invoke( "Walk", element ) );
                   Assert.AreEqual( elements.Sum(), person.GetFieldValue( "metersTravelled" ) );
               } );
        }

        [TestMethod]
        public void TestInvokeInstanceMethodViaMethodInfo()
        {
            RunWith( ( object person ) =>
               {
                   var elements = new[] { 1d, 2d, 3d, 4d, 5d };
                   var methodInfo = person.UnwrapIfWrapped().GetType().Method( "Walk" );
                   elements.ForEach( element => methodInfo.Invoke( person, element ) );
                   Assert.AreEqual( elements.Sum(), person.GetFieldValue( "metersTravelled" ) );
               } );
        }

        [TestMethod]
        public void TestInvokeWithCoVariantReturnAndParamType()
        {
            var person = PersonType.CreateInstance();
            var friend = EmployeeType.CreateInstance();
            var result = person.Invoke( "AddFriend", friend );
            Assert.AreSame( friend, result );
        }

        [TestMethod]
        public void TestInvokeMethodWithOutArgument()
        {
            RunWith( ( object person ) =>
               {
                   var arguments = new object[] { 10d, null };
                   person.Invoke( "Walk", new[] { typeof(double), typeof(double).MakeByRefType() }, arguments );
                   Assert.AreEqual( person.GetFieldValue( "metersTravelled" ), arguments[ 1 ] );
               } );
        }

        [TestMethod]
        public void TestInvokeExplicitlyImplementedMethod()
        {
			var employee = EmployeeType.CreateInstance();
            var currentMeters = (double) employee.GetFieldValue( "metersTravelled" );
            employee.Invoke( "Swim", 100d );
            VerifyFields( employee, new { metersTravelled = currentMeters + 100 } );
        }

        [TestMethod]
        public void TestInvokeBaseClassMethods()
        {
            var employee = EmployeeType.CreateInstance();
            var currentMeters = (double) employee.GetFieldValue( "metersTravelled" );
            employee.Invoke( "Walk", 100d );
            VerifyFields( employee, new { metersTravelled = currentMeters + 100 } );
        }

        [TestMethod]
        public void TestInvokeStaticMethod()
        {
            RunWith( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetFieldValue( "totalPeopleCreated" );
                   Assert.AreEqual( totalPeopleCreated, type.Invoke( "GetTotalPeopleCreated" ) );
               } );
        }

        [TestMethod]
        public void TestInvokeStaticMethodViaMethodInfo()
        {
            RunWith( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetFieldValue( "totalPeopleCreated" );
                   Assert.AreEqual( totalPeopleCreated,
                                    type.Method( "GetTotalPeopleCreated", Flags.StaticCriteria ).Invoke() );
               } );
        }

        [TestMethod]
        public void TestInvokeStaticMethodsWithArgument()
        {
            RunWith( ( Type type ) =>
               {
                   var totalPeopleCreated = (int) type.GetFieldValue( "totalPeopleCreated" );
                   Assert.AreEqual( totalPeopleCreated + 20, type.Invoke( "AdjustTotalPeopleCreated", 20 ) );
               } );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingMethodException) )]
        public void TestInvokeNonExistentInstanceMethod()
        {
            RunWith( ( object person ) => person.Invoke( "not_exist" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingMethodException) )]
        public void TestInvokeNonExistentStaticMethod()
        {
            RunWith( ( Type type ) => type.Invoke( "not_exist" ) );
        }
    }
}