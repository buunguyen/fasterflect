using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class ArrayTest
    {
        [TestMethod]
        public void test_construct_array()
        {
            var obj = typeof(PersonStruct[]).Construct(10);
            Assert.IsNotNull(obj);
            Assert.AreEqual(10, obj.GetProperty<int>("Length"));

            obj = typeof(int[]).Construct(0);
            Assert.IsNotNull(obj);
            Assert.AreEqual(0, obj.GetProperty<object>("Length"));
        }

        [TestMethod]
        public void test_get_set()
        {
            var obj = typeof(PersonStruct[]).Construct(10);
            var p = new PersonStruct();
            obj.SetElement(1, p);
            Assert.AreEqual(p, obj.GetElement<object>(1));

            obj = typeof(int[]).Construct(10);
            obj.SetElement(1, 2);
            Assert.AreEqual(2, obj.GetElement<int>(1));
        }

        public void test_get_set_with_cached_delegate()
        {
            var type = typeof(PersonStruct[]);
            var setFunc = type.DelegateForSetElement();
            var getFunc = type.DelegateForGetElement();
            var instance = type.Construct(10);

            var person = type.GetElementType().Construct();
            setFunc(instance, 9, person);
            Assert.AreEqual(person, getFunc(instance, 9));
        }
    }
}
