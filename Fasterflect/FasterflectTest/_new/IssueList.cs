using System;
using System.Collections;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.NUnit
{
    [TestClass]
    public class IssueList
    {
        [TestMethod]
        public void Issue1()
        {
            Console.WriteLine("List 1: Add() without Fasterflect");
            var list1 = (ArrayList)typeof(ArrayList).CreateInstance();
            for (int i = 0; i < 10; i++)
            {
                list1.Add( i );
            }

            Console.WriteLine("List 1: Add() by Fasterflect");
            var list2 = typeof(ArrayList).CreateInstance();
            for (int i = 0; i < 10; i++)
            {
                list2.CallMethod("Add", i);
            }
            //var size = (int)list2.GetPropertyValue("Count");
            //for (int i = 0; i < size; i++)
            //{
            //    Assert.AreEqual(i + 1, list2.GetIndexer(i));
            //}
        }
    }
}
