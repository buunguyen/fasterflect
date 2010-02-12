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
        	Flags flags = Flags.InstanceCriteria | Flags.IgnoreCase;

			AnimalInstanceMemberNames.Select( s => s.ToUpper() ).Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNotNull );
			LionInstanceMemberNames.Select( s => s.ToUpper() ).Select( s => typeof(Lion).Member( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestMemberInstanceDeclaredOnly()
        {
        	Flags flags = Flags.InstanceCriteria | Flags.DeclaredOnly;
			
			AnimalInstanceMemberNames.Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNotNull );
			LionDeclaredInstanceMemberNames.Select( s => typeof(Lion).Member( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestMemberStatic()
        {
        	Flags flags = Flags.StaticCriteria;
			
			AnimalInstanceMemberNames.Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNull );

			AnimalStaticMemberNames.Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticMemberNames.Select( s => typeof(Lion).Member( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestMemberStaticDeclaredOnly()
        {
        	Flags flags = Flags.StaticCriteria | Flags.DeclaredOnly;
			
			AnimalStaticMemberNames.Select( s => typeof(Animal).Member( s, flags ) ).ForEach( Assert.IsNotNull );
			AnimalStaticMemberNames.Select( s => typeof(Lion).Member( s, flags ) ).ForEach( Assert.IsNull );
        }
		#endregion

		#region Multiple Members
        [TestMethod]
		public void TestMembersInstance()
        {
			IList<MemberInfo> members = typeof(object).Members( Flags.InstanceCriteria );
			Assert.IsNotNull( members );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Animal).Members( Flags.InstanceCriteria );
			Assert.AreEqual( AnimalInstanceMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalInstanceMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstanceMemberTypes, members.Select( m => m.MemberType ).ToArray() );

			members = typeof(Mammal).Members( Flags.InstanceCriteria );
			Assert.AreEqual( AnimalInstanceMemberNames.Length + MammalDeclaredInstanceMemberNames.Length, members.Count );

			members = typeof(Lion).Members( Flags.InstanceCriteria );
			Assert.AreEqual( LionInstanceMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( LionInstanceMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( LionInstanceMemberTypes, members.Select( m => m.MemberType ).ToArray() );
        }

        [TestMethod]
		public void TestMembersInstanceWithDeclaredOnlyFlag()
        {
			IList<MemberInfo> members = typeof(object).Members( Flags.InstanceCriteria | Flags.DeclaredOnly );
			Assert.IsNotNull( members );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Animal).Members( Flags.InstanceCriteria | Flags.DeclaredOnly );
			Assert.AreEqual( AnimalInstanceMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalInstanceMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalInstanceMemberTypes, members.Select( m => m.MemberType ).ToArray() );

			members = typeof(Mammal).Members( Flags.InstanceCriteria | Flags.DeclaredOnly );
			Assert.AreEqual( MammalDeclaredInstanceMemberNames.Length, members.Count );

			members = typeof(Lion).Members( Flags.InstanceCriteria | Flags.DeclaredOnly );
			Assert.AreEqual( LionDeclaredInstanceMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( LionDeclaredInstanceMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( LionDeclaredInstanceMemberTypes, members.Select( m => m.MemberType ).ToArray() );
        }

        [TestMethod]
		public void TestMembersStatic()
        {
			IList<MemberInfo> members = typeof(object).Members( Flags.StaticCriteria );
			Assert.IsNotNull( members );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Animal).Members( Flags.StaticCriteria );
			Assert.AreEqual( AnimalStaticMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalStaticMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticMemberTypes, members.Select( m => m.MemberType ).ToArray() );

			members = typeof(Lion).Members( Flags.StaticCriteria );
			Assert.AreEqual( AnimalStaticMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalStaticMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticMemberTypes, members.Select( m => m.MemberType ).ToArray() );
       }

        [TestMethod]
		public void TestMembersStaticWithDeclaredOnlyFlag()
        {
			IList<MemberInfo> members = typeof(object).Members( Flags.StaticCriteria | Flags.DeclaredOnly );
			Assert.IsNotNull( members );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Animal).Members( Flags.StaticCriteria | Flags.DeclaredOnly );
			Assert.AreEqual( AnimalStaticMemberNames.Length, members.Count );
			CollectionAssert.AreEquivalent( AnimalStaticMemberNames, members.Select( m => m.Name ).ToArray() );
			CollectionAssert.AreEquivalent( AnimalStaticMemberTypes, members.Select( m => m.MemberType ).ToArray() );

			members = typeof(Mammal).Members( Flags.StaticCriteria | Flags.DeclaredOnly );
			Assert.AreEqual( 0, members.Count );

			members = typeof(Lion).Members( Flags.StaticCriteria | Flags.DeclaredOnly );
			Assert.AreEqual( 0, members.Count );
        }
		#endregion
 	}
}
