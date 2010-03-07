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
using System.Collections;
using Fasterflect;
using Fasterflect.Caching;
using Fasterflect.Emitter;
using FasterflectTest.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class DelegateCacheTest : BaseInvocationTest
    {
        private readonly object[] objectTypes = {
                                                    typeof(Person).CreateInstance(),
                                                    typeof(PersonStruct).CreateInstance().WrapIfValueType()
                                                };

        private IDictionary delegateMap;

        [TestInitialize]
        public void TestInitialize()
        {
            delegateMap = (IDictionary)typeof(BaseEmitter).GetFieldValue("cache").GetFieldValue("entries");
            delegateMap.Clear();
        }

        private void ExecuteCacheTest( params Action[] actions )
        {
            int delCount = delegateMap.Count;
            foreach( var action in actions )
            {
                for( int i = 0; i < 20; i++ )
                {
                    action();
                }
                Assert.AreEqual( ++delCount, delegateMap.Count );
            }
        }

        [TestMethod]
        public void TestDelegateIsProperlyCachedForFields()
        {
            objectTypes.ForEach(
                                   obj =>
                                   ExecuteCacheTest(
                                                       () => obj.SetFieldValue( "name", "John" ),
                                                       () => obj.GetFieldValue( "age" ) ) );
            Types.ForEach( type => ExecuteCacheTest(
                                                       () => type.SetFieldValue( "totalPeopleCreated", 1 ),
                                                       () => type.GetFieldValue( "totalPeopleCreated" ) ) );
        }

        [TestMethod]
        public void TestDelegateIsProperlyCachedForProperties()
        {
            objectTypes.ForEach(
                                   obj =>
                                   ExecuteCacheTest(
                                                       () =>
                                                       obj.SetPropertyValue( "Name", "John" ),
                                                       () => obj.GetPropertyValue( "Age" ) ) );
            Types.ForEach( type => ExecuteCacheTest(
                                                       () => type.SetPropertyValue( "TotalPeopleCreated", 1 ),
                                                       () => type.GetPropertyValue( "TotalPeopleCreated" ) ) );
        }

        [TestMethod]
        public void TestDelegateIsProperlyCachedForConstructors()
        {
            RunWith( ( Type type ) => ExecuteCacheTest(
                                                    () => type.CreateInstance(),
                                                    () => type.CreateInstance( "John", 10 ) ) );
        }

        [TestMethod]
        public void TestDelegateIsProperlyCachedForMethods()
        {
            var arguments = new object[] { 100d, null };
            objectTypes.ForEach(
                                   obj =>
                                   ExecuteCacheTest( () => obj.CallMethod( "Walk", 100d ),
                                                     () => obj.CallMethod( "Walk",
                                                                       new[]
                                                                       {
                                                                           typeof(double), typeof(double).MakeByRefType()
                                                                       }, arguments ) ) );
            Types.ForEach( type => ExecuteCacheTest(
                                                       () => type.CallMethod( "GetTotalPeopleCreated" ),
                                                       () => type.CallMethod( "AdjustTotalPeopleCreated", 10 ) ) );
        }

        [TestMethod]
        public void TestDelegateIsProperlyCachedForIndexers()
        {
            for( int i = 0; i < Types.Length; i++ )
            {
                var emptyDictionary = Types[ i ].Field( "friends" ).FieldType.CreateInstance();
                objectTypes[ i ].SetFieldValue( "friends", emptyDictionary );
                ExecuteCacheTest( () => objectTypes[ i ].SetIndexer(
                                                                       new[]
                                                                       {
                                                                           typeof(string),
                                                                           Types[ i ] == typeof(Person)
                                                                               ? typeof(Person)
                                                                               : typeof(PersonStruct?)
                                                                       },
                                                                       "John", null ),
                                  () => objectTypes[ i ].GetIndexer( "John" ),
                                  () => objectTypes[ i ].GetIndexer( "John", 10 ) );
            }
        }
    }
}