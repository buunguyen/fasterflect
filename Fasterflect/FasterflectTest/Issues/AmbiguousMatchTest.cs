using System;
using System.Collections;
using System.Linq;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Issues
{
    [TestClass]
    public class AmbiguousMatchTest
	{
		#region Sample Classes
	    private class Foo
	    {
			public object Property { get; set; }
	    }
	    private class Bar : Foo
	    {
			public new string Property { get; set; }
	    }
	    #endregion

		[TestMethod]
        public void Test_PropertyLookupWithNameAndEXHFlagShouldNotThrowAmbiguousMatchException()
		{
			var propertyInfo = typeof(Bar).Property( "Property", Flags.InstanceAnyVisibility | Flags.ExcludeHiddenMembers );
			Assert.IsNotNull( propertyInfo );
			Assert.AreEqual( typeof(Bar), propertyInfo.DeclaringType );
		}

		[TestMethod]
        public void Test_PropertiesLookupWithNameAndEXHFlagShouldFindSingleResult()
		{
			var propertyInfo = typeof(Bar).Properties( Flags.InstanceAnyVisibility | Flags.ExcludeHiddenMembers, "Property" ).Single();
			Assert.IsNotNull( propertyInfo );
			Assert.AreEqual( typeof(Bar), propertyInfo.DeclaringType );
		}
    }
}
