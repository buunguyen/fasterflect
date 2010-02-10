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

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class MemberTest : BaseInvocationTest
    {
        [TestMethod]
        public void TestAccessStaticMemberViaMemberInfo()
        {
            RunWith((Type type) =>
               {
                   var memberInfo = type.Member("TotalPeopleCreated", Flags.StaticCriteria);
                   var totalPeopleCreated = (int)memberInfo.Get() + 1;
                   memberInfo.Set(totalPeopleCreated);
                   VerifyProperties(type, new { totalPeopleCreated });
               });
        }

        [TestMethod]
        public void TestAccessInstanceMemberViaMemberInfo()
        {
            RunWith((object person) =>
            {
                var memberInfo = person.UnwrapIfWrapped().GetType().Member("Name");
                var name = (string)memberInfo.Get(person) + " updated";
                memberInfo.Set(person, name);
                VerifyProperties(person, new { name });
            });
        }
    }
}
