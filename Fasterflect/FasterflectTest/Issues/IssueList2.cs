using System;
using System.Collections;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Issues
{
    [TestClass]
    public class IssueList2
    {
        [TestMethod]
        public void ArrayListFailureDemo_Works()
        {
        	var list = typeof(ArrayList).CreateInstance();
        	var add_with_object = list.GetType().DelegateForCallMethod( "Add", typeof(object) );
        	for( int i = 0; i < 10; i++ )
        	{
        		add_with_object( list, i );
        	}
        	var size = (int) list.GetPropertyValue( "Count" );
        	Assert.AreEqual( 10, size );
        }

		// uncomment line 54 in LookupUtils to enable older code base behavior
    	[TestMethod]
        public void ArrayListFailureDemo_WorksButFailsWithOlderCodeBase()
        {
        	var list = typeof(ArrayList).CreateInstance();
			var add_with_int = list.GetType().DelegateForCallMethod( "Add", typeof(int) );
			for (int i = 0; i < 10; i++)
			{
				add_with_int( list, i );
            }
            var size = (int)list.GetPropertyValue("Count");
            Assert.AreEqual( 10, size );
        }

        [TestMethod]
        public void Issue1()
        {
            var list = typeof(ArrayList).CreateInstance();
            for (int i = 0; i < 10; i++)
            {
                list.CallMethod("Add", i);
            }
            var size = (int)list.GetPropertyValue("Count");
            Assert.AreEqual( 10, size );
        }

        class AClass
        {
            public AClass(object o)
            {
            }   
            public int Add(object i)
            {
                Console.WriteLine( "object" );
                return 1;
            }
            public int Add(int i)
            {
                Console.WriteLine( "int" );
                return 1;
            }
        }

        [TestMethod]
        public void Issue1a()
        {
            var list = typeof(AClass).CreateInstance(0);
            for (int i = 0; i < 10; i++)
            {
                list.CallMethod("Add", i);
            }
        }

        [TestMethod]
        public void Issue2()
        {
            for( int i = 0; i < 10; i++ )
            {
                var obj = typeof(AClass).CreateInstance(i);
                Assert.IsNotNull(obj);   
            }
        }
    }
}
