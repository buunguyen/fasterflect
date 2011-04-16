using System;
using Fasterflect;
using NUnit.Framework;
using System.Collections.Generic;

namespace FasterflectTest.NUnit
{
    class GenericTest
    {
        [Test]
        public void Test_instantiate_generic_types()
        {
            var list = typeof(List<>).MakeGenericType(typeof(string)).CreateInstance();
            Assert.IsInstanceOf( typeof(List<string>), list );

            var dict = typeof(Dictionary<,>).MakeGenericType(typeof(string), typeof(int)).CreateInstance();
            Assert.IsInstanceOf(typeof(Dictionary<string, int>), dict);
        }

        class Host
        {
            public static T Exact<T>(T t)
            {
                return t;
            }

            public string Add<T1, T2>(T1 obj1, T2 obj2)
            {
                return (string)(object)obj1 + (int)(object)obj2;
            }
        }

        [Test]
        public void Test_invoke_static_generic_methods()
        {
            var type = typeof(Host);
            int val = (int)type.CallMethod( new[] { typeof(int) }, "Exact", 1 );
            Assert.AreEqual( 1, val );
        }

        [Test]
        public void Test_invoke_instance_generic_methods()
        {
            var target = typeof(Host).CreateInstance(  );
            string result = (string)target.CallMethod(new[] { typeof(string), typeof(int) }, "Add", "1", 2);
            Assert.AreEqual("12", result);
        }
    }
}
