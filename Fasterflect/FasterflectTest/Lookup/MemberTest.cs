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
    public class MemberTest : BaseLookupTest
	{
        #region Single Member
        [TestMethod]
		public void TestMemberInstance()
        {
			MemberInfo member = typeof(object).Member( "id" );
			Assert.IsNull( member );

			AnimalInstanceMemberNames.Select( s => typeof(Animal).Member( s ) ).ForEach( Assert.IsNotNull );
			LionInstanceMemberNames.Select( s => typeof(Lion).Member( s ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestMemberInstanceIgnoreCase()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.IgnoreCase;

			AnimalInstanceMemberNames.Select( s => s.ToUpper() ).Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNotNull );
			LionInstanceMemberNames.Select( s => s.ToUpper() ).Select( s => typeof(Lion).Member( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestMemberInstanceDeclaredOnly()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.DeclaredOnly;
			
			AnimalInstanceMemberNames.Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNotNull );
			LionDeclaredInstanceMemberNames.Select( s => typeof(Lion).Member( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestMemberStatic()
        {
        	Flags flags = Flags.StaticAnyVisibility;
			
			AnimalInstanceMemberNames.Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNull );

			AnimalStaticMemberNames.Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticMemberNames.Select( s => typeof(Lion).Member( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestMemberStaticDeclaredOnly()
        {
        	Flags flags = Flags.StaticAnyVisibility | Flags.DeclaredOnly;
			
			AnimalStaticMemberNames.Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticMemberNames.Select( s => typeof(Lion).Member( s, flags ) ).ForEach( Assert.IsNull );
        }
		#endregion

		#region Multiple Members
        [TestMethod]
		public void TestMembersInstance()
        {
			IList<MemberInfo> members = typeof(object).Members( Flags.InstanceAnyVisibility );
			Assert.IsNotNull( members );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Animal).Members( Flags.InstanceAnyVisibility );
			Assert.AreEqual( AnimalInstanceMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalInstanceMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstanceMemberTypes, members.Select( m => m.MemberType ).ToArray() );

			members = typeof(Mammal).Members( Flags.InstanceAnyVisibility );
			Assert.AreEqual( AnimalInstanceMemberNames.Length + MammalDeclaredInstanceMemberNames.Length, members.Count );

			members = typeof(Lion).Members( Flags.InstanceAnyVisibility );
			Assert.AreEqual( LionInstanceMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( LionInstanceMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( LionInstanceMemberTypes, members.Select( m => m.MemberType ).ToArray() );
        }

        [TestMethod]
		public void TestMembersInstanceWithDeclaredOnlyFlag()
        {
			IList<MemberInfo> members = typeof(object).Members( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			Assert.IsNotNull( members );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Animal).Members( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( AnimalInstanceMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalInstanceMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstanceMemberTypes, members.Select( m => m.MemberType ).ToArray() );

			members = typeof(Mammal).Members( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( MammalDeclaredInstanceMemberNames.Length, members.Count );

			members = typeof(Lion).Members( Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( LionDeclaredInstanceMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( LionDeclaredInstanceMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( LionDeclaredInstanceMemberTypes, members.Select( m => m.MemberType ).ToArray() );
        }

        [TestMethod]
		public void TestMembersStatic()
        {
			IList<MemberInfo> members = typeof(object).Members( Flags.StaticAnyVisibility );
			Assert.IsNotNull( members );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Animal).Members( Flags.StaticAnyVisibility );
			Assert.AreEqual( AnimalStaticMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalStaticMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticMemberTypes, members.Select( m => m.MemberType ).ToArray() );

			members = typeof(Lion).Members( Flags.StaticAnyVisibility );
			Assert.AreEqual( AnimalStaticMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalStaticMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticMemberTypes, members.Select( m => m.MemberType ).ToArray() );
       }

        [TestMethod]
		public void TestMembersStaticWithDeclaredOnlyFlag()
        {
			IList<MemberInfo> members = typeof(object).Members( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.IsNotNull( members );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Animal).Members( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( AnimalStaticMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalStaticMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticMemberTypes, members.Select( m => m.MemberType ).ToArray() );

			members = typeof(Mammal).Members( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Lion).Members( Flags.StaticAnyVisibility | Flags.DeclaredOnly );
			Assert.AreEqual( 0, members.Count );
        }
		#endregion

		#region ExcludeBackingMembers and ExcludeHiddenMembers
		public class Person
		{
			public virtual string Name { get; protected set; }
			public virtual void Foo() {}
		}
		public class Employee : Person
		{
			public override string Name { get; protected set; }
			new public virtual void Foo() {}
			public void Foo(int foo) {}
		}
		public class Manager : Employee
		{
			public override void Foo() {}
			public void Foo(int foo, int bar) {}
		}

		[TestMethod]
		public void TestWithExcludeBackingMembers()
		{
			Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeBackingMembers;

			IList<PropertyInfo> properties = typeof(Employee).Properties( "Name" );
			Assert.AreEqual( 2, properties.Count );
			Assert.AreEqual( typeof(Employee), properties.First().DeclaringType );

			properties = typeof(Employee).Properties( flags, "Name" );
			Assert.AreEqual( 1, properties.Count );
			Assert.AreEqual( typeof(Employee), properties.First().DeclaringType );

			MemberTypes memberTypes = MemberTypes.Method | MemberTypes.Field | MemberTypes.Property;
			IList<MemberInfo> members = typeof(Employee).Members( memberTypes );
			Assert.AreEqual( 11, members.Count );
			Assert.AreEqual( typeof(Employee), members.First( m => m.Name == "Foo" ).DeclaringType );
			Assert.AreEqual( typeof(Employee), members.First( m => m.Name == "Name" ).DeclaringType );

			members = typeof(Employee).Members( memberTypes, flags );
			Assert.AreEqual( 3, members.Count );
			Assert.AreEqual( typeof(Employee), members.First( m => m.Name == "Name" ).DeclaringType );
		}

		[TestMethod]
		public void TestWithExcludeHiddenMembers()
		{
			Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeHiddenMembers;

			IList<PropertyInfo> properties = typeof(Manager).Properties( "Name" );
			Assert.AreEqual( 2, properties.Count );
			Assert.AreEqual( typeof(Employee), properties.First().DeclaringType );

			properties = typeof(Manager).Properties( flags, "Name" );
			Assert.AreEqual( 1, properties.Count );
			Assert.AreEqual( typeof(Employee), properties.First().DeclaringType );

			MemberTypes memberTypes = MemberTypes.Method | MemberTypes.Field | MemberTypes.Property;
			IList<MemberInfo> members = typeof(Manager).Members( memberTypes );
			Assert.AreEqual( 13, members.Count );
			Assert.AreEqual( typeof(Manager), members.First().DeclaringType );

			members = typeof(Manager).Members( memberTypes, flags );
			Assert.AreEqual( 7, members.Count );
			Assert.AreEqual( typeof(Manager), members.First().DeclaringType );

			members = typeof(Manager).Members( memberTypes, flags | Flags.ExcludeBackingMembers );
			Assert.AreEqual( 4, members.Count );
			Assert.AreEqual( typeof(Manager), members.First().DeclaringType );

		}
		#endregion

		#region Member Helpers (HasName, Type, IsReadable/IsWritable)
		[TestMethod]
		public void TestMemberHasName()
        {
			var member = typeof(Lion).Member( "lastMealTime" );
			Assert.IsNotNull( member );
			Assert.IsTrue( member.HasName( "lastMealTime" ) );
			Assert.IsTrue( member.HasName( "_lastMealTime" ) );
		}

		[TestMethod]
		public void TestMemberType()
        {
			var member = typeof(Lion).Member( "lastMealTime" );
			Assert.IsNotNull( member );
			Assert.AreEqual( typeof(DateTime), member.Type() );
		}

		[TestMethod]
		public void TestMemberIsReadableIsWritable()
        {
			// normal instance field
			var member = typeof(Lion).Member( "lastMealTime" );
			Assert.IsNotNull( member );
			Assert.IsTrue( member.IsReadable() );
			Assert.IsTrue( member.IsWritable() );
			// normal instance property
			member = typeof(Lion).Member( "Name" );
			Assert.IsNotNull( member );
			Assert.IsTrue( member.IsReadable() );
			Assert.IsTrue( member.IsWritable() );

			// const field
			member = typeof(Zoo).Member( "FirstId", Flags.StaticAnyVisibility );
			Assert.IsNotNull( member );
			Assert.IsTrue( member.IsReadable() );
			Assert.IsFalse( member.IsWritable() );
			// static field
			member = typeof(Zoo).Member( "nextId", Flags.StaticAnyVisibility );
			Assert.IsNotNull( member );
			Assert.IsTrue( member.IsReadable() );
			Assert.IsTrue( member.IsWritable() );
			// readonly instance field
			member = typeof(Zoo).Member( "name" );
			Assert.IsNotNull( member );
			Assert.IsTrue( member.IsReadable() );
			Assert.IsFalse( member.IsWritable() );
			// read-only instance property
			member = typeof(Zoo).Member( "Name" );
			Assert.IsNotNull( member );
			Assert.IsTrue( member.IsReadable() );
			Assert.IsFalse( member.IsWritable() );
			// write-only instance property
			member = typeof(Zoo).Member( "Alias" );
			Assert.IsNotNull( member );
			Assert.IsFalse( member.IsReadable() );
			Assert.IsTrue( member.IsWritable() );
		}
		#endregion
	}
}
