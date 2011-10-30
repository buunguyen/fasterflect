using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.NUnit
{
	[TestClass]
    public class EventHandlerTest
    {
        class EventSource
        {
            private delegate void VoidOp();
            private delegate string StringOp(string s);
            public delegate int IntOp(int i);

			#pragma warning disable 0169
			// ReSharper disable UnusedMember.Local
			private VoidOp voidOp;
            private StringOp stringOp;
            private IntOp intOp;

            private static VoidOp StaticVoidOp;
            private static StringOp StaticStringOp;
            private static IntOp StaticIntOp;

            private event IntOp intEvent;
            private static event IntOp StaticIntEvent;
			// ReSharper restore UnusedMember.Local
			#pragma warning restore 0169
			
			public int TriggerEvents(int i)
            {
                return intEvent(i);
            }

            public static int TriggerStaticEvents(int i)
            {
                return StaticIntEvent(i);
            }
        }

        [TestMethod]
        public void Test_assign_static_no_arg_void_return_delegate()
        {
            var type = typeof(EventSource);
            var call = false;
            type.AssignHandler("StaticVoidOp", args => call = true);
            type.InvokeDelegate("StaticVoidOp");
            Assert.AreEqual( true, call );
        }

        [TestMethod]
        public void Test_handle_static_int_arg_int_return_delegate()
        {
            var type = typeof(EventSource);
            var sum = 0;
            type.AddHandler("StaticIntOp", args => sum += (int)args[0] * 2);
            type.AddHandler("StaticIntOp", args => sum += (int)args[0] * 3);
            var result = (int)type.InvokeDelegate("StaticIntOp", 2);
            Assert.AreEqual(10, sum);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Test_assign_static_string_arg_string_return_delegate()
        {
            var type = typeof(EventSource);
            type.AddHandler("StaticStringOp", args => (string)args[0] + "1");
            type.AddHandler("StaticStringOp", args => (string)args[0] + "2");
            var result = (string)type.InvokeDelegate("StaticStringOp", "A");
            Assert.AreEqual("A2", result);
        }

        [TestMethod]
        public void Test_assign_instance_no_arg_void_return_delegate()
        {
            var target = typeof(EventSource).CreateInstance(  );
            var call = false;
            target.AssignHandler("voidOp", args => call = true);
            target.InvokeDelegate("voidOp");
            Assert.AreEqual(true, call);
        }
        
        [TestMethod]
        public void Test_handle_instance_int_arg_int_return_delegate()
        {
            var target = typeof(EventSource).CreateInstance();
            var sum = 0;
            target.AddHandler("intOp", args => sum += (int)args[0] * 2);
            target.AddHandler("intOp", args => sum += (int)args[0] * 3);
            var result = (int)target.InvokeDelegate("intOp", 2);
            Assert.AreEqual(10, sum);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Test_assign_instance_string_arg_string_return_delegate()
        {
            var target = typeof(EventSource).CreateInstance(  );
            target.AddHandler("stringOp", args => (string)args[0] + "1");
            target.AddHandler("stringOp", args => (string)args[0] + "2");
            var result = (string)target.InvokeDelegate("stringOp", "A");
            Assert.AreEqual("A2", result);
        }

        [TestMethod]
        public void Test_handle_instance_int_arg_int_return_event()
        {
            var target = typeof(EventSource).CreateInstance();
            var sum = 0;
            target.AddHandler("intEvent", args => sum += (int)args[0] * 2);
            target.AddHandler("intEvent", args => sum += (int)args[0] * 3);
            var result = target.CallMethod("TriggerEvents", 2);
            Assert.AreEqual(10, sum);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Test_handle_static_int_arg_int_return_event()
        {
            var type = typeof(EventSource);
            var sum = 0;
            type.AddHandler("StaticIntEvent", args => sum += (int)args[0] * 2);
            type.AddHandler("StaticIntEvent", args => sum += (int)args[0] * 3);
            var result = type.CallMethod("TriggerStaticEvents", 2);
            Assert.AreEqual(10, sum);
            Assert.AreEqual(10, result);
        }

    }
}
