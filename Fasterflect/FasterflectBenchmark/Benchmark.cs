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

        private static readonly int[] Iterations = new[] { 1, 100, 10000, 1000000 };
        private static readonly object[] NoArgArray = new object[0];
        private static readonly object[] ArgArray = new object[]{10};

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
        }

        private static void RunConstructorBenchmark()
        {
            var ctorInfo = typeof (Person).GetConstructor(BindingFlags.Instance | 
                BindingFlags.NonPublic, null, new Type[0], null);
            ConstructorInvoker invoker = typeof(Person).DelegateForConstruct();
            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct ctor", () => new Person() },
                                  {"Reflection ctor", () => ctorInfo.Invoke(NoArgArray)},
                                  {"Fasterflect ctor", () => typeof(Person).Construct() },
                                  {"Fasterflect cached ctor", () => invoker(NoArgArray) },
                              };
            Execute("Construction", actionMap);
        }

        private static void RunFieldBenchmark()
        {
            var person = new Person();
            var fieldInfo = person.GetType().GetField("name", BindingFlags.NonPublic | BindingFlags.Instance);
            AttributeSetter setter = person.GetType().DelegateForSetField("name");
            AttributeGetter getter = person.GetType().DelegateForGetField("name");

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { person.name = "John"; }},
                                  {"Direct get", () => { var name = person.name; }},
                                  {"Reflection set", () => fieldInfo.SetValue(person, "John")},
                                  {"Reflection get", () => fieldInfo.GetValue(person)},
                                  {"Fasterflect set", () => person.SetField("name", "John")},
                                  {"Fasterflect get", () => person.GetField<string>("name")},
                                  {"Fasterflect cached set", () => setter(person, "John")},
                                  {"Fasterflect cached get", () => getter(person)},
                              };
            Execute("Field Accesses", actionMap);
        }

        private static void RunStaticFieldBenchmark()
        {
            Type type = typeof(Person);
            var fieldInfo = type.GetField("counter", BindingFlags.NonPublic | BindingFlags.Static);
            StaticAttributeSetter setter = type.DelegateForSetStaticField("counter");
            StaticAttributeGetter getter = type.DelegateForGetStaticField("counter");

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { Person.counter = 1; }},
                                  {"Direct get", () => { var counter = Person.counter; }},
                                  {"Reflection set", () => fieldInfo.SetValue(type, 1)},
                                  {"Reflection get", () => fieldInfo.GetValue(type)},
                                  {"Fasterflect set", () => type.SetField("counter", 1)},
                                  {"Fasterflect get", () => type.GetField<int>("counter")},
                                  {"Fasterflect cached set", () => setter(1)},
                                  {"Fasterflect cached get", () => getter()},
                              };
            Execute("Static Field Accesses", actionMap);
        }

        private static void RunPropertyBenchmark()
        {
            var person = new Person();
            var propInfo = person.GetType().GetProperty("Age", BindingFlags.NonPublic | BindingFlags.Instance);
            AttributeSetter setter = person.GetType().DelegateForSetProperty("Age");
            AttributeGetter getter = person.GetType().DelegateForGetProperty("Age");
      
            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { person.Age = 10; }},
                                  {"Direct get", () => { var age = person.Age; }},
                                  {"Reflection set", () => propInfo.SetValue(person, 10, null)},
                                  {"Reflection get", () => propInfo.GetValue(person, null)},
                                  {"Fasterflect set", () => person.SetProperty("Age", 10)},
                                  {"Fasterflect get", () => person.GetProperty<int>("Age")},
                                  {"Fasterflect cached set", () => setter(person, 10)},
                                  {"Fasterflect cached get", () => getter(person)},
                              };
            Execute("Property Accesses", actionMap);
        }

        private static void RunStaticPropertyBenchmark()
        {
            var type = typeof(Person);
            var person = new Person();
            var propInfo = person.GetType().GetProperty("Counter", BindingFlags.NonPublic | BindingFlags.Static);
            StaticAttributeSetter setter = person.GetType().DelegateForSetStaticProperty("Counter");
            StaticAttributeGetter getter = person.GetType().DelegateForGetStaticProperty("Counter");
         
            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { Person.Counter = 10; }},
                                  {"Direct get", () => { var counter = Person.Counter; }},
                                  {"Reflection set", () => propInfo.SetValue(type, 10, null)},
                                  {"Reflection get", () => propInfo.GetValue(type, null)},
                                  {"Fasterflect set", () => type.SetProperty("Counter", 10)},
                                  {"Fasterflect get", () => type.GetProperty<int>("Counter")},
                                  {"Fasterflect cached set", () => setter(10)},
                                  {"Fasterflect cached get", () => getter()},
                              };
            Execute("Static Property Accesses", actionMap);
        }

        private static void RunIndexerBenchmark()
        {
            var person = new Person();
            var setterInfo = person.GetType().GetMethod("set_Item", new[] { typeof(int), typeof(int), 
                typeof(object) });
            var getterInfo = person.GetType().GetMethod("get_Item", new[] { typeof(int), typeof(int) });
            MethodInvoker setter = person.GetType().DelegateForSetIndexer(new[] { typeof(int), typeof(int), typeof(object) });
            MethodInvoker getter = person.GetType().DelegateForGetIndexer(new[] { typeof(int), typeof(int) });

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct set", () => { person[1, 2] = null; }},
                                  {"Direct get", () => { var tmp = person[1, 2]; }},
                                  {"Reflection set", () => setterInfo.Invoke(person, new object[]{1, 2, null})},
                                  {"Reflection get", () => getterInfo.Invoke(person, new object[]{1, 2 })},
                                  {"Fasterflect set", () => person.SetIndexer(new[] { typeof(int), typeof(int), typeof(object) }, new object[]{1, 2, null})},
                                  {"Fasterflect get", () => person.GetIndexer<object>(new[] { typeof(int), typeof(int) }, new object[]{1, 2})},
                                  {"Fasterflect cached set", () => setter(person, new object[]{1, 2, null})},
                                  {"Fasterflect cached get", () => getter(person, new object[]{1, 2})},
                              };
            Execute("Indexer Invocation", actionMap);
        }

        private static void RunMethodInvocationBenchmark()
        {
            var person = new Person();
            var noArgMethodInfo = person.GetType().GetMethod("Walk", BindingFlags.NonPublic | BindingFlags.Instance,
                null, new Type[0], null);
            var argMethodInfo = person.GetType().GetMethod("Walk", BindingFlags.NonPublic | BindingFlags.Instance,
                null, new Type[]{typeof(int)}, null);

            MethodInvoker noArgInvoker = person.GetType().DelegateForInvoke("Walk");
            MethodInvoker argInvoker = person.GetType().DelegateForInvoke("Walk", new[] { typeof(int) });

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct invoke", () => person.Walk()},
                                  {"Direct invoke (arg)", () => person.Walk(10)},
                                  {"Reflection invoke", () => noArgMethodInfo.Invoke(person, NoArgArray)},
                                  {"Reflection invoke (arg)", () => argMethodInfo.Invoke(person, ArgArray)},
                                  {"Fasterflect invoke", () => person.Invoke("Walk")},
                                  {"Fasterflect invoke (arg)", () => person.Invoke("Walk", new[]{typeof(int)}, ArgArray)},
                                  {"Fasterflect cached invoke", () => noArgInvoker(person, NoArgArray)},
                                  {"Fasterflect cached invoke (arg)", () => argInvoker(person, ArgArray)}
                              };
            Execute("Method Invocations", actionMap);
        }

        private static void RunStaticMethodInvocationBenchmark()
        {
            var type = typeof(Person);
            var noArgMethodInfo = type.GetMethod("Generate", BindingFlags.NonPublic | BindingFlags.Static,
                null, new Type[0], null);
            var argMethodInfo = type.GetMethod("Generate", BindingFlags.NonPublic | BindingFlags.Static,
                null, new[] { typeof(int) }, null);

            StaticMethodInvoker noArgInvoker = type.DelegateForStaticInvoke("Generate");
            StaticMethodInvoker argInvoker = type.DelegateForStaticInvoke("Generate", new[] { typeof(int) });

            var actionMap = new Dictionary<string, Action>
                              {
                                  {"Direct invoke", () => Person.Generate() },
                                  {"Direct invoke (arg)", () => Person.Generate(10) },
                                  {"Reflection invoke", () => noArgMethodInfo.Invoke(type, NoArgArray)},
                                  {"Reflection invoke (arg)", () => argMethodInfo.Invoke(type, ArgArray)},
                                  {"Fasterflect invoke", () => type.Invoke("Generate")},
                                  {"Fasterflect invoke (arg)", () => type.Invoke("Generate", new[]{typeof(int)}, ArgArray)},
                                  {"Fasterflect cached invoke", () => noArgInvoker(NoArgArray)},
                                  {"Fasterflect cached invoke (arg)", () => argInvoker(ArgArray)}
                              };
            Execute("Static Method Invocations", actionMap);
        }

        private static void Execute(string name, Dictionary<string, Action> actionMap)
        {
            Console.WriteLine("------------- {0} ------------- ", name);
            foreach (var iteration in Iterations)
            {
                Console.WriteLine("* Executing for {0} iterations", iteration);
                foreach (var entry in actionMap)
                {
                    long start = Environment.TickCount;
                    for (int i = 0; i < iteration; i++)
                    {
                        entry.Value();
                    }
                    long end = Environment.TickCount;
                    Console.WriteLine("{0,-35} {1} millis", entry.Key + ":", end - start);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
