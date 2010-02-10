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
            //delegateMap = typeof(DelegateCache).GetFieldValue<CacheStore<CallInfo,Delegate>>("cache").GetFieldValue<IDictionary>("entries");
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
                                                       obj.SetFieldValue( "Name", "John" ),
                                                       () => obj.GetFieldValue( "Age" ) ) );
            Types.ForEach( type => ExecuteCacheTest(
                                                       () => type.SetFieldValue( "TotalPeopleCreated", 1 ),
                                                       () => type.GetFieldValue( "TotalPeopleCreated" ) ) );
        }

        [TestMethod]
        public void TestDelegateIsProperlyCachedForArrayElement()
        {
            RunWith( ( Type type ) =>
               {
                   object array = type.MakeArrayType().CreateInstance( 10 );
                   ExecuteCacheTest(
                                       () =>
                                       array.SetElement( 1,
                                                         type == typeof(Person)
                                                             ? (object) new Person()
                                                             : new PersonStruct() ),
                                       () => array.GetElement( 1 ) );
               } );
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
                                   ExecuteCacheTest( () => obj.Invoke( "Walk", 100d ),
                                                     () => obj.Invoke( "Walk",
                                                                       new[]
                                                                       {
                                                                           typeof(double), typeof(double).MakeByRefType()
                                                                       }, arguments ) ) );
            Types.ForEach( type => ExecuteCacheTest(
                                                       () => type.Invoke( "GetTotalPeopleCreated" ),
                                                       () => type.Invoke( "AdjustTotalPeopleCreated", 10 ) ) );
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