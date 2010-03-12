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
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FasterflectTest.SampleModel.People;

namespace FasterflectTest.Services
{
    [TestClass]
    public class XmlTransformerTest
	{
    	#region Map
		[TestMethod]
		public void TestToXml()
		{
			string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>{0}" +
			             "<Person>{0}{1}<Name>Bruce Lee</Name>{0}{1}<Age>25</Age>{0}{1}" +
						 "<MetersTravelled>0</MetersTravelled>{0}</Person>{0}";
			string expected = string.Format( xml, Environment.NewLine, "\t" );
			Person person = new Person( "Bruce Lee", 25 );
			Assert.AreEqual( expected, person.ToXml() );
		}
    	#endregion
	}
}
