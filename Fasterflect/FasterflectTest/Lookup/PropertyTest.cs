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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FasterflectTest.SampleModel.Animals;

namespace FasterflectTest.Lookup
{
    [TestClass]
    public class PropertyTest : BaseLookupTest
	{
        #region Single Property
        [TestMethod]
		public void TestPropertyInstance()
        {
			PropertyInfo property = typeof(object).Property( "ID" );
			Assert.IsNull( property );

			AnimalInstancePropertyNames.Select( s => typeof(Animal).Property( s ) ).ForEach( Assert.IsNotNull );
			LionInstancePropertyNames.Select( s => typeof(Lion).Property( s ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestPropertyInstanceIgnoreCase()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.IgnoreCase;

			AnimalInstancePropertyNames.Select( s => s.ToLower() ).Select( s => typeof(Animal).Property( s ) ).ForEach( Assert.IsNull );
			AnimalInstancePropertyNames.Select( s => s.ToLower() ).Select( s => typeof(Animal).Property( s, flags ) ).ForEach( Assert.IsNotNull );

			LionInstancePropertyNames.Select( s => s.ToLower() ).Select( s => typeof(Lion).Property( s ) ).ForEach( Assert.IsNull );
			LionInstancePropertyNames.Select( s => s.ToLower() ).Select( s => typeof(Lion).Property( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestPropertyInstanceDeclaredOnly()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.DeclaredOnly;
			
			AnimalInstancePropertyNames.Select( s => typeof(Animal).Property( s, flags ) ).ForEach( Assert.IsNotNull );
			LionDeclaredInstancePropertyNames.Select( s => typeof(Lion).Property( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestPropertyByPartialName()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.PartialNameMatch;

        	var expectedName = AnimalInstancePropertyNames.Where( s => s.Contains( "C" ) ).First();
			var property = typeof(Animal).Property( "C", flags );
			Assert.IsNotNull( property );
        	Assert.AreEqual( expectedName, property.Name );

        	expectedName = AnimalInstancePropertyNames.Where( s => s.Contains( "B" ) ).First();
			property = typeof(Animal).Property( "B", flags );
			Assert.IsNotNull( property );
        	Assert.AreEqual( expectedName, property.Name );
        }

        [TestMethod]
		public void TestPropertyWithExcludeExplicitlyImplemented()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeExplicitlyImplemented;

			// using explicit name
			var property = typeof(Giraffe).Property( "FasterflectTest.SampleModel.Animals.Interfaces.ISwim.SwimDistance" );
			Assert.IsNotNull( property );
			property = typeof(Giraffe).Property( "FasterflectTest.SampleModel.Animals.Interfaces.ISwim.SwimDistance", flags );
			Assert.IsNull( property );

			// using short name
			property = typeof(Giraffe).Property( "SwimDistance", Flags.InstanceAnyVisibility | Flags.TrimExplicitlyImplemented );
			Assert.IsNotNull( property );
			property = typeof(Giraffe).Property( "SwimDistance", flags | Flags.TrimExplicitlyImplemented );
			Assert.IsNull( property );
        }

        [TestMethod]
		public void TestPropertyStatic()
        {
        	Flags flags = Flags.StaticAnyVisibility;
			
			AnimalInstancePropertyNames.Select( s => typeof(Animal).Property( s, flags ) ).ForEach( Assert.IsNull );

			AnimalStaticPropertyNames.Select( s => typeof(Animal).Property( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticPropertyNames.Select( s => typeof(Lion).Property( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestPropertyStaticDeclaredOnly()
        {
        	Flags flags = Flags.StaticAnyVisibility | Flags.DeclaredOnly;
			
			AnimalStaticPropertyNames.Select( s => typeof(Animal).Property( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticPropertyNames.Select( s => typeof(Lion).Property( s, flags ) ).ForEach( Assert.IsNull );
        }
		#endregion

		#region Multiple Properties
        [TestMethod]
		public void TestPropertiesInstance()
        {
			IList<PropertyInfo> properties = typeof(object).Properties();
			Assert.IsNotNull( properties );
			Assert.AreEqual( 0, properties.Count );

			properties = typeof(Animal).Properties();
			CollectionAssert.AreEquivalent( AnimalInstancePropertyNames, properties.Select( p => p.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstancePropertyTypes, properties.Select( p => p.PropertyType ).ToArray() );

			properties = typeof(Mammal).Properties();
			Assert.AreEqual( AnimalInstanceFieldNames.Length, properties.Count );

			properties = typeof(Lion).Properties();
			CollectionAssert.AreEquivalent( LionInstancePropertyNames, properties.Select( p => p.Name ).ToArray() );
			CollectionAssert.AreEquivalent( LionInstancePropertyTypes, properties.Select( p => p.PropertyType ).ToArray() );
        }

        [TestMethod]
		public void TestPropertiesInstanceWithDeclaredOnlyFlag()
        {
			IList<PropertyInfo> properties = typeof(object).Properties( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			Assert.IsNotNull( properties );
			Assert.AreEqual( 0, properties.Count );

			properties = typeof(Animal).Properties( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			CollectionAssert.AreEquivalent( AnimalInstancePropertyNames, properties.Select( p => p.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstancePropertyTypes, properties.Select( p => p.PropertyType ).ToArray() );

			properties = typeof(Mammal).Properties( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( 0, properties.Count );

			properties = typeof(Lion).Properties( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			CollectionAssert.AreEquivalent( LionDeclaredInstancePropertyNames, properties.Select( p => p.Name ).ToArray() );
			CollectionAssert.AreEquivalent( LionDeclaredInstancePropertyTypes, properties.Select( p => p.PropertyType ).ToArray() );
        }

        [TestMethod]
		public void TestPropertiesStatic()
        {
			IList<PropertyInfo> properties = typeof(object).Properties( Flags.StaticAnyVisibility );
			Assert.IsNotNull( properties );
			Assert.AreEqual( 0, properties.Count );

			properties = typeof(Animal).Properties( Flags.StaticAnyVisibility );
			CollectionAssert.AreEquivalent( AnimalStaticPropertyNames, properties.Select( p => p.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticPropertyTypes, properties.Select( p => p.PropertyType ).ToArray() );

			properties = typeof(Mammal).Properties( Flags.StaticAnyVisibility );
			Assert.AreEqual( AnimalStaticPropertyNames.Length, properties.Count );

			properties = typeof(Lion).Properties( Flags.StaticAnyVisibility );
			CollectionAssert.AreEquivalent( AnimalStaticPropertyNames, properties.Select( p => p.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticPropertyTypes, properties.Select( p => p.PropertyType ).ToArray() );
       }

        [TestMethod]
		public void TestPropertiesStaticWithDeclaredOnlyFlag()
        {
			IList<PropertyInfo> properties = typeof(object).Properties( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.IsNotNull( properties );
			Assert.AreEqual( 0, properties.Count );

			properties = typeof(Animal).Properties( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			CollectionAssert.AreEquivalent( AnimalStaticPropertyNames, properties.Select( p => p.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticPropertyTypes, properties.Select( p => p.PropertyType ).ToArray() );

			properties = typeof(Mammal).Properties( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( 0, properties.Count );

			properties = typeof(Lion).Properties( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( 0, properties.Count );
        }
		
		[TestMethod]
		public void TestPropertiesWithNameFilterList()
        {
			IList<PropertyInfo> properties = typeof(object).Properties( AnimalInstancePropertyNames );
			Assert.AreEqual( 0, properties.Count );

			properties = typeof(Animal).Properties( AnimalInstancePropertyNames );
			CollectionAssert.AreEquivalent( AnimalInstancePropertyNames, properties.Select( p => p.Name ).ToArray() );

			properties = typeof(Lion).Properties( AnimalInstancePropertyNames );
			CollectionAssert.AreEquivalent( AnimalInstancePropertyNames, properties.Select( p => p.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstancePropertyTypes, properties.Select( p => p.PropertyType ).ToArray() );

			var list = AnimalInstancePropertyNames.Where( s => s.Contains( "C" ) ).ToArray();
			properties = typeof(Animal).Properties( list );
			CollectionAssert.AreEquivalent( list, properties.Select( p => p.Name ).ToArray() );

			list = AnimalInstancePropertyNames.Where( s => s.Contains( "B" ) ).ToArray();
			properties = typeof(Lion).Properties( list );
			CollectionAssert.AreEquivalent( list, properties.Select( p => p.Name ).ToArray() );
        }
		#endregion
	}
}
