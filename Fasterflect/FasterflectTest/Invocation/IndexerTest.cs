using System;
using System.Linq;
using Fasterflect;
using FasterflectTest.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class IndexerTest : BaseInvocationTest
    {
        [TestMethod()]
        public void TestInvokeOneArgIndexer()
        {
            InvokeIndexer( ( person, sample ) => person.GetIndexer( sample[ 0 ] ) );
        }

        [TestMethod()]
        public void TestInvokeTwoArgsIndexer()
        {
            InvokeIndexer( ( person, sample ) => person.GetIndexer( sample[ 0 ], sample[ 1 ] ) );
        }

        private static void InvokeIndexer( Func<object, object[], object> getIndexerAction )
        {
            _( ( personBase, elementType, elementTypeNullable ) =>
               {
                   var people = new[] { new object[] { "John", 10 }, new object[] { "Jane", 20 } };
                   people.ForEach( sample =>
                                   {
                                       var person = elementType.CreateInstance( sample[ 0 ], sample[ 1 ] );
                                       var name = person.WrapIfValueType().GetFieldValue( "name" );
                                       personBase.SetIndexer( new[] { typeof(string), elementTypeNullable }, name,
                                                              person );
                                   } );
                   people.ForEach( sample =>
                                   {
                                       var person = getIndexerAction( personBase, sample );
                                       VerifyFields( person.WrapIfValueType(),
                                                     new { name = sample[ 0 ], age = sample[ 1 ] } );
                                   } );
               } );
        }

        private static void _( Action<object, Type, Type> assertionAction )
        {
            Types.Select( t =>
                          {
                              var emptyDictionary = t.Field( "friends" ).FieldType.CreateInstance();
                              var person = t.CreateInstance().WrapIfValueType();
                              person.SetFieldValue( "friends", emptyDictionary );
                              return person;
                          } ).ForEach(
                                         person =>
                                         assertionAction(
                                                            person,
                                                            !person.IsWrapped() ? typeof(Person) : typeof(PersonStruct),
                                                            person is Person ? typeof(Person) : typeof(PersonStruct?) ) );
        }
    }
}