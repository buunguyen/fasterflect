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
    public class MethodTest : BaseLookupTest
	{
        #region Single Method
        [TestMethod]
		public void TestFindInstanceMethod()
        {
			MethodInfo method = typeof(object).Method( "ID" );
			Assert.IsNull( method );

			AnimalInstanceMethodNames.Select( s => typeof(Animal).Method( s ) ).ForEach( Assert.IsNotNull );
			SnakeInstanceMethodNames.Select( s => typeof(Snake).Method( s ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFindMethodInstanceIgnoreCase()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.IgnoreCase;

			AnimalInstanceMethodNames.Select( s => s.ToLower() ).Select( s => typeof(Animal).Method( s ) ).ForEach( Assert.IsNull );
			AnimalInstanceMethodNames.Select( s => s.ToLower() ).Select( s => typeof(Animal).Method( s, flags ) ).ForEach( Assert.IsNotNull );

			ReptileInstanceMethodNames.Select( s => s.ToLower() ).Select( s => typeof(Reptile).Method( s ) ).ForEach( Assert.IsNull );
			ReptileInstanceMethodNames.Select( s => s.ToLower() ).Select( s => typeof(Reptile).Method( s, flags ) ).ForEach( Assert.IsNotNull );

			SnakeInstanceMethodNames.Select( s => s.ToLower() ).Select( s => typeof(Snake).Method( s ) ).ForEach( Assert.IsNull );
			SnakeInstanceMethodNames.Select( s => s.ToLower() ).Select( s => typeof(Snake).Method( s, flags ) ).ForEach( Assert.IsNotNull );
        }

        [TestMethod]
		public void TestFindMethodInstanceWithExcludeBackingMembers()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeBackingMembers;

			AnimalInstanceMethodNames.Select( s => typeof(Animal).Method( s ) ).ForEach( Assert.IsNotNull );
			AnimalInstanceMethodNames.Where( s => s.Contains( "_" ) ).Select( s => typeof(Animal).Method( s, flags ) ).ForEach( Assert.IsNull );
			AnimalInstanceMethodNames.Where( s => ! s.Contains( "_" ) ).Select( s => typeof(Animal).Method( s, flags ) ).ForEach( Assert.IsNotNull );

			ReptileInstanceMethodNames.Select( s => typeof(Reptile).Method( s ) ).ForEach( Assert.IsNotNull );
			ReptileInstanceMethodNames.Where( s => s.Contains( "_" ) ).Select( s => typeof(Reptile).Method( s, flags ) ).ForEach( Assert.IsNull );
			ReptileInstanceMethodNames.Where( s => ! s.Contains( "_" ) ).Select( s => typeof(Reptile).Method( s, flags ) ).ForEach( Assert.IsNotNull );

			SnakeInstanceMethodNames.Select( s => typeof(Snake).Method( s ) ).ForEach( Assert.IsNotNull );
			SnakeInstanceMethodNames.Where( s => s.Contains( "_" ) ).Select( s => typeof(Snake).Method( s, flags ) ).ForEach( Assert.IsNull );
			SnakeInstanceMethodNames.Where( s => ! s.Contains( "_" ) ).Select( s => typeof(Snake).Method( s, flags ) ).ForEach( Assert.IsNotNull );
        }

		//[TestMethod]
		//public void TestFindMethodInstanceWithAbstract()
		//{
		//    Flags flags = Flags.InstanceAnyVisibility | Flags.Abstract;

		//    Assert.IsNull( typeof(Reptile).Method( "Move", Flags.InstanceAnyVisibility, new [] { typeof(double) }, null ) );
		//    Assert.IsNotNull( typeof(Reptile).Method( "Move", flags, new [] { typeof(double) }, null ) );
		//}

        [TestMethod]
		public void TestFindMethodInstanceWithParameterMatch()
        {
			// BindingFlags.ParameterMatch should be applied automatically if parameter types are supplied
        	Flags flags = Flags.InstanceAnyVisibility;

			Assert.IsNull( typeof(Snake).Method( "Bite", flags, new Type[] {} ) );
			Assert.IsNull( typeof(Snake).Method( "Bite", flags, new [] { typeof(object) } ) );
			Assert.IsNotNull( typeof(Snake).Method( "Bite", flags, null ) ); // should ignore flag when null is passed
			Assert.IsNotNull( typeof(Snake).Method( "Bite", flags, new [] { typeof(Animal) } ) );
			Assert.IsNotNull( typeof(Snake).Method( "Bite", flags, new [] { typeof(Mammal) } ) );
			Assert.IsNotNull( typeof(Snake).Method( "Bite", flags, new [] { typeof(Lion) } ) );
        }

        [TestMethod]
		public void TestFindMethodInstanceWithExactBinding()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.ExactBinding;

			Assert.IsNull( typeof(Snake).Method( "Bite", flags, new Type[] {} ) );
			Assert.IsNull( typeof(Snake).Method( "Bite", flags, new [] { typeof(object) } ) );
			Assert.IsNotNull( typeof(Snake).Method( "Bite", flags, null ) ); // should ignore flag when null is passed
			Assert.IsNotNull( typeof(Snake).Method( "Bite", flags, new [] { typeof(Animal) } ) );
			Assert.IsNull( typeof(Snake).Method( "Bite", flags, new [] { typeof(Mammal) } ) );
			Assert.IsNull( typeof(Snake).Method( "Bite", flags, new [] { typeof(Lion) } ) );
        }

        [TestMethod]
		public void TestFindMethodInstanceWithDeclaredOnly()
        {
        	BindingFlags flags = Flags.InstanceAnyVisibility | BindingFlags.DeclaredOnly;
			
			AnimalInstanceMethodNames.Select( s => typeof(Animal).Method( s, flags ) ).ForEach( Assert.IsNotNull );
			ReptileDeclaredInstanceMethodNames.Select( s => typeof(Reptile).Method( s, flags ) ).ForEach( Assert.IsNotNull );
			ReptileInstanceMethodNames.Where( s => ! ReptileDeclaredInstanceMethodNames.Contains( s ) ).Select( s => typeof(Reptile).Method( s, flags ) ).ForEach( Assert.IsNull );
			SnakeDeclaredInstanceMethodNames.Select( s => typeof(Snake).Method( s, flags ) ).ForEach( Assert.IsNotNull );
			SnakeInstanceMethodNames.Where( s => ! SnakeDeclaredInstanceMethodNames.Contains( s ) ).Select( s => typeof(Snake).Method( s, flags ) ).ForEach( Assert.IsNull );
        }
		#endregion

		#region Multiple Methods
        [TestMethod]
		public void TestFindMethodsExcludesObjectMembers()
        {
			var methods = typeof(object).Methods();
			Assert.IsNotNull( methods );
			Assert.AreEqual( 0, methods.Count );
        }

        [TestMethod]
		public void TestFindMethodsInstance()
        {
			var methods = typeof(Animal).Methods();
			CollectionAssert.AreEquivalent( AnimalInstanceMethodNames, methods.Select( m => m.Name ).ToList() );

			methods = typeof(Reptile).Methods();
			CollectionAssert.AreEquivalent( ReptileInstanceMethodNames, methods.Select( m => m.Name ).ToList() );

			methods = typeof(Snake).Methods();
			CollectionAssert.AreEquivalent( SnakeInstanceMethodNames, methods.Select( m => m.Name.Contains( "." ) ? m.Name.Substring( m.Name.LastIndexOf( "." )+1 ) : m.Name ).ToList() );
        }

        [TestMethod]
		public void TestFindMethodsInstanceWithDeclaredOnly()
        {
			var methods = typeof(Animal).Methods( Flags.InstanceAnyVisibility | BindingFlags.DeclaredOnly );
			CollectionAssert.AreEquivalent( AnimalInstanceMethodNames, methods.Select( m => m.Name ).ToList() );

			methods = typeof(Reptile).Methods( Flags.InstanceAnyVisibility | BindingFlags.DeclaredOnly );
			CollectionAssert.AreEquivalent( ReptileDeclaredInstanceMethodNames, methods.Select( m => m.Name ).ToList() );

			methods = typeof(Snake).Methods( Flags.InstanceAnyVisibility | BindingFlags.DeclaredOnly );
			CollectionAssert.AreEquivalent( SnakeDeclaredInstanceMethodNames, methods.Select( m => m.Name.Contains( "." ) ? m.Name.Substring( m.Name.LastIndexOf( "." )+1 ) : m.Name ).ToList() );
        }

        [TestMethod]
		public void TestFindMethodsInstanceWithPartialNameMatch()
        {
        	Flags flags = Flags.InstanceAnyVisibility | Flags.PartialNameMatch;

			var methods = typeof(Animal).Methods( flags, "B" );
			CollectionAssert.AreEquivalent( AnimalInstanceMethodNames.Where( s => s.Contains( "B" ) ).ToList(), methods.Select( m => m.Name ).ToList() );

			methods = typeof(Reptile).Methods( flags, "et_" );
			CollectionAssert.AreEquivalent( ReptileInstanceMethodNames.Where( s => s.Contains( "et_" ) ).ToList(), methods.Select( m => m.Name ).ToList() );

			methods = typeof(Snake).Methods( flags, "get", "C" );
			CollectionAssert.AreEquivalent( SnakeInstanceMethodNames.Where( s => s.Contains( "get" ) || s.Contains( "C" ) ).ToList(), methods.Select( m => m.Name ).ToList() );

			methods = typeof(Snake).Methods( flags, "_" );
			CollectionAssert.AreEquivalent( SnakeInstanceMethodNames.Where( s => s.Contains( "_" ) ).ToList(), methods.Select( m => m.Name ).ToList() );

			methods = typeof(Snake).Methods( flags );
            CollectionAssert.AreEquivalent(SnakeInstanceMethodNames, methods.Select(m => m.Name.TrimExplicitlyImplementedName()).ToList());
        }

		[TestMethod]
		public void TestMethodsWithNameFilterList()
        {
			IList<MethodInfo> methods = typeof(object).Methods( AnimalInstanceMethodNames );
			Assert.AreEqual( 0, methods.Count );

			methods = typeof(Animal).Methods( AnimalInstanceMethodNames );
			CollectionAssert.AreEquivalent( AnimalInstanceMethodNames, methods.Select( m => m.Name ).ToArray() );

			methods = typeof(Snake).Methods( AnimalInstanceMethodNames );
			CollectionAssert.AreEquivalent( AnimalInstanceMethodNames, methods.Select( m => m.Name ).ToArray() );

			var list = AnimalInstanceMethodNames.Where( s => s.Contains( "C" ) ).ToArray();
			methods = typeof(Animal).Methods( list );
			CollectionAssert.AreEquivalent( list, methods.Select( m => m.Name ).ToArray() );

			list = AnimalInstanceMethodNames.Where( s => s.Contains( "B" ) ).ToArray();
			methods = typeof(Snake).Methods( list );
			CollectionAssert.AreEquivalent( list, methods.Select( m => m.Name ).ToArray() );
        }
		#endregion
	}
}
