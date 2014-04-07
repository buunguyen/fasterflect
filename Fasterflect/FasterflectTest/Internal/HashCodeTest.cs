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
using System.Linq;
using System.Reflection;
using Fasterflect;
using Fasterflect.Caching;
using Fasterflect.Emitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Internal
{
	[TestClass]
	public class HashCodeTest
	{
		#region Sample Classes
		public class A1
		{
			public int P1 { get; set; }
		}

		public class B1 : A1
		{
			public string P2 { get; set; }
		}

		public class A2
		{
			public int P1 { get; set; }
		}

		public class B2
		{
			public int P1 { get; set; }
			public string P2 { get; set; }
		}

	  public class C
	  {
	    public void M( A1 arg ) {}
	  }
	  #endregion

		[TestMethod]
		public void TestHashCodeUniqueness()
		{
			var types = new[] { typeof(A1), typeof(B1), typeof(A2), typeof(B2) };
			var instances = types.Select( t => t.CreateInstance() );
			Assert.AreEqual( types.Length, instances.Select( t => t.GetHashCode() ).Distinct().Count() );
		}

		[TestMethod]
		public void TestCallInfoHashCodeUniqueness()
		{
			var types = new[] { typeof(A1), typeof(B1), typeof(A2), typeof(B2) };
			var infos = types.Select( t => new CallInfo( t, Type.EmptyTypes, Flags.InstanceAnyVisibility, MemberTypes.Property, "P1", Type.EmptyTypes, null, true ) );
			Assert.AreEqual( types.Length, infos.Select( ci => ci.GetHashCode() ).Distinct().Count() );
			infos = types.Select( t => new CallInfo( t, Type.EmptyTypes, Flags.InstanceAnyVisibility, MemberTypes.Property, "P2", Type.EmptyTypes, null, true ) );
			Assert.AreEqual( types.Length, infos.Select( ci => ci.GetHashCode() ).Distinct().Count() );
		}

	  [TestMethod]
	  public void TestCallInfoEqualityForProperties()
	  {
			var types = new[] { typeof(A1), typeof(A1) };
			var infos = types.Select( t => new CallInfo( t, null, Flags.StaticInstanceAnyVisibility, MemberTypes.Property, "P1", Type.EmptyTypes, null, true ) ).ToList();
      Assert.AreEqual( infos[0].GetHashCode(), infos[1].GetHashCode() );
      Assert.AreEqual( infos[0], infos[1] );
      Assert.IsTrue( infos[0].Equals( infos[1] ) );
      Assert.IsTrue( infos[0] == infos[1] );
	  }

	  [TestMethod]
	  public void TestCallInfoEqualityForMethods()
	  {
	    var args = new [] { new A1() };
			var types = new[] { typeof(C), typeof(C) };
			var infos = types.Select( t => new CallInfo( t, null, Flags.StaticInstanceAnyVisibility, MemberTypes.Method, "M", args.ToTypeArray(), null, true ) ).ToList();
      Assert.AreEqual( infos[0].GetHashCode(), infos[1].GetHashCode() );
      Assert.AreEqual( infos[0], infos[1] );
      Assert.IsTrue( infos[0].Equals( infos[1] ) );
      Assert.IsTrue( infos[0] == infos[1] );
	  }

	  [TestMethod]
	  public void TestCache()
	  {
			var types = new[] { typeof(A1), typeof(A1) };
			var infos = types.Select( t => new CallInfo( t, null, Flags.StaticInstanceAnyVisibility, MemberTypes.Property, "P1", Type.EmptyTypes, null, true ) ).ToList();
	    var cache = new Cache<CallInfo, object>();
      infos.ForEach( ci => cache.Insert( ci, ci ) );
      Assert.AreEqual( 1, cache.Count );
	    Assert.IsNotNull( cache.Get( infos[ 0 ] ) );
	    Assert.IsNotNull( cache.Get( infos[ 1 ] ) );
      Assert.AreEqual( infos[0], cache.Get( infos[ 0 ] ) );
	  }

	  [TestMethod]
		public void TestMapCallInfoHashCodeUniqueness()
		{
			var map1 = GetMapCallInfo( typeof(A1), typeof(A2), "P1" );
			var map2 = GetMapCallInfo( typeof(A2), typeof(A1), "P1" );
			Assert.AreNotEqual( map1.GetHashCode(), map2.GetHashCode() );
			map1 = GetMapCallInfo( typeof(B1), typeof(B2), "P1" );
			map2 = GetMapCallInfo( typeof(B2), typeof(B1), "P1" );
			Assert.AreNotEqual( map1.GetHashCode(), map2.GetHashCode() );
			map1 = GetMapCallInfo( typeof(B1), typeof(B2), "P2" );
			map2 = GetMapCallInfo( typeof(B2), typeof(B1), "P2" );
			Assert.AreNotEqual( map1.GetHashCode(), map2.GetHashCode() );
			map1 = GetMapCallInfo( typeof(B1), typeof(B2), "P1", "P2" );
			map2 = GetMapCallInfo( typeof(B2), typeof(B1), "P1", "P2" );
			Assert.AreNotEqual( map1.GetHashCode(), map2.GetHashCode() );
		}

		private static MapCallInfo GetMapCallInfo( Type sourceType, Type targetType, params string[] properties )
		{
			return new MapCallInfo( targetType, Type.EmptyTypes, Flags.InstanceAnyVisibility, MemberTypes.Property, "map", Type.EmptyTypes, null,
			                        true, sourceType, MemberTypes.Property, MemberTypes.Property, properties );
		}
	}
}