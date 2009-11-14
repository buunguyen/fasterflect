using System;
using System.Collections.Generic;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class ArrayTest
    {
        class AClass { }
        class AStruct { }

        private static readonly List<Type> TypeList = new List<Type>
                {
                    typeof(AClass[]), 
                    typeof(AStruct[]), 
                    typeof(int[])
                };

        [TestMethod]
        public void Test_construct_arrays()
        {
            TypeList.ForEach(type =>
                             {
                                 var obj = type.Construct(10);
                                 Assert.IsNotNull(obj);
                                 Assert.AreEqual(10, obj.GetProperty<int>("Length"));
                             });
        }

        [TestMethod]
        public void Test_construct_arrays_via_delegate()
        {
            TypeList.ForEach(type =>
                            {
                                var ctorFunc = type.DelegateForConstruct(typeof(int));
                                var obj = ctorFunc(20);
                                Assert.IsNotNull(obj);
                                Assert.AreEqual(20, obj.GetProperty<int>("Length"));
                            });
        }

        [TestMethod]
        public void Test_get_set_elements()
        {
            TypeList.ForEach(type =>
                                 {
                                     var array = type.Construct(10);
                                     var instance = type.GetElementType().Construct();
                                     array.SetElement(1, instance);
                                     Assert.AreEqual(instance, array.GetElement<object>(1));
                                 });
        }

        [TestMethod]
        public void Test_get_set_elements_via_delegate()
        {
            TypeList.ForEach(type =>
            {
                var setFunc = type.DelegateForSetElement();
                var getFunc = type.DelegateForGetElement();
                var array = type.Construct(10);
                var instance = type.GetElementType().Construct();
                setFunc(array, 9, instance);
                Assert.AreEqual(instance, getFunc(array, 9));
            });
        }
    }
}
