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

using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Internal
{
    [TestClass]
    public class FlagsTest
    {
        [TestMethod]
        public void TestFlagsToString()
        {
 			Assert.AreEqual( string.Empty, Flags.None.ToString() );
 			Assert.AreEqual( "Public", Flags.Public.ToString() );
 			Assert.AreEqual( "Instance", Flags.Instance.ToString() );
			Assert.AreEqual( "Public", (Flags.None | Flags.Public).ToString() );
			Assert.AreEqual( "Instance | Public", (Flags.Instance | Flags.Public).ToString() );
			Assert.AreEqual( "Instance | NonPublic | Public", (Flags.Instance | Flags.Public | Flags.NonPublic).ToString() );
			Assert.AreEqual( "Instance | NonPublic | Public", Flags.InstanceAnyVisibility.ToString() );
        }
    }
}


