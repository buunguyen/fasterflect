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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;
using FasterflectTest.SampleModel.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Lookup
{
	[TestClass]
	public class TypeTest
	{
		#region Implements()
		[TestMethod]
		public void TestImplements()
		{
			Assert.IsTrue( typeof(int).Implements<IComparable>() );
			Assert.IsFalse( typeof(Stack).Implements<IComparable>() );
			Assert.IsTrue( typeof(int[]).Implements<ICollection>() );
			Assert.IsTrue( typeof(List<>).Implements<ICollection>() );
			Assert.IsTrue( typeof(List<int>).Implements<ICollection>() );
			Assert.IsTrue( typeof(List<int>).Implements<IList<int>>() );
			Assert.IsFalse( typeof(List<int>).Implements<IList<string>>() );
			Assert.IsFalse( typeof(List<int>).Implements<IEnumerator>() );
			Assert.IsTrue( typeof(IList).Implements<ICollection>() );
			Assert.IsTrue( typeof(IList<int>).Implements<ICollection<int>>() );
			Assert.IsFalse( typeof(ICollection).Implements<IList>() );
			Assert.IsFalse( typeof(ICollection<int>).Implements<IList<int>>() );
		}
		#endregion

		#region Implements()
		[TestMethod]
		public void TestIsFrameworkType()
		{
			Assert.IsTrue( typeof(int).IsFrameworkType() );
			Assert.IsTrue( typeof(Assembly).IsFrameworkType() );
			Assert.IsTrue( typeof(BindingFlags).IsFrameworkType() );
			Assert.IsTrue( typeof(List<>).IsFrameworkType() );
			Assert.IsTrue( typeof(int).IsFrameworkType() );
			Assert.IsFalse( typeof(Flags).IsFrameworkType() );
			Assert.IsFalse( typeof(Lion).IsFrameworkType() );
		}
		#endregion
	}
}