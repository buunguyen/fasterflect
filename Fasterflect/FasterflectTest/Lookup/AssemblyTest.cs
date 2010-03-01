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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Fasterflect;
using FasterflectTest.SampleModel.Animals;
using FasterflectTest.SampleModel.Animals.Attributes;
using FasterflectTest.SampleModel.Animals.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Lookup
{
    [TestClass]
    public class AssemblyTest
    {
        #region Types()
        [TestMethod]
        public void TestFindTypes()
        {
            Assembly assembly = Assembly.GetAssembly( typeof(int) );
        	IList<Type> types = assembly.Types();
            Assert.IsNotNull( types );
            Assert.IsTrue( types.Count > 1000 );
        }

        [TestMethod]
        public void TestFindTypesByEmptyNameListShouldReturnAllTypes()
        {
            Assembly assembly = Assembly.GetAssembly( typeof(int) );
        	IList<Type> types = assembly.Types( new string[ 0 ] );
            Assert.IsNotNull( types );
            Assert.IsTrue( types.Count > 1000 );

			types = assembly.Types( null );
            Assert.IsNotNull( types );
            Assert.IsTrue( types.Count > 1000 );
		}

        [TestMethod]
        public void TestFindTypesShouldReturnEmptyListIfNotFound()
        {
            Assembly assembly = Assembly.GetAssembly( typeof(int) );
        	IList<Type> types = assembly.Types( "UrzgHafn" );
            Assert.IsNotNull( types );
            Assert.AreEqual( 0, types.Count );
        }

        [TestMethod]
        public void TestFindTypesByName()
        {
            Assembly assembly = Assembly.GetAssembly( typeof(int) );
        	IList<Type> types = assembly.Types( "ArgumentException" );
            Assert.IsNotNull( types );
            Assert.AreEqual( 1, types.Count );
        }

        [TestMethod]
        public void TestFindTypesByPartialName()
        {
            Assembly assembly = Assembly.GetAssembly( typeof(int) );
        	IList<Type> types = assembly.Types( Flags.PartialNameMatch, "Argument" );
            Assert.IsNotNull( types );
            Assert.AreEqual( 10, types.Count );
        }
        #endregion
    }
}


