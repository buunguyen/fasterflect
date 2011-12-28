using System;
using System.Reflection;
using Fasterflect;
using System.Collections.Generic;
using Fasterflect.Emitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.NUnit
{
	[TestClass]
	public class GenericTest
	{
		[TestMethod]
		public void Test_instantiate_generic_types()
		{
			object list = typeof(List<>).MakeGenericType( typeof(string) ).CreateInstance();
			Assert.IsInstanceOfType( list, typeof(List<string>) );

			object dict = typeof(Dictionary<,>).MakeGenericType( typeof(string), typeof(int) ).CreateInstance();
			Assert.IsInstanceOfType( dict, typeof(Dictionary<string, int>) );
		}

		private class Host
		{
			public static T Exact<T>( T t )
			{
				return t;
			}

			public string Add<T1, T2>( T1 obj1, T2 obj2 )
			{
				return string.Format( "1:{0}{1}", obj1, obj2 );
			}

			public string Add<T1, T2>( T1 obj1, T2 obj2, T2 obj3 )
			{
				return string.Format( "2:{0}{1}{2}", obj1, obj2, obj3 );
			}

			public string Add<T1>( T1 obj1, int obj2 )
			{
				return string.Format( "3:{0}{1}", obj1, obj2 );
			}

			public string Add<T1>( T1 obj1, int obj2, T1 obj3 )
			{
				return string.Format( "4:{0}{1}{2}", obj1, obj2, obj3 );
			}

			public string Add<T1, T2>( T1 obj1 )
			{
				return string.Format( "5:{0}_{1}", obj1, typeof(T2).Name );
			}

			public string Add( object obj1, object obj2 )
			{
				return string.Format( "6:{0}{1}", obj1, obj2 );
			}
		}

		[TestMethod]
		public void Test_invoke_static_generic_methods()
		{
			Type type = typeof(Host);
			var val = (int) type.CallMethod( new[] { typeof(int) }, "Exact", 1 );
			Assert.AreEqual( 1, val );
		}

		[TestMethod]
		public void Test_invoke_instance_generic_method_two_arguments_two_params()
		{
			object target = typeof(Host).CreateInstance();
			var result = (string) target.CallMethod( new[] { typeof(string), typeof(int) }, "Add", "1", 2 );
			Assert.AreEqual( "1:12", result );
		}

		[TestMethod]
		public void Test_invoke_instance_generic_method_two_arguments_three_params()
		{
			object target = typeof(Host).CreateInstance();
			var result = (string) target.CallMethod( new[] { typeof(string), typeof(int) }, "Add", "1", 2, 3 );
			Assert.AreEqual( "2:123", result );
		}

		[TestMethod]
		public void Test_invoke_instance_generic_method_one_argument_two_params_one_generic()
		{
			object target = typeof(Host).CreateInstance();
			var result = (string) target.CallMethod( new[] { typeof(string) }, "Add", "1", 2 );
			Assert.AreEqual( "3:12", result );
		}

		[TestMethod]
		public void Test_invoke_instance_generic_method_one_argument_three_params_two_generic()
		{
			object target = typeof(Host).CreateInstance();
			var result = (string) target.CallMethod( new[] { typeof(string) }, "Add", "1", 2, "3" );
			Assert.AreEqual( "4:123", result );
		}

		[TestMethod]
		public void Test_invoke_instance_generic_method_two_arguments_one_param_generic()
		{
			object target = typeof(Host).CreateInstance();
			var result = (string) target.CallMethod( new[] { typeof(string), typeof(int) }, "Add", "1" );
			Assert.AreEqual( "5:1_Int32", result );
		}

		[TestMethod]
		public void Test_invoke_instance_non_generic_method_two_params()
		{
			object target = typeof(Host).CreateInstance();
			var result = (string) target.CallMethod( null, "Add", "1", 2 );
			Assert.AreEqual( "6:12", result );
		}
	}
}
