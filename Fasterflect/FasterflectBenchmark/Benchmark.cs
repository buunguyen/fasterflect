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
using System.Reflection;
using Fasterflect;

namespace FasterflectBenchmark
{
    public class Benchmark
    {
        class Person
        {
            internal Person(){}
            public object this[int a, int b]
            {
                get { return null; }
                set {}
            }
            internal static int counter;
            internal string name;
            internal int Age { get; set; }
            internal static int Counter { get; set; }
            internal void Walk() { }
            internal void Walk(int speed) { }
            internal static void Generate() { }
            internal static void Generate(int seed) { }
            internal string GetName() { return name; }
            internal string GetName(string prefix) { return prefix + " " + name; }
        }

        struct Animal
        {
            internal int id;
        }

        private static readonly int[] Iterations = new[] { 20000, 2000000 };
        private static readonly object[] NoArgArray = new object[0];
        private static readonly object[] ArgArray = new object[]{10};
        private static readonly Type TargetType = typeof (Person);
        private static readonly Person TargetPerson = new Person();
        private static readonly Stopwatch Watch = new Stopwatch();

        public static void Main(string[] args)
        {
            RunConstructorBenchmark();
            RunFieldBenchmark();
            RunStaticFieldBenchmark();
            RunPropertyBenchmark();
            RunStaticPropertyBenchmark();
            RunMethodInvocationBenchmark();
            RunStaticMethodInvocationBenchmark();
            RunIndexerBenchmark();
            RunStructFieldBenchmark();
        }

        private static void RunConstructorBenchmark()
        {
            ConstructorInfo ctorInfo = null;
            ConstructorInvoker invoker = null;;
            var initMap = new Dictionary<string, Action>
                              {
                                  {"Init info", () => { ctorInfo = typeof (Person).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null); }},
                                  {"Init ctorInvoker", () => {invoker = typeof(Person).DelegateForConstruct();}}
                              };
            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct ctor", () => new Person() },
                                  {"Reflection ctor", () => ctorInfo.Invoke(NoArgArray)},
                                  {"Fasterflect ctor", () => typeof(Person).Construct() },
                                  {"Fasterflect cached ctor", () => invoker(NoArgArray) },
                              };
            Execute("Construction Benchmark", initMap, actionMap);
        }

        private static void RunFieldBenchmark()
        {
            FieldInfo fieldInfo = null;
            AttributeSetter setter = null;
            AttributeGetter getter = null;
            var initMap = new Dictionary<string, Action>
                              {
                                  {"Init info", () => { fieldInfo = TargetType.GetField("name", BindingFlags.NonPublic | BindingFlags.Instance); }},
                                  {"Init setter", () => { setter = TargetType.DelegateForSetField("name"); }},
                                  {"Init getter", () => {getter = TargetType.DelegateForGetField("name");}}
                              };

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { TargetPerson.name = "John"; }},
                                  {"Direct get", () => { var name = TargetPerson.name; }},
                                  {"Reflection set", () => fieldInfo.SetValue(TargetPerson, "John")},
                                  {"Reflection get", () => fieldInfo.GetValue(TargetPerson)},
                                  {"Fasterflect set", () => TargetPerson.SetField("name", "John")},
                                  {"Fasterflect get", () => TargetPerson.GetField<string>("name")},
                                  {"Fasterflect cached set", () => setter(TargetPerson, "John")},
                                  {"Fasterflect cached get", () => getter(TargetPerson)},
                              };
            Execute("Field Benchmark", initMap, actionMap);
        }

        private static void RunStructFieldBenchmark()
        {
            var targetType = typeof (Animal);
            var targetAnimal = new Animal();
            var wrapper = new Struct(targetAnimal);
            FieldInfo fieldInfo = null;
            AttributeSetter setter = null;
            AttributeGetter getter = null;
            var initMap = new Dictionary<string, Action>
                              {
                                  {"Init info", () => { fieldInfo = targetType.GetField("id", BindingFlags.NonPublic | BindingFlags.Instance); }},
                                  {"Init setter", () => { setter = targetType.DelegateForSetField("id"); }},
                                  {"Init getter", () => { getter = targetType.DelegateForGetField("id");}}
                              };

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { targetAnimal.id = 10; }},
                                  {"Direct get", () => { var id = targetAnimal.id; }},
                                  {"Reflection set", () => fieldInfo.SetValue(targetAnimal, 10)},
                                  {"Reflection get", () => fieldInfo.GetValue(targetAnimal)},
                                  {"Fasterflect set", () => wrapper.SetField("id", 10)},
                                  {"Fasterflect get", () => wrapper.GetField<int>("id")},
                                  {"Fasterflect cached set", () => setter(wrapper, 10)},
                                  {"Fasterflect cached get", () => getter(wrapper)},
                              };
            Execute("Field Benchmark", initMap, actionMap);
        }

        private static void RunStaticFieldBenchmark()
        {
            FieldInfo fieldInfo = null;
            StaticAttributeSetter setter = null;
            StaticAttributeGetter getter = null;
            var initMap = new Dictionary<string, Action>
                              {
                                  {"Init info", () => { fieldInfo = TargetType.GetField("counter", BindingFlags.NonPublic | BindingFlags.Static); }},
                                  {"Init setter", () => { setter = TargetType.DelegateForSetStaticField("counter"); }},
                                  {"Init getter", () => { getter = TargetType.DelegateForGetStaticField("counter"); }}
                              };

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { Person.counter = 1; }},
                                  {"Direct get", () => { var counter = Person.counter; }},
                                  {"Reflection set", () => fieldInfo.SetValue(TargetType, 1)},
                                  {"Reflection get", () => fieldInfo.GetValue(TargetType)},
                                  {"Fasterflect set", () => TargetType.SetField("counter", 1)},
                                  {"Fasterflect get", () => TargetType.GetField<int>("counter")},
                                  {"Fasterflect cached set", () => setter(1)},
                                  {"Fasterflect cached get", () => getter()},
                              };
            Execute("Static Field Benchmark", initMap, actionMap);
        }

        private static void RunPropertyBenchmark()
        {
            PropertyInfo propInfo = null;
            AttributeSetter setter = null;
            AttributeGetter getter = null;
            var initMap = new Dictionary<string, Action>
                              {
                                  {"Init info", () => { propInfo = TargetType.GetProperty("Age", BindingFlags.NonPublic | BindingFlags.Instance); }},
                                  {"Init setter", () => { setter = TargetType.DelegateForSetProperty("Age"); }},
                                  {"Init getter", () => { getter = TargetType.DelegateForGetProperty("Age"); }}
                              };
      
            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { TargetPerson.Age = 10; }},
                                  {"Direct get", () => { var age = TargetPerson.Age; }},
                                  {"Reflection set", () => propInfo.SetValue(TargetPerson, 10, null)},
                                  {"Reflection get", () => propInfo.GetValue(TargetPerson, null)},
                                  {"Fasterflect set", () => TargetPerson.SetProperty("Age", 10)},
                                  {"Fasterflect get", () => TargetPerson.GetProperty<int>("Age")},
                                  {"Fasterflect cached set", () => setter(TargetPerson, 10)},
                                  {"Fasterflect cached get", () => getter(TargetPerson)},
                              };
            Execute("Property Benchmark", initMap, actionMap);
        }

        private static void RunStaticPropertyBenchmark()
        {
            PropertyInfo propInfo = null;
            StaticAttributeSetter setter = null;
            StaticAttributeGetter getter = null;

            var initMap = new Dictionary<string, Action>
                              {
                                  {"Init info", () => { propInfo = TargetType.GetProperty("Counter", BindingFlags.NonPublic | BindingFlags.Static); }},
                                  {"Init setter", () => { setter = TargetType.DelegateForSetStaticProperty("Counter"); }},
                                  {"Init getter", () => { getter = TargetType.DelegateForGetStaticProperty("Counter"); }}
                              };

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { Person.Counter = 10; }},
                                  {"Direct get", () => { var counter = Person.Counter; }},
                                  {"Reflection set", () => propInfo.SetValue(TargetType, 10, null)},
                                  {"Reflection get", () => propInfo.GetValue(TargetType, null)},
                                  {"Fasterflect set", () => TargetType.SetProperty("Counter", 10)},
                                  {"Fasterflect get", () => TargetType.GetProperty<int>("Counter")},
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
                                  {"Init setter info", () => { setterInfo = TargetType.GetMethod("set_Item", new[] { typeof(int), typeof(int), typeof(object) }); }},
                                  {"Init getter info", () => { getterInfo = TargetType.GetMethod("get_Item", new[] { typeof(int), typeof(int) }); }},
                                  {"Init setter", () => { setter = TargetType.DelegateForSetIndexer(new[] { typeof(int), typeof(int), typeof(object) }); }},
                                  {"Init getter", () => { getter = TargetType.DelegateForGetIndexer(new[] { typeof(int), typeof(int) }); }}
                              };

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { TargetPerson[1, 2] = null; }},
                                  {"Direct get", () => { var tmp = TargetPerson[1, 2]; }},
                                  {"Reflection set", () => setterInfo.Invoke(TargetPerson, new object[]{1, 2, null})},
                                  {"Reflection get", () => getterInfo.Invoke(TargetPerson, new object[]{1, 2 })},
                                  {"Fasterflect set", () => TargetPerson.SetIndexer(new[] { typeof(int), typeof(int), typeof(object) }, new object[]{1, 2, null})},
                                  {"Fasterflect get", () => TargetPerson.GetIndexer<object>(new[] { typeof(int), typeof(int) }, new object[]{1, 2})},
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
                                  {"Init no-arg info", () => { noArgMethodInfo = TargetType.GetMethod("Walk", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null); }},
                                  {"Init arg info", () => { argMethodInfo = TargetType.GetMethod("Walk", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[]{typeof(int)}, null); }},
                                  {"Init no-arg invoker", () => { noArgInvoker = TargetType.DelegateForInvoke("Walk"); }},
                                  {"Init arg invoker", () => { argInvoker = TargetType.DelegateForInvoke("Walk", new[] { typeof(int) }); }}
                              };

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct invoke", () => TargetPerson.Walk()},
                                  {"Direct invoke (arg)", () => TargetPerson.Walk(10)},
                                  {"Reflection invoke", () => noArgMethodInfo.Invoke(TargetPerson, NoArgArray)},
                                  {"Reflection invoke (arg)", () => argMethodInfo.Invoke(TargetPerson, ArgArray)},
                                  {"Fasterflect invoke", () => TargetPerson.Invoke("Walk")},
                                  {"Fasterflect invoke (arg)", () => TargetPerson.Invoke("Walk", new[]{typeof(int)}, ArgArray)},
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
                                  {"Init no-arg info", () => { noArgMethodInfo = TargetType.GetMethod("Generate", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[0], null); }},
                                  {"Init arg info", () => { argMethodInfo = TargetType.GetMethod("Generate", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(int) }, null); }},
                                  {"Init no-arg invoker", () => { noArgInvoker = TargetType.DelegateForStaticInvoke("Generate"); }},
                                  {"Init arg invoker", () => { argInvoker = TargetType.DelegateForStaticInvoke("Generate", new[] { typeof(int) }); }}
                              };

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct invoke", () => Person.Generate() },
                                  {"Direct invoke (arg)", () => Person.Generate(10) },
                                  {"Reflection invoke", () => noArgMethodInfo.Invoke(TargetType, NoArgArray)},
                                  {"Reflection invoke (arg)", () => argMethodInfo.Invoke(TargetType, ArgArray)},
                                  {"Fasterflect invoke", () => TargetType.Invoke("Generate")},
                                  {"Fasterflect invoke (arg)", () => TargetType.Invoke("Generate", new[]{typeof(int)}, ArgArray)},
                                  {"Fasterflect cached invoke", () => noArgInvoker(NoArgArray)},
                                  {"Fasterflect cached invoke (arg)", () => argInvoker(ArgArray)}
                              };
            Execute("Static Method Invocations", initMap, actionMap);
        }

        private static void Execute(string name, Dictionary<string, Action> initMap, 
            Dictionary<string, Action> actionMap)
        {
            Console.WriteLine("------------- {0} ------------- ", name);

            Console.WriteLine("*** Initialization");
            Measure(Watch, initMap, 1);
            Console.WriteLine();

            foreach (var iterationCount in Iterations)
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
    }
}
