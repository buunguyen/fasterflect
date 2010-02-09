using System;
using System.Reflection;
using Fasterflect;
using FasterflectTest.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class ConstructorTest : BaseInvocationTest
    {
        [TestMethod]
        public void TestInvokeNoArgCtor()
        {
            var person = PersonType.CreateInstance();
            Assert.IsNotNull( person );
        }

        [TestMethod]
        public void TestInvokeCtorWithPrimitiveArguments()
        {
            _( type =>
               {
                   var person = type.CreateInstance( "John", 10 ).WrapIfValueType();
                   VerifyFields( person, new { name = "John", age = 10 } );
               } );
        }

        [TestMethod]
        public void TestInvokeCtorWithComplexArgument()
        {
            _( type =>
               {
                   var person = type.CreateInstance( "John", 10 );
                   var copy = type.CreateInstance( person ).WrapIfValueType();
                   VerifyFields( copy, new { name = "John", age = 10 } );
               } );
        }

        [TestMethod]
        public void TestInvokeCtorWithComplexArgumentCoveriant()
        {
            var employee = EmployeeType.CreateInstance( "John", 10 );
            var copy = PersonType.CreateInstance( employee ).WrapIfValueType();
            VerifyFields( copy, new { name = "John", age = 10 } );
        }

        [TestMethod]
        public void TestInvokeCtorWithOutArgument()
        {
            _( type =>
               {
                   var arguments = new object[] { "John", 10, 0 };
                   var person = type.CreateInstance( new[] { typeof(string), typeof(int), typeof(int).MakeByRefType() },
                                                     arguments ).WrapIfValueType();
                   VerifyFields( person, new { name = "John", age = 10 } );
                   Assert.IsTrue( (int) arguments[ 2 ] > 0 );
               } );
        }

        [TestMethod]
        public void TestInvokeCtorWithArrayArgument()
        {
            var employee = EmployeeType.CreateInstance( new[] { EmployeeType.MakeArrayType() },
                                                        new[] { new Employee[0] } );
            Assert.IsNotNull( employee );
            Assert.AreEqual( 0, employee.GetPropertyValue( "Subordinates" ).GetPropertyValue( "Length" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(MissingMemberException) )]
        public void TestInvokeMissingCtor()
        {
            _( type => type.CreateInstance( "oneStringArgument" ) );
        }

        [TestMethod]
        [ExpectedException( typeof(NullReferenceException) )]
        public void TestInvokeCtorWithNullParametersTheWrongWay()
        {
            _( type => type.CreateInstance( null, 10 ) );
        }

        [TestMethod]
        public void TestInvokeCtorWithNullParametersTheRightWay()
        {
            _( type =>
               {
                   var person = type.CreateInstance( new[] { typeof(string), typeof(int) },
                                                     null, 10 ).WrapIfValueType();
                   VerifyFields( person, new { name = (string) null, age = 10 } );
               } );
        }

        [TestMethod]
        public void TestInvokeNoArgCtorViaConstructorInfo()
        {
            ConstructorInfo ctorInfo = PersonType.Constructor();
            var person = ctorInfo.CreateInstance().WrapIfValueType();
            VerifyFields( person, new { name = string.Empty, age = 0 } );
        }

        [TestMethod]
        public void TestInvokeCtorViaConstructorInfo()
        {
            _( type =>
               {
                   ConstructorInfo ctorInfo = type.Constructor( typeof(string), typeof(int) );
                   var person = ctorInfo.CreateInstance( null, 10 ).WrapIfValueType();
                   VerifyFields( person, new { name = (string) null, age = 10 } );
               } );
        }
    }
}