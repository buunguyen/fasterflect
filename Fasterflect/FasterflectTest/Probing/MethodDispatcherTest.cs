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
using System.Xml.Linq;
using Fasterflect;
using Fasterflect.Probing;
using FasterflectTest.SampleModel.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Probing
{
	[TestClass]
	public class MethodDispatcherTest
	{
		[TestMethod]
		public void TestMethodDispatcherWithSingleMethod()
		{
			var obj = new Elephant();
			var dispatcher = new MethodDispatcher();
			dispatcher.AddMethod( typeof(Elephant).Methods( "Eat" ).First() );
			dispatcher.Invoke( obj, true, new {} );
			Assert.AreEqual( 1, obj.MethodInvoked );
		}

		[TestMethod]
		public void TestMethodDispatcherWithMultipleMethods()
		{
			var obj = new Elephant();
			var dispatcher = new MethodDispatcher();
			typeof(Elephant).Methods( Flags.InstanceAnyVisibility | Flags.ExcludeBackingMembers ).ForEach( dispatcher.AddMethod );
			dispatcher.Invoke( obj, true, new {} );
			Assert.AreEqual( 1, obj.MethodInvoked );
			dispatcher.Invoke( obj, true, new { count=2.0, food="hay", isHay=true } );
			Assert.AreEqual( 5, obj.MethodInvoked );
			dispatcher.Invoke( obj, true, new { count=2, volume=4 } );
			Assert.AreEqual( 11, obj.MethodInvoked );
		}

		#region Sample class with static overloads
		internal class StaticElephant
		{
			#pragma warning disable 0169, 0649
			public static int MethodInvoked { get; private set; }
			#pragma warning restore 0169, 0649

			public static void Eat()
			{
				MethodInvoked = 1;
			}
			public static void Eat( string food )
			{
				MethodInvoked = 2;
			}
			public static void Eat( int count )
			{
				MethodInvoked = 3;
			}
			public static void Eat( int count, string food )
			{
				MethodInvoked = 4;
			}
			public static void Eat( double count, string food, bool isHay )
			{
				MethodInvoked = 5;
			}
		}
		#endregion

		[TestMethod]
		public void TestMethodDispatcherWithStaticMethods()
		{
			var type = typeof(StaticElephant);
			var dispatcher = new MethodDispatcher();
			typeof(StaticElephant).Methods( Flags.StaticAnyVisibility ).ForEach( dispatcher.AddMethod );
			dispatcher.Invoke( type, true, new {} );
			Assert.AreEqual( 1, StaticElephant.MethodInvoked );
			dispatcher.Invoke( type, true, new { count=2.0, food="hay", isHay=true } );
			Assert.AreEqual( 5, StaticElephant.MethodInvoked );
			dispatcher.Invoke( type, false, new { count=2, volume=4 } );
			Assert.AreEqual( 3, StaticElephant.MethodInvoked );
		}
	}
}