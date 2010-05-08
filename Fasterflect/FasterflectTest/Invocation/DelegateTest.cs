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
using FasterflectTest.SampleModel.People;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class DelegateTest : BaseInvocationTest
    {
        [TestMethod]
        public void TestDelegateRetrievalMethodsReturnCorrectDelegateType()
        {
            RunWith( ( Type type ) =>
               {
                   var funcs = new Func<Delegate>[]
                               {
                                   () => type.MakeArrayType().DelegateForGetElement(),
                                   () => type.MakeArrayType().DelegateForSetElement(),
                                   () => type.DelegateForCreateInstance(),
                                   () => type.DelegateForCreateInstance( new[] { typeof(string), typeof(int) } ),
                                   () =>
                                   type.DelegateForCallMethod( "AdjustTotalPeopleCreated", new[] { typeof(int) } ),
                                   () => type.DelegateForCallMethod( "GetTotalPeopleCreated" ),
                                   () => type.DelegateForSetFieldValue( "totalPeopleCreated" ),
                                   () => type.DelegateForGetFieldValue( "totalPeopleCreated" ),
                                   () => type.DelegateForSetPropertyValue( "TotalPeopleCreated" ),
                                   () => type.DelegateForGetPropertyValue( "TotalPeopleCreated" ),
                                   () => type.DelegateForGetIndexer( new[] { typeof(string) } ),
                                   () =>
                                   type.DelegateForSetIndexer( new[]
                                                               {
                                                                   typeof(string),
                                                                   type == typeof(Person) ? type : typeof(PersonStruct?)
                                                               } )
                                   ,
                                   () => type.DelegateForGetIndexer( new[] { typeof(string), typeof(int) } ),
                                   () => type.DelegateForSetFieldValue( "name" ),
                                   () => type.DelegateForGetFieldValue( "name" ),
                                   () => type.DelegateForSetPropertyValue( "Age" ),
                                   () => type.DelegateForGetPropertyValue( "Age" ),
                                   () => type.DelegateForCallMethod( "Walk", new[] { typeof(double) } ),
                                   () =>
                                   type.DelegateForCallMethod( "Walk",
                                                           new[] { typeof(double), typeof(double).MakeByRefType() } ),
                               };
                   var types = new[]
                               {
                                   typeof(ArrayElementGetter),
                                   typeof(ArrayElementSetter),
                                   typeof(ConstructorInvoker),
                                   typeof(ConstructorInvoker),
                                   typeof(MethodInvoker),
                                   typeof(MethodInvoker),
                                   typeof(MemberSetter),
                                   typeof(MemberGetter),
                                   typeof(MemberSetter),
                                   typeof(MemberGetter),
                                   typeof(MethodInvoker),
                                   typeof(MethodInvoker),
                                   typeof(MethodInvoker),
                                   typeof(MemberSetter),
                                   typeof(MemberGetter),
                                   typeof(MemberSetter),
                                   typeof(MemberGetter),
                                   typeof(MethodInvoker),
                                   typeof(MethodInvoker)
                               };

                   for( int i = 0; i < funcs.Length; i++ )
                   {
                       var result = funcs[ i ]();
                       Assert.IsNotNull( result );
                       Assert.IsInstanceOfType( result, types[ i ] );
                   }
               } );
        }
    }
}