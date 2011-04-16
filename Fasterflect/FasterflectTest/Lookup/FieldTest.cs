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
using FasterflectTest.SampleModel.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Lookup
{
    [TestClass]
    public class FieldTest : BaseLookupTest
	{
        #region Single Field
        [TestMethod]
		public void TestFieldInstance()
        {
			FieldInfo field = typeof(object).Field( "id" );
			Assert.IsNull( field );

			AnimalInstanceFieldNames.Select( s => typeof(Animal).Field( s ) ).ForEach( Assert.IsNotNull );
			LionInstanceFieldNames.Select( s => typeof(Lion).Field( s ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFieldInstanceIgnoreCase()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.IgnoreCase;

			AnimalInstanceFieldNames.Select( s => s.ToUpper() ).Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNotNull );
			LionInstanceFieldNames.Select( s => s.ToUpper() ).Select( s => typeof(Lion).Field( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFieldInstanceDeclaredOnly()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.DeclaredOnly;
			
			AnimalInstanceFieldNames.Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNotNull );
			LionDeclaredInstanceFieldNames.Select( s => typeof(Lion).Field( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFieldByPartialName()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.PartialNameMatch;

        	var expectedName = AnimalInstanceFieldNames.Where( s => s.Contains( "i" ) ).First();
			var field = typeof(Animal).Field( "i", flags );
			Assert.IsNotNull( field );
        	Assert.AreEqual( expectedName, field.Name );

        	expectedName = AnimalInstanceFieldNames.Where( s => s.Contains( "bi" ) ).First();
			field = typeof(Animal).Field( "bi", flags );
			Assert.IsNotNull( field );
        	Assert.AreEqual( expectedName, field.Name );
        }

        [TestMethod]
		public void TestFieldWithPartialNameMatchAndExcludeBackingMembers()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.PartialNameMatch;

			var expectedName = AnimalInstanceFieldNames.Where( s => s.Contains( "Movement" ) ).First();
			var field = typeof(Animal).Field( "Movement", flags );
			Assert.IsNotNull( field );
        	Assert.AreEqual( expectedName, field.Name );

			field = typeof(Animal).Field( "Movement", flags | Flags.ExcludeBackingMembers );
			Assert.IsNull( field );
        }

        [TestMethod]
		public void TestFieldStatic()
        {
        	Flags flags = Flags.StaticAnyVisibility;
			
			AnimalInstanceFieldNames.Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNull );

			AnimalStaticFieldNames.Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticFieldNames.Select( s => typeof(Lion).Field( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFieldStaticDeclaredOnly()
        {
        	Flags flags = Flags.StaticAnyVisibility | Flags.DeclaredOnly;
			
			AnimalStaticFieldNames.Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticFieldNames.Select( s => typeof(Lion).Field( s, flags ) ).ForEach( Assert.IsNull );
        }
		#endregion

		#region Multiple Fields
        [TestMethod]
		public void TestFieldsInstance()
        {
			IList<FieldInfo> fields = typeof(object).Fields();
			Assert.IsNotNull( fields );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Animal).Fields();
			CollectionAssert.AreEquivalent( AnimalInstanceFieldNames, fields.Select( f => f.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstanceFieldTypes, fields.Select( f => f.FieldType ).ToArray() );

			fields = typeof(Mammal).Fields();
			Assert.AreEqual( AnimalInstanceFieldNames.Length, fields.Count );

			fields = typeof(Lion).Fields();
			CollectionAssert.AreEquivalent( LionInstanceFieldNames, fields.Select( f => f.Name ).ToArray() );
			CollectionAssert.AreEquivalent( LionInstanceFieldTypes, fields.Select( f => f.FieldType ).ToArray() );
        }

        [TestMethod]
		public void TestFieldsInstanceWithDeclaredOnlyFlag()
        {
			IList<FieldInfo> fields = typeof(object).Fields( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			Assert.IsNotNull( fields );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Animal).Fields( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			CollectionAssert.AreEquivalent( AnimalInstanceFieldNames, fields.Select( f => f.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstanceFieldTypes, fields.Select( f => f.FieldType ).ToArray() );

			fields = typeof(Mammal).Fields( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Lion).Fields( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			CollectionAssert.AreEquivalent( LionDeclaredInstanceFieldNames, fields.Select( f => f.Name ).ToArray() );
			CollectionAssert.AreEquivalent( LionDeclaredInstanceFieldTypes, fields.Select( f => f.FieldType ).ToArray() );
        }

        [TestMethod]
		public void TestFieldsStatic()
        {
			IList<FieldInfo> fields = typeof(object).Fields( Flags.StaticAnyVisibility );
			Assert.IsNotNull( fields );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Animal).Fields( Flags.StaticAnyVisibility );
			CollectionAssert.AreEquivalent( AnimalStaticFieldNames, fields.Select( f => f.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticFieldTypes, fields.Select( f => f.FieldType ).ToArray() );

			fields = typeof(Lion).Fields( Flags.StaticAnyVisibility );
			CollectionAssert.AreEquivalent( AnimalStaticFieldNames, fields.Select( f => f.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticFieldTypes, fields.Select( f => f.FieldType ).ToArray() );
       }

        [TestMethod]
		public void TestFieldsStaticWithDeclaredOnlyFlag()
        {
			IList<FieldInfo> fields = typeof(object).Fields( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.IsNotNull( fields );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Animal).Fields( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			CollectionAssert.AreEquivalent( AnimalStaticFieldNames, fields.Select( f => f.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticFieldTypes, fields.Select( f => f.FieldType ).ToArray() );

			fields = typeof(Mammal).Fields( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Lion).Fields( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( 0, fields.Count );
        }
		#endregion
 	}
}
