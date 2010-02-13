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
        	BindingFlags flags = Flags.InstanceCriteria | BindingFlags.IgnoreCase;

			AnimalInstanceFieldNames.Select( s => s.ToUpper() ).Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNotNull );
			LionInstanceFieldNames.Select( s => s.ToUpper() ).Select( s => typeof(Lion).Field( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFieldInstanceWithFieldType()
        {
        	BindingFlags flags = Flags.InstanceCriteria;

			AnimalInstanceFieldNames.Select( s => typeof(Animal).Field( s, flags, AnimalInstanceFieldTypes[ Array.IndexOf( AnimalInstanceFieldNames, s ) ] ) ).ForEach( Assert.IsNotNull );
			LionInstanceFieldNames.Select( s => typeof(Lion).Field( s, flags, LionInstanceFieldTypes[ Array.IndexOf( LionInstanceFieldNames, s ) ] ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFieldInstanceDeclaredOnly()
        {
        	BindingFlags flags = Flags.InstanceCriteria | BindingFlags.DeclaredOnly;
			
			AnimalInstanceFieldNames.Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNotNull );
			LionDeclaredInstanceFieldNames.Select( s => typeof(Lion).Field( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFieldStatic()
        {
        	Flags flags = Flags.StaticCriteria;
			
			AnimalInstanceFieldNames.Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNull );

			AnimalStaticFieldNames.Select( s => typeof(Animal).Field( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticFieldNames.Select( s => typeof(Lion).Field( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFieldStaticDeclaredOnly()
        {
        	BindingFlags flags = Flags.StaticCriteria | BindingFlags.DeclaredOnly;
			
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
			Assert.AreEqual( AnimalInstanceFieldNames.Length, fields.Count );
			Assert.IsTrue( AnimalInstanceFieldNames.SequenceEqual( fields.Select( f => f.Name ) ) );
			Assert.IsTrue( AnimalInstanceFieldTypes.SequenceEqual( fields.Select( f => f.FieldType ) ) );

			fields = typeof(Mammal).Fields();
			Assert.AreEqual( AnimalInstanceFieldNames.Length, fields.Count );

			fields = typeof(Lion).Fields();
			Assert.AreEqual( LionInstanceFieldNames.Length, fields.Count );
			Assert.IsTrue( LionInstanceFieldNames.SequenceEqual( fields.Select( f => f.Name ) ) );
			Assert.IsTrue( LionInstanceFieldTypes.SequenceEqual( fields.Select( f => f.FieldType ) ) );
        }

        [TestMethod]
		public void TestFieldsInstanceWithDeclaredOnlyFlag()
        {
			IList<FieldInfo> fields = typeof(object).Fields( Flags.InstanceCriteria | BindingFlags.DeclaredOnly );
			Assert.IsNotNull( fields );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Animal).Fields( Flags.InstanceCriteria | BindingFlags.DeclaredOnly );
			Assert.AreEqual( AnimalInstanceFieldNames.Length, fields.Count );
			Assert.IsTrue( AnimalInstanceFieldNames.SequenceEqual( fields.Select( f => f.Name ) ) );
			Assert.IsTrue( AnimalInstanceFieldTypes.SequenceEqual( fields.Select( f => f.FieldType ) ) );

			fields = typeof(Mammal).Fields( Flags.InstanceCriteria | BindingFlags.DeclaredOnly );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Lion).Fields( Flags.InstanceCriteria | BindingFlags.DeclaredOnly );
			Assert.AreEqual( LionDeclaredInstanceFieldNames.Length, fields.Count );
			Assert.IsTrue( LionDeclaredInstanceFieldNames.SequenceEqual( fields.Select( f => f.Name ) ) );
			Assert.IsTrue( LionDeclaredInstanceFieldTypes.SequenceEqual( fields.Select( f => f.FieldType ) ) );
        }

        [TestMethod]
		public void TestFieldsStatic()
        {
			IList<FieldInfo> fields = typeof(object).Fields( Flags.StaticCriteria );
			Assert.IsNotNull( fields );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Animal).Fields( Flags.StaticCriteria );
			Assert.AreEqual( AnimalStaticFieldNames.Length, fields.Count );
			Assert.IsTrue( AnimalStaticFieldNames.SequenceEqual( fields.Select( f => f.Name ) ) );
			Assert.IsTrue( AnimalStaticFieldTypes.SequenceEqual( fields.Select( f => f.FieldType ) ) );

			fields = typeof(Lion).Fields( Flags.StaticCriteria );
			Assert.AreEqual( AnimalStaticFieldNames.Length, fields.Count );
			Assert.IsTrue( AnimalStaticFieldNames.SequenceEqual( fields.Select( f => f.Name ) ) );
			Assert.IsTrue( AnimalStaticFieldTypes.SequenceEqual( fields.Select( f => f.FieldType ) ) );
       }

        [TestMethod]
		public void TestFieldsStaticWithDeclaredOnlyFlag()
        {
			IList<FieldInfo> fields = typeof(object).Fields( Flags.StaticCriteria | BindingFlags.DeclaredOnly );
			Assert.IsNotNull( fields );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Animal).Fields( Flags.StaticCriteria | BindingFlags.DeclaredOnly );
			Assert.AreEqual( AnimalStaticFieldNames.Length, fields.Count );
			Assert.IsTrue( AnimalStaticFieldNames.SequenceEqual( fields.Select( f => f.Name ) ) );
			Assert.IsTrue( AnimalStaticFieldTypes.SequenceEqual( fields.Select( f => f.FieldType ) ) );

			fields = typeof(Mammal).Fields( Flags.StaticCriteria | BindingFlags.DeclaredOnly );
			Assert.AreEqual( 0, fields.Count );

			fields = typeof(Lion).Fields( Flags.StaticCriteria | BindingFlags.DeclaredOnly );
			Assert.AreEqual( 0, fields.Count );
        }
		#endregion
 	}
}
