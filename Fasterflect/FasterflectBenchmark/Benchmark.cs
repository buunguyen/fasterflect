#region License

// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Fasterflect.Caching;
using Fasterflect.Emitter;
using Fasterflect.ObjectConstruction;

namespace FasterflectBenchmark
{
	public class Benchmark
	{
		#region Sample Classes
		private class Person
		{
			internal static int counter;
			internal string name;

			internal Person()
			{
			}

			internal Person(int age, string name)
			{
				Age = age;
				this.name = name;
			}

			public object this[int a, int b]
			{
				get { return null; }
				set { }
			}

			internal int Age { get; set; }
			internal static int Counter { get; set; }

			internal void Walk()
			{
			}

			internal void Walk(int speed)
			{
			}

			internal static void Generate()
			{
			}

			internal static void Generate(int seed)
			{
			}

			internal string GetName()
			{
				return name;
			}

			internal string GetName(string prefix)
			{
				return prefix + " " + name;
			}
		}
		#endregion

		private static readonly int[] Iterations = new[] { 20000, 2000000 };
		private static readonly object[] NoArgArray = new object[0];
		private static readonly object[] ArgArray = new object[] {10};
		private static readonly Type TargetType = typeof (Person);
		private static readonly Person TargetPerson = new Person();
		private static readonly Stopwatch Watch = new Stopwatch();

		public static void Main(string[] args)
		{
			//RunDictionaryBenchmark();
			//RunHashCodeBenchmark();
			RunTryCreateInstanceBenchmark();
			RunConstructorBenchmark();
			RunFieldBenchmark();
			RunStaticFieldBenchmark();
			RunPropertyBenchmark();
			RunStaticPropertyBenchmark();
			RunMethodInvocationBenchmark();
			RunStaticMethodInvocationBenchmark();
			RunIndexerBenchmark();
		}

		#region Internal Testing
		private static void RunDictionaryBenchmark()
		{
			int dictionarySize = 1000;
			int index = new Random( (int) (DateTime.Now.Ticks % int.MaxValue) ).Next( 0, dictionarySize );
			List<string> stringList = Enumerable.Range( 0, dictionarySize ).Select( s => Path.GetRandomFileName() + Path.GetRandomFileName() ).ToList();
			Dictionary<string, string> stringDictionary = stringList.ToDictionary( s => s, s => s );
			var stringCache = new Cache<string,string>();
			stringList.ForEach( s => stringCache.Insert( s, s, CacheStrategy.Permanent ) );
			
			string key = stringList[ index ];
			var callInfo = new CallInfo( TargetType, MemberTypes.Field, "name", Constants.ArrayOfObjectType, false );

			var initMap = new Dictionary<string, Action> {};
			var actionMap = new Dictionary<string, Action>
			                	{
									{"Dictionary ContainsKey", () => stringDictionary.ContainsKey( key ) },
									{"Dictionary Indexer", () => { var x = stringDictionary[ key ]; } },
									{"Dictionary TryGetValue", () => { string s; stringDictionary.TryGetValue( key, out s ); } },
									//{"List Contains", () => stringList.Contains( key ) },
									//{"List Linq First", () => stringList.First( item => item == key ) },
									{"Cache GetValue", () => stringCache.Get( key ) },
			                	};
			Execute("Dictionary Benchmark", initMap, actionMap);
		}

		private static void RunHashCodeBenchmark()
		{
			var callInfo = new CallInfo( TargetType, MemberTypes.Field, "name", new [] { typeof(int), typeof(string) }, false );
			var callInfoOther = new CallInfo( typeof(CallInfo), MemberTypes.Field, "other", new [] { typeof(string) }, false );
			var sourceInfo = new SourceInfo( new { ID=42, Name="Test" }.GetType() );
			var sourceInfoOther = new SourceInfo( new { id=42, Name="Test" }.GetType() );

			var initMap = new Dictionary<string, Action> {};
			var actionMap = new Dictionary<string, Action>
			                	{
			                		{"CallInfo GetHashCode", () => callInfo.GetHashCode() },
			                		{"CallInfo Equals Other", () => callInfo.Equals( callInfoOther ) },
			                		{"SourceInfo GetHashCode", () => sourceInfo.GetHashCode() },
			                		{"SourceInfo Equals Other", () => sourceInfo.Equals( sourceInfoOther ) },
			                		{"string GetHashCode", () => "foo".GetHashCode() },
			                		{"string Equals", () => "foo".Equals( "bar" ) },
									{"new CallInfo", () => new CallInfo(TargetType, MemberTypes.Field, "name", new [] { typeof(int), typeof(string) }, false) },
									{"new SourceInfo", () => new SourceInfo( TargetType, new [] { "ID", "Name" }, new [] { typeof(int), typeof(string) } ) },
									{"new SourceInfo anon", () => new SourceInfo( new { ID=42, Name="Test" }.GetType() ) },
			                	};
			Execute("HashCode Benchmark", initMap, actionMap);
		}
		#endregion

		#region Fasterflect Core
		private static void RunTryCreateInstanceBenchmark()
		{
			var names = new string[] {"Age", "Name"};
			var types = new Type[] {typeof (int), typeof (string)};
			var values = new object[] {42, "Arthur Dent"};

			var initMap = new Dictionary<string, Action> {};
			var actionMap = new Dictionary<string, Action>
			                	{
									{"TryCreateInstance [empty]", () => typeof (Person).TryCreateInstance(new {})},
									{"TryCreateInstance [n+t+v]", () => typeof (Person).TryCreateInstance(names, types, values)},
			                		{"TryCreateInstance [object]", () => typeof (Person).TryCreateInstance(new {Age = 42, Name = "Arthur Dent"})},
			                	};
			Execute("TryCreateInstance Benchmark", initMap, actionMap);
		}

		private static void RunConstructorBenchmark()
		{
			ConstructorInfo ctorInfo = null;
			ConstructorInvoker invoker = null;

			var initMap = new Dictionary<string, Action>
			              	{
			              		{"Init info", () => {ctorInfo = typeof (Person).GetConstructor(
									BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);}},
			              		{"Init ctorInvoker", () => { invoker = typeof (Person).DelegateForCreateInstance(); }},
			              	};
			var actionMap = new Dictionary<string, Action>
			                	{
			                		{"Direct ctor", () => new Person()},
			                		{"Reflection ctor", () => ctorInfo.Invoke(NoArgArray)},
			                		{"Fasterflect ctor", () => typeof (Person).CreateInstance()},
			                		{"Fasterflect cached ctor", () => invoker(NoArgArray)},
			                	};
			Execute("Construction Benchmark", initMap, actionMap);
		}

		private static void RunFieldBenchmark()
		{
			FieldInfo fieldInfo = null;
			MemberSetter setter = null;
			MemberGetter getter = null;
			var initMap = new Dictionary<string, Action>
			              	{
								{"Init info", () => { fieldInfo = TargetType.GetField("name", BindingFlags.NonPublic | BindingFlags.Instance); }},
								{"Init setter", () => { setter = TargetType.DelegateForSetFieldValue("name"); }},
								{"Init getter", () => { getter = TargetType.DelegateForGetFieldValue("name"); }}
			              	};

			dynamic tmp = TargetPerson;
			var actionMap = new Dictionary<string, Action>
			                	{
									{"Direct set", () => { TargetPerson.name = "John"; }},
									{"Direct get", () => { string name = TargetPerson.name; }},
									{"'dynamic' set", () => { tmp.name = "John"; }},
									{"'dynamic' get", () => { dynamic name = tmp.name; }},
									{"Reflection set", () => fieldInfo.SetValue(TargetPerson, "John")},
									{"Reflection get", () => fieldInfo.GetValue(TargetPerson)},
									{"Fasterflect set", () => TargetPerson.SetFieldValue("name", "John")},
									{"Fasterflect get", () => TargetPerson.GetFieldValue("name")},
									{"Fasterflect cached set", () => setter(TargetPerson, "John")},
									{"Fasterflect cached get", () => getter(TargetPerson)},
			                	};
			Execute("Field Benchmark", initMap, actionMap);
		}

		private static void RunStaticFieldBenchmark()
		{
			FieldInfo fieldInfo = null;
			StaticMemberSetter setter = null;
			StaticMemberGetter getter = null;
			var initMap = new Dictionary<string, Action>
			              	{
			              		{"Init info", () => { fieldInfo = TargetType.GetField("counter", BindingFlags.NonPublic | BindingFlags.Static); }},
			              		{"Init setter", () => { setter = TargetType.DelegateForSetStaticFieldValue("counter"); }},
			              		{"Init getter", () => { getter = TargetType.DelegateForGetStaticFieldValue("counter"); }}
			              	};

			var actionMap = new Dictionary<string, Action>
			                	{
			                		{"Direct set", () => { Person.counter = 1; }},
			                		{"Direct get", () => { int counter = Person.counter; }},
			                		{"Reflection set", () => fieldInfo.SetValue(TargetType, 1)},
			                		{"Reflection get", () => fieldInfo.GetValue(TargetType)},
			                		{"Fasterflect set", () => TargetType.SetFieldValue("counter", 1)},
			                		{"Fasterflect get", () => TargetType.GetFieldValue("counter")},
			                		{"Fasterflect cached set", () => setter(1)},
			                		{"Fasterflect cached get", () => getter()},
			                	};
			Execute("Static Field Benchmark", initMap, actionMap);
		}

		private static void RunPropertyBenchmark()
		{
			PropertyInfo propInfo = null;
			MemberSetter setter = null;
			MemberGetter getter = null;
			var initMap = new Dictionary<string, Action>
			              	{
			              		{"Init info", () => { propInfo = TargetType.GetProperty("Age", BindingFlags.NonPublic | BindingFlags.Instance); }},
			              		{"Init setter", () => { setter = TargetType.DelegateForSetPropertyValue("Age"); }},
			              		{"Init getter", () => { getter = TargetType.DelegateForGetPropertyValue("Age"); }}
			              	};
			dynamic tmp = TargetPerson;
			var actionMap = new Dictionary<string, Action>
			                	{
			                		{"Direct set", () => { TargetPerson.Age = 10; }},
			                		{"Direct get", () => { int age = TargetPerson.Age; }},
			                		{"'dynamic' set", () => { tmp.Age = 10; }},
			                		{"'dynamic' get", () => { dynamic age = tmp.Age; }},
			                		{"Reflection set", () => propInfo.SetValue(TargetPerson, 10, null)},
			                		{"Reflection get", () => propInfo.GetValue(TargetPerson, null)},
			                		{"Fasterflect set", () => TargetPerson.SetPropertyValue("Age", 10)},
			                		{"Fasterflect get", () => TargetPerson.GetPropertyValue("Age")},
			                		{"Fasterflect cached set", () => setter(TargetPerson, 10)},
			                		{"Fasterflect cached get", () => getter(TargetPerson)},
			                	};
			Execute("Property Benchmark", initMap, actionMap);
		}

		private static void RunStaticPropertyBenchmark()
		{
			PropertyInfo propInfo = null;
			StaticMemberSetter setter = null;
			StaticMemberGetter getter = null;

			var initMap = new Dictionary<string, Action>
			              	{
			              		{"Init info", () => { propInfo = TargetType.GetProperty("Counter", BindingFlags.NonPublic | BindingFlags.Static); }},
			              		{"Init setter", () => { setter = TargetType.DelegateForSetStaticPropertyValue("Counter"); }},
			              		{"Init getter", () => { getter = TargetType.DelegateForGetStaticPropertyValue("Counter"); }}
			              	};

			var actionMap = new Dictionary<string, Action>
			                	{
			                		{"Direct set", () => { Person.Counter = 10; }},
			                		{"Direct get", () => { int counter = Person.Counter; }},
			                		{"Reflection set", () => propInfo.SetValue(TargetType, 10, null)},
			                		{"Reflection get", () => propInfo.GetValue(TargetType, null)},
			                		{"Fasterflect set", () => TargetType.SetPropertyValue("Counter", 10)},
			                		{"Fasterflect get", () => TargetType.GetPropertyValue("Counter")},
			                		{"Fasterflect cached set", () => setter(10)},
			                		{"Fasterflect cached get", () => getter()},
			                	};
			Execute("Static Property Benchmark", initMap, actionMap);
		}

		private static void RunIndexerBenchmark()
		{
			MethodInfo setterInfo = null;
			MethodInfo getterInfo = null;
			MethodInvoker setter = null;
			MethodInvoker getter = null;

			var initMap = new Dictionary<string, Action>
			              	{
			              		{"Init setter info", () => { setterInfo = TargetType.GetMethod("set_Item", new[] {typeof (int), typeof (int), typeof (object)}); }},
			              		{"Init getter info", () => { getterInfo = TargetType.GetMethod("get_Item", new[] {typeof (int), typeof (int)}); }},
			              		{"Init setter", () => { setter = TargetType.DelegateForSetIndexer(new[] {typeof (int), typeof (int), typeof (object)}); }},
			              		{"Init getter", () => { getter = TargetType.DelegateForGetIndexer(new[] {typeof (int), typeof (int)}); }}
			              	};

			dynamic tmp = TargetPerson;
			var actionMap = new Dictionary<string, Action>
			                	{
			                		{"Direct set", () => { TargetPerson[1, 2] = null; }},
			                		{"Direct get", () => { object t = TargetPerson[1, 2]; }},
			                		{"'dynamic' set", () => { tmp[1, 2] = null; }},
			                		{"'dynamic' get", () => { dynamic t = tmp[1, 2]; }},
			                		{"Reflection set", () => setterInfo.Invoke(TargetPerson, new object[] {1, 2, null})},
			                		{"Reflection get", () => getterInfo.Invoke(TargetPerson, new object[] {1, 2})},
			                		{"Fasterflect set", () => TargetPerson.SetIndexer(new[] {typeof (int), typeof (int), typeof (object)}, new object[] {1, 2, null})},
			                		{"Fasterflect get", () => TargetPerson.GetIndexer(new[] {typeof (int), typeof (int)}, new object[] {1, 2})},
			                		{"Fasterflect cached set", () => setter(TargetPerson, 1, 2, null)},
			                		{"Fasterflect cached get", () => getter(TargetPerson, 1, 2)},
			                	};
			Execute("Indexer Benchmark", initMap, actionMap);
		}

		private static void RunMethodInvocationBenchmark()
		{
			MethodInfo noArgMethodInfo = null;
			MethodInfo argMethodInfo = null;

			MethodInvoker noArgInvoker = null;
			MethodInvoker argInvoker = null;

			var initMap = new Dictionary<string, Action>
			              	{
			              		{"Init no-arg info", () => {noArgMethodInfo = TargetType.GetMethod("Walk", BindingFlags.NonPublic | BindingFlags.Instance,
			              					                                                       null, new Type[0], null);}},
			              		{"Init arg info", () => {argMethodInfo = TargetType.GetMethod("Walk", BindingFlags.NonPublic | BindingFlags.Instance, null,
			              					                                                  new Type[] {typeof (int)}, null);}},
			              		{"Init no-arg invoker", () => { noArgInvoker = TargetType.DelegateForInvoke("Walk"); }},
			              		{"Init arg invoker", () => { argInvoker = TargetType.DelegateForInvoke("Walk", new[] {typeof (int)}); }}
			              	};

			dynamic tmp = TargetPerson;
			var actionMap = new Dictionary<string, Action>
			                	{
			                		{"Direct invoke", () => TargetPerson.Walk()},
			                		{"Direct invoke (arg)", () => TargetPerson.Walk(10)},
			                		{"'dynamic' invoke", () => { tmp.Walk(); }},
			                		{"'dynamic' invoke (arg)", () => { tmp.Walk(10); }},
			                		{"Reflection invoke", () => noArgMethodInfo.Invoke(TargetPerson, NoArgArray)},
			                		{"Reflection invoke (arg)", () => argMethodInfo.Invoke(TargetPerson, ArgArray)},
			                		{"Fasterflect invoke", () => TargetPerson.Invoke("Walk")},
			                		{"Fasterflect invoke (arg)", () => TargetPerson.Invoke("Walk", new[] {typeof (int)}, ArgArray)},
			                		{"Fasterflect cached invoke", () => noArgInvoker(TargetPerson, NoArgArray)},
			                		{"Fasterflect cached invoke (arg)", () => argInvoker(TargetPerson, ArgArray)}
			                	};
			Execute("Method Benchmark", initMap, actionMap);
		}

		private static void RunStaticMethodInvocationBenchmark()
		{
			MethodInfo noArgMethodInfo = null;
			MethodInfo argMethodInfo = null;
			StaticMethodInvoker noArgInvoker = null;
			StaticMethodInvoker argInvoker = null;

			var initMap = new Dictionary<string, Action>
			              	{
			              		{"Init no-arg info", () => {noArgMethodInfo = TargetType.GetMethod("Generate", BindingFlags.NonPublic | BindingFlags.Static,
			              					                                                       null, new Type[0], null);}},
			              		{"Init arg info", () => {argMethodInfo = TargetType.GetMethod("Generate", BindingFlags.NonPublic | BindingFlags.Static,
			              					                                                  null, new[] {typeof (int)}, null);}},
			              		{"Init no-arg invoker", () => { noArgInvoker = TargetType.DelegateForStaticInvoke("Generate"); }},
			              		{"Init arg invoker", () => { argInvoker = TargetType.DelegateForStaticInvoke("Generate", new[] {typeof (int)}); }}
			              	};

			var actionMap = new Dictionary<string, Action>
			                	{
			                		{"Direct invoke", () => Person.Generate()},
			                		{"Direct invoke (arg)", () => Person.Generate(10)},
			                		{"Reflection invoke", () => noArgMethodInfo.Invoke(TargetType, NoArgArray)},
			                		{"Reflection invoke (arg)", () => argMethodInfo.Invoke(TargetType, ArgArray)},
			                		{"Fasterflect invoke", () => TargetType.Invoke("Generate")},
			                		{"Fasterflect invoke (arg)", () => TargetType.Invoke("Generate", new[] {typeof (int)}, ArgArray)},
			                		{"Fasterflect cached invoke", () => noArgInvoker(NoArgArray)},
			                		{"Fasterflect cached invoke (arg)", () => argInvoker(ArgArray)}
			                	};
			Execute("Static Method Invocations", initMap, actionMap);
		}
		#endregion

		#region Execute & Measure
		private static void Execute(string name, Dictionary<string, Action> initMap,
		                            Dictionary<string, Action> actionMap)
		{
			Console.WriteLine("------------- {0} ------------- ", name);

			Console.WriteLine("*** Initialization");
			Measure(Watch, initMap, 1);
			Console.WriteLine();

			foreach (int iterationCount in Iterations)
			{
				Console.WriteLine("*** Executing for {0} iterations", iterationCount);
				Measure(Watch, actionMap, iterationCount);
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private static void Measure(Stopwatch watch, Dictionary<string, Action> actionMap,
		                            int iterationCount)
		{
			foreach (var entry in actionMap)
			{
				watch.Start();
				for (int i = 0; i < iterationCount; i++)
					entry.Value();
				watch.Stop();
				Console.WriteLine("{0,-35} {1,6} ms", entry.Key + ":", watch.ElapsedMilliseconds);
				watch.Reset();
			}
		}
		#endregion
	}
}