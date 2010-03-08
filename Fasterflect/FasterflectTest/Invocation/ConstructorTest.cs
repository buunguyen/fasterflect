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
using System.Reflection;
using Fasterflect;
using FasterflectTest.SampleModel.People;
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
        public void TestInvokeCtorWithCorrectBindingFlags()
        {
            RunWith( type => type.CreateInstance( Flags.Instance | Flags.NonPublic ) );
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void TestInvokeCtorWithIncorrectBindingFlags()
        {
            RunWith(type => type.CreateInstance(Flags.Public | Flags.Instance));
        }

        [TestMethod]
        public void TestInvokeCtorWithPrimitiveArguments()
        {
            RunWith( type =>
               {
                   var person = type.CreateInstance( "John", 10 ).WrapIfValueType();
                   VerifyFields( person, new { name = "John", age = 10 } );
               } );
        }

        [TestMethod]
        public void TestInvokeCtorWithComplexArgument()
        {
            RunWith( type =>
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
            RunWith( type =>
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
            RunWith( type => type.CreateInstance( "oneStringArgument" ) );
        }

        [TestMethod]
        public void TestInvokeCtorWithNullParametersTheRightWay()
        {
            RunWith( type =>
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
            RunWith( type =>
               {
                   ConstructorInfo ctorInfo = type.Constructor( typeof(string), typeof(int) );
                   var person = ctorInfo.CreateInstance( null, 10 ).WrapIfValueType();
                   VerifyFields( person, new { name = (string) null, age = 10 } );
               } );
        }
    }
}