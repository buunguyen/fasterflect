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
