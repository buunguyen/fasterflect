using System;
using System.Collections.Generic;
using Fasterflect;
using FasterflectTest.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Invocation
{
    [TestClass]
    public class ArrayTest : BaseInvocationTest
    {
        private static readonly List<Type> typeList = new List<Type>
                                                      {
                    typeof(Person[]), 
                    typeof(PersonStruct[]), 
                    typeof(int[])
                };

        [TestMethod]
        public void TestConstructArrays()
        {
            typeList.ForEach(type =>
            {
                var obj = type.CreateInstance(10);
                Assert.IsNotNull(obj);
                Assert.AreEqual(10, obj.GetPropertyValue("Length"));
            });
        }

        [TestMethod]
        public void TestGetSetElements()
        {
            typeList.ForEach(type =>
            {
                var array = type.CreateInstance(10);
                var instance = type.GetElementType().CreateInstance();
                array.SetElement(1, instance);
                Assert.AreEqual(instance, array.GetElement(1));
            });
        }
    }
}
