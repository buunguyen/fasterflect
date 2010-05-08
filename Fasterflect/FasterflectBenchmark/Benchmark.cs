#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
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
using System.Threading;
using Fasterflect;
using Fasterflect.Caching;
using Fasterflect.Emitter;
using Fasterflect.Probing;

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

            internal Person( int age, string name )
            {
                Age = age;
                this.name = name;
            }

            public object this[ int a, int b ]
            {
                get { return null; }
                set { }
            }

            internal int Age { get; set; }
            internal static int Counter { get; set; }

            internal void Walk()
            {
            }

            internal void Walk( int speed )
            {
            }

            internal static void Generate()
            {
            }

            internal static void Generate( int seed )
            {
            }

            internal string GetName()
            {
                return name;
            }

            internal string GetName( string prefix )
            {
                return prefix + " " + name;
            }
        }
        #endregion

        private static readonly int[] Iterations = new[] { 20000, 2000000 };
        private static readonly object[] NoArgArray = new object[0];
        private static readonly object[] ArgArray = new object[] { 10 };
        private static readonly Type TargetType = typeof(Person);
        private static readonly Person TargetPerson = new Person();
        private static readonly Person[] PeopleArray = new Person[100];
        private static readonly Stopwatch Watch = new Stopwatch();

        public static void Main( string[] args )
        {
            Prepare();
#if DOT_NET_4
            Console.SetOut( new StreamWriter( "benchmark.4.txt" ) );
#else
            Console.SetOut( new StreamWriter( "benchmark.35.txt" ) );
#endif
            //RunDictionaryBenchmark();
            //RunHashCodeBenchmark();
            //RunTryCreateInstanceBenchmark();
            RunConstructorBenchmark();
            RunFieldBenchmark();
            RunStaticFieldBenchmark();
            RunPropertyBenchmark();
            RunStaticPropertyBenchmark();
            RunMethodInvocationBenchmark();
            RunStaticMethodInvocationBenchmark();
            RunIndexerBenchmark();
            RunArrayBenchmark();
            RunMetadataIntegrationBenchmark();
            RunLookupBenchmark();
            Console.Out.Flush();
        }

        #region Internal Testing
        private static void RunDictionaryBenchmark()
        {
            int dictionarySize = 1000;
            int index = new Random( (int) (DateTime.Now.Ticks % int.MaxValue) ).Next( 0, dictionarySize );
            List<string> stringList =
                Enumerable.Range( 0, dictionarySize ).Select( s => Path.GetRandomFileName() + Path.GetRandomFileName() )
                    .ToList();
            Dictionary<string, string> stringDictionary = stringList.ToDictionary( s => s, s => s );
            var stringCache = new Cache<string, string>();
            stringList.ForEach( s => stringCache.Insert( s, s, CacheStrategy.Permanent ) );

            string key = stringList[ index ];
            var callInfo = new CallInfo( TargetType, Flags.InstanceAnyVisibility, MemberTypes.Field, "name",
                                         Constants.ArrayOfObjectType, null );

            var initMap = new Dictionary<string, Action> { };
            var actionMap = new Dictionary<string, Action>
                            {
                                { "Dictionary ContainsKey", () => stringDictionary.ContainsKey( key ) },
                                { "Dictionary Indexer", () => { var x = stringDictionary[ key ]; } },
                                {
                                    "Dictionary TryGetValue", () =>
                                                              {
                                                                  string s;
                                                                  stringDictionary.TryGetValue( key, out s );
                                                              }
                                    },
                                //{"List Contains", () => stringList.Contains( key ) },
                                //{"List Linq First", () => stringList.First( item => item == key ) },
                                { "Cache GetValue", () => stringCache.Get( key ) },
                            };
            Execute( "Dictionary Benchmark", initMap, actionMap );
        }

        private static void RunHashCodeBenchmark()
        {
            var callInfo = new CallInfo( TargetType, Flags.InstanceAnyVisibility, MemberTypes.Field, "name",
                                         new[] { typeof(int), typeof(string) }, null );
            var callInfoOther = new CallInfo( typeof(CallInfo), Flags.InstanceAnyVisibility, MemberTypes.Field, "other",
                                              new[] { typeof(string) }, null );
            var sourceInfo = new SourceInfo( new { ID = 42, Name = "Test" }.GetType() );
            var sourceInfoOther = new SourceInfo( new { id = 42, Name = "Test" }.GetType() );

            var initMap = new Dictionary<string, Action> { };
            var actionMap = new Dictionary<string, Action>
                            {
                                { "CallInfo GetHashCode", () => callInfo.GetHashCode() },
                                { "CallInfo Equals Other", () => callInfo.Equals( callInfoOther ) },
                                { "SourceInfo GetHashCode", () => sourceInfo.GetHashCode() },
                                { "SourceInfo Equals Other", () => sourceInfo.Equals( sourceInfoOther ) },
                                { "string GetHashCode", () => "foo".GetHashCode() },
                                { "string Equals", () => "foo".Equals( "bar" ) },
                                {
                                    "new CallInfo",
                                    () =>
                                    new CallInfo( TargetType, Flags.InstanceAnyVisibility, MemberTypes.Field, "name",
                                                  new[] { typeof(int), typeof(string) }, null )
                                    },
                                {
                                    "new SourceInfo",
                                    () =>
                                    new SourceInfo( TargetType, new[] { "ID", "Name" },
                                                    new[] { typeof(int), typeof(string) } )
                                    },
                                {
                                    "new SourceInfo anon",
                                    () => new SourceInfo( new { ID = 42, Name = "Test" }.GetType() )
                                    },
                            };
            Execute( "HashCode Benchmark", initMap, actionMap );
        }
        #endregion

        #region Fasterflect Lookup
        private static void RunLookupBenchmark()
        {
            var defaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var declaredOnlyFlags = defaultFlags | BindingFlags.DeclaredOnly;
            var initMap = new Dictionary<string, Action>
                          {
                              { "Init Flags", () => { var f = Flags.None; } },
                          };
            var actionMap = new Dictionary<string, Action>
                            {
                                { "--- Fields", () => { } },
                                { "Reflection GetField", () => typeof(Person).GetField( "name", defaultFlags ) },
                                { "Fasterflect Field", () => typeof(Person).Field( "name", declaredOnlyFlags ) },
                                { "Reflection GetFields", () => typeof(Person).GetFields( defaultFlags ) },
                                { "Fasterflect Fields", () => typeof(Person).Fields( declaredOnlyFlags ) },
                                { "Fasterflect Fields+1", () => typeof(Person).Fields( declaredOnlyFlags, "Name" ) },
                                {
                                    "Fasterflect Fields+5",
                                    () => typeof(Person).Fields( declaredOnlyFlags, "Name", "A", "B", "C", "D" )
                                    },
                                { "--- Properties", () => { } },
                                { "Reflection GetProperty", () => typeof(Person).GetProperty( "Name", defaultFlags ) },
                                { "Fasterflect Property", () => typeof(Person).Property( "Name", declaredOnlyFlags ) },
                                { "Reflection GetProperties", () => typeof(Person).GetProperties( defaultFlags ) },
                                { "Fasterflect Properties", () => typeof(Person).Properties( declaredOnlyFlags ) },
                                {
                                    "Fasterflect Properties+1",
                                    () => typeof(Person).Properties( declaredOnlyFlags, "Name" )
                                    },
                                {
                                    "Fasterflect Properties+5",
                                    () => typeof(Person).Properties( declaredOnlyFlags, "Name", "A", "B", "C", "D" )
                                    },
                                { "--- Members", () => { } },
                                { "Reflection GetMember", () => typeof(Person).GetMember( "name", defaultFlags ) },
                                { "Fasterflect Member", () => typeof(Person).Member( "name", declaredOnlyFlags ) },
                                { "Reflection GetMembers", () => typeof(Person).GetMembers( defaultFlags ) },
                                {
                                    "Fasterflect Members",
                                    () => typeof(Person).Members( MemberTypes.All, declaredOnlyFlags )
                                    },
                                {
                                    "Fasterflect Members+1",
                                    () => typeof(Person).Members( MemberTypes.All, declaredOnlyFlags, "Name" )
                                    },
                                {
                                    "Fasterflect Members+5",
                                    () =>
                                    typeof(Person).Members( MemberTypes.All, declaredOnlyFlags, "Name", "A", "B", "C",
                                                            "D" )
                                    },
                                { "--- Methods", () => { } },
                                {
                                    "Reflection GetMethod",
                                    () =>
                                    typeof(Person).GetMethod( "Walk", defaultFlags, null, new[] { typeof(int) }, null )
                                    },
                                {
                                    "Fasterflect Method",
                                    () => typeof(Person).Method( "Walk", new[] { typeof(int) }, declaredOnlyFlags )
                                    },
                                { "Reflection GetMethods", () => typeof(Person).GetMethods( defaultFlags ) },
                                { "Fasterflect Methods", () => typeof(Person).Methods( declaredOnlyFlags ) },
                                {
                                    "Fasterflect Methods+1",
                                    () => typeof(Person).Methods( new[] { typeof(int) }, declaredOnlyFlags )
                                    },
                                {
                                    "Fasterflect Methods+2",
                                    () =>
                                    typeof(Person).Methods( new[] { typeof(int), typeof(string) }, declaredOnlyFlags )
                                    },
                                {
                                    "Fasterflect Methods+1+1",
                                    () => typeof(Person).Methods( new[] { typeof(int) }, declaredOnlyFlags, "Walk" )
                                    },
                                {
                                    "Fasterflect Members M",
                                    () => typeof(Person).Members( MemberTypes.Method, declaredOnlyFlags )
                                    },
                                { "--- Constructors", () => { } },
                                {
                                    "Reflection GetConstructor",
                                    () =>
                                    typeof(Person).GetConstructor( defaultFlags, null, CallingConventions.Standard,
                                                                   Type.EmptyTypes, null )
                                    },
                                {
                                    "Fasterflect Constructor",
                                    () => typeof(Person).Constructor( declaredOnlyFlags, Type.EmptyTypes )
                                    },
                                { "Reflection GetConstructors", () => typeof(Person).GetConstructors( defaultFlags ) },
                                { "Fasterflect Constructors", () => typeof(Person).Constructors( declaredOnlyFlags ) },
                            };
            Execute( "Benchmark for Metadata Lookup", initMap, actionMap );
        }
        #endregion

        #region Fasterflect Core
        private static void RunTryCreateInstanceBenchmark()
        {
            var names = new string[] { "Age", "Name" };
            var types = new Type[] { typeof(int), typeof(string) };
            var values = new object[] { 42, "Arthur Dent" };

            var initMap = new Dictionary<string, Action> { };
            var actionMap = new Dictionary<string, Action>
                            {
                                { "TryCreateInstance [empty]", () => typeof(Person).TryCreateInstance( new { } ) },
                                {
                                    "TryCreateInstance [n+t+v]",
                                    () => typeof(Person).TryCreateInstance( names, types, values )
                                    },
                                {
                                    "TryCreateInstance [object]",
                                    () => typeof(Person).TryCreateInstance( new { Age = 42, Name = "Arthur Dent" } )
                                    },
                            };
            Execute( "TryCreateInstance Benchmark", initMap, actionMap );
        }

        private static void RunMetadataIntegrationBenchmark()
        {
            ConstructorInfo ctorInfo = null;
            PropertyInfo propInfo = null;
            PropertyInfo staticPropInfo = null;
            FieldInfo fieldInfo = null;
            FieldInfo staticFieldInfo = null;
            MethodInfo noArgMethodInfo = null;
            MethodInfo argMethodInfo = null;
            MethodInfo noArgStaticMethodInfo = null;
            MethodInfo argStaticMethodInfo = null;

            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init ctor info",
                                  () =>
                                  {
                                      ctorInfo =
                                          typeof(Person).GetConstructor(
                                              BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null );
                                  }
                                  },
                              {
                                  "Init property info",
                                  () =>
                                  {
                                      propInfo = TargetType.GetProperty( "Age",
                                                                         BindingFlags.NonPublic | BindingFlags.Instance );
                                  }
                                  },
                              {
                                  "Init static property info",
                                  () =>
                                  {
                                      staticPropInfo = TargetType.GetProperty( "Counter",
                                                                               BindingFlags.NonPublic |
                                                                               BindingFlags.Static );
                                  }
                                  },
                              {
                                  "Init field info",
                                  () =>
                                  {
                                      fieldInfo = TargetType.GetField( "name",
                                                                       BindingFlags.NonPublic | BindingFlags.Instance );
                                  }
                                  },
                              {
                                  "Init static field info",
                                  () =>
                                  {
                                      staticFieldInfo = TargetType.GetField( "counter",
                                                                             BindingFlags.NonPublic |
                                                                             BindingFlags.Static );
                                  }
                                  },
                              {
                                  "Init no-arg info",
                                  () =>
                                  {
                                      noArgMethodInfo = TargetType.GetMethod( "Walk",
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance, null, new Type[0],
                                                                              null );
                                  }
                                  },
                              {
                                  "Init arg info",
                                  () =>
                                  {
                                      argMethodInfo = TargetType.GetMethod( "Walk",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.Instance, null,
                                                                            new Type[] { typeof(int) }, null );
                                  }
                                  },
                              {
                                  "Init static no-arg info",
                                  () =>
                                  {
                                      noArgStaticMethodInfo = TargetType.GetMethod( "Generate",
                                                                                    BindingFlags.NonPublic |
                                                                                    BindingFlags.Static, null,
                                                                                    new Type[0], null );
                                  }
                                  },
                              {
                                  "Init static arg info",
                                  () =>
                                  {
                                      argStaticMethodInfo = TargetType.GetMethod( "Generate",
                                                                                  BindingFlags.NonPublic |
                                                                                  BindingFlags.Static, null,
                                                                                  new[] { typeof(int) }, null );
                                  }
                                  },
                          };

            var actionMap = new Dictionary<string, Action>
                            {
                                //{"--- Constructor", () => {} },
                                //{"Direct ctor", () => new Person()},
                                //{"Reflection ctor", () => ctorInfo.Invoke(NoArgArray)},
                                //{"Fasterflect ctor", () => ctorInfo.CreateInstance(NoArgArray)},
                                    
                                //{"--- Instance Field", () => {} },
                                //{"Direct instance field set", () => { TargetPerson.name = "John"; }},
                                //{"Direct instance field get", () => { string name = TargetPerson.name; }},
                                //{"Reflection instance field set", () => fieldInfo.SetValue(TargetPerson, "John")},
                                //{"Reflection instance field get", () => fieldInfo.GetValue(TargetPerson)},
                                //{"Fasterflect instance field set", () => fieldInfo.Set(TargetPerson, "John")},
                                //{"Fasterflect instance field get", () => fieldInfo.Get(TargetPerson)},
                                    
                                //{"--- Static Field", () => {} },
                                //{"Direct static field set", () => { Person.counter = 1; }},
                                //{"Direct static field get", () => { int counter = Person.counter; }},
                                //{"Reflection static field set", () => staticFieldInfo.SetValue(TargetType, 1)},
                                //{"Reflection static field get", () => staticFieldInfo.GetValue(TargetType)},
                                //{"Fasterflect static field set", () => staticFieldInfo.Set(1)},
                                //{"Fasterflect static field get", () => staticFieldInfo.Get()},
                                    
                                //{"--- Instance Property", () => {} },
                                //{"Direct instance property set", () => { TargetPerson.Age = 10; }},
                                //{"Direct instance property get", () => { int age = TargetPerson.Age; }},
                                //{"Reflection instance property set", () => propInfo.SetValue(TargetPerson, 10, null)},
                                //{"Reflection instance property get", () => propInfo.GetValue(TargetPerson, null)},
                                //{"Fasterflect instance property set", () => propInfo.Set(TargetPerson, 10)},
                                //{"Fasterflect instance property get", () => propInfo.Get(TargetPerson)},
                                    
                                //{"--- Static Property", () => {} },
                                //{"Direct static property set", () => { Person.Counter = 10; }},
                                //{"Direct static property get", () => { int counter = Person.Counter; }},
                                //{"Reflection static property set", () => staticPropInfo.SetValue(TargetType, 10, null)},
                                //{"Reflection static property get", () => staticPropInfo.GetValue(TargetType, null)},
                                //{"Fasterflect static property set", () => staticPropInfo.Set(10)},
                                //{"Fasterflect static property get", () => staticPropInfo.Get()},
                                    
                                //{"--- Instance Method", () => {} },
                                //{"Direct invoke", () => TargetPerson.Walk()},
                                //{"Direct invoke (arg)", () => TargetPerson.Walk(10)},
                                //{"Reflection invoke", () => noArgMethodInfo.Invoke(TargetPerson, NoArgArray)},
                                //{"Reflection invoke (arg)", () => argMethodInfo.Invoke(TargetPerson, ArgArray)},
                                //{"Fasterflect invoke", () => noArgMethodInfo.Call(TargetPerson)},
                                //{"Fasterflect invoke (arg)", () => argMethodInfo.Call(TargetPerson, ArgArray)},
                                    
                                { "--- Static Method", () => { } },
                                { "Direct static invoke", () => Person.Generate() },
                                { "Direct static invoke (arg)", () => Person.Generate( 10 ) },
                                {
                                    "Reflection static invoke",
                                    () => noArgStaticMethodInfo.Invoke( TargetType, NoArgArray )
                                    },
                                {
                                    "Reflection static invoke (arg)",
                                    () => argStaticMethodInfo.Invoke( TargetType, ArgArray )
                                    },
                                { "Fasterflect static invoke", () => noArgStaticMethodInfo.Call() },
                                { "Fasterflect static invoke (arg)", () => argStaticMethodInfo.Call( ArgArray ) },
                            };
            Execute( "Benchmark for Access Methods Enabled via .NET Reflection Metadata", initMap, actionMap );
        }

        private static void RunConstructorBenchmark()
        {
            ConstructorInfo ctorInfo = null;
            ConstructorInvoker invoker = null;

            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init info", () =>
                                               {
                                                   ctorInfo = typeof(Person).GetConstructor(
                                                       BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0],
                                                       null );
                                               }
                                  },
                              { "Init ctorInvoker", () => { invoker = typeof(Person).DelegateForCreateInstance(); } },
                          };
            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct ctor", () => new Person() },
                                { "Reflection ctor", () => ctorInfo.Invoke( NoArgArray ) },
                                { "Fasterflect ctor", () => typeof(Person).CreateInstance() },
                                { "Fasterflect cached ctor", () => invoker( NoArgArray ) },
                            };
            Execute( "Benchmark for Object Construction", initMap, actionMap );
        }

        private static void RunFieldBenchmark()
        {
            FieldInfo fieldInfo = null;
            MemberSetter setter = null;
            MemberGetter getter = null;
            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init info",
                                  () =>
                                  {
                                      fieldInfo = TargetType.GetField( "name",
                                                                       BindingFlags.NonPublic | BindingFlags.Instance );
                                  }
                                  },
                              { "Init setter", () => { setter = TargetType.DelegateForSetFieldValue( "name" ); } },
                              { "Init getter", () => { getter = TargetType.DelegateForGetFieldValue( "name" ); } }
                          };

#if DOT_NET_4
			dynamic tmp = TargetPerson;
#endif
            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct set", () => { TargetPerson.name = "John"; } },
                                { "Direct get", () => { string name = TargetPerson.name; } },
#if DOT_NET_4
									{"'dynamic' set", () => { tmp.name = "John"; }},
									{"'dynamic' get", () => { dynamic name = tmp.name; }},
#endif
                                { "Reflection set", () => fieldInfo.SetValue( TargetPerson, "John" ) },
                                { "Reflection get", () => fieldInfo.GetValue( TargetPerson ) },
                                { "Fasterflect set", () => TargetPerson.SetFieldValue( "name", "John" ) },
                                { "Fasterflect get", () => TargetPerson.GetFieldValue( "name" ) },
                                { "Fasterflect cached set", () => setter( TargetPerson, "John" ) },
                                { "Fasterflect cached get", () => getter( TargetPerson ) },
                            };
            Execute( "Benchmark for Field Access", initMap, actionMap );
        }

        private static void RunStaticFieldBenchmark()
        {
            FieldInfo fieldInfo = null;
            MemberSetter setter = null;
            MemberGetter getter = null;
            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init info",
                                  () =>
                                  {
                                      fieldInfo = TargetType.GetField( "counter",
                                                                       BindingFlags.NonPublic | BindingFlags.Static );
                                  }
                                  },
                              {
                                  "Init setter",
                                  () => { setter = TargetType.DelegateForSetFieldValue( "counter" ); }
                                  },
                              {
                                  "Init getter",
                                  () => { getter = TargetType.DelegateForGetFieldValue( "counter" ); }
                                  }
                          };

            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct set", () => { Person.counter = 1; } },
                                { "Direct get", () => { int counter = Person.counter; } },
                                { "Reflection set", () => fieldInfo.SetValue( TargetType, 1 ) },
                                { "Reflection get", () => fieldInfo.GetValue( TargetType ) },
                                { "Fasterflect set", () => TargetType.SetFieldValue( "counter", 1 ) },
                                { "Fasterflect get", () => TargetType.GetFieldValue( "counter" ) },
                                { "Fasterflect cached set", () => setter( null, 1 ) },
                                { "Fasterflect cached get", () => getter( null ) },
                            };
            Execute( "Benchmark for Static Field Access", initMap, actionMap );
        }

        private static void RunPropertyBenchmark()
        {
            PropertyInfo propInfo = null;
            MemberSetter setter = null;
            MemberGetter getter = null;
            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init info",
                                  () =>
                                  {
                                      propInfo = TargetType.GetProperty( "Age",
                                                                         BindingFlags.NonPublic | BindingFlags.Instance );
                                  }
                                  },
                              { "Init setter", () => { setter = TargetType.DelegateForSetPropertyValue( "Age" ); } },
                              { "Init getter", () => { getter = TargetType.DelegateForGetPropertyValue( "Age" ); } }
                          };

#if DOT_NET_4
			dynamic tmp = TargetPerson;
#endif
            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct set", () => { TargetPerson.Age = 10; } },
                                { "Direct get", () => { int age = TargetPerson.Age; } },
#if DOT_NET_4
			                		{"'dynamic' set", () => { tmp.Age = 10; }},
			                		{"'dynamic' get", () => { dynamic age = tmp.Age; }},
#endif
                                { "Reflection set", () => propInfo.SetValue( TargetPerson, 10, null ) },
                                { "Reflection get", () => propInfo.GetValue( TargetPerson, null ) },
                                { "Fasterflect set", () => TargetPerson.SetPropertyValue( "Age", 10 ) },
                                { "Fasterflect get", () => TargetPerson.GetPropertyValue( "Age" ) },
                                { "Fasterflect cached set", () => setter( TargetPerson, 10 ) },
                                { "Fasterflect cached get", () => getter( TargetPerson ) },
                            };
            Execute( "Benchmark for Property Access", initMap, actionMap );
        }

        private static void RunStaticPropertyBenchmark()
        {
            PropertyInfo propInfo = null;
            MemberSetter setter = null;
            MemberGetter getter = null;

            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init info",
                                  () =>
                                  {
                                      propInfo = TargetType.GetProperty( "Counter",
                                                                         BindingFlags.NonPublic | BindingFlags.Static );
                                  }
                                  },
                              {
                                  "Init setter",
                                  () => { setter = TargetType.DelegateForSetPropertyValue( "Counter" ); }
                                  },
                              {
                                  "Init getter",
                                  () => { getter = TargetType.DelegateForGetPropertyValue( "Counter" ); }
                                  }
                          };

            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct set", () => { Person.Counter = 10; } },
                                { "Direct get", () => { int counter = Person.Counter; } },
                                { "Reflection set", () => propInfo.SetValue( TargetType, 10, null ) },
                                { "Reflection get", () => propInfo.GetValue( TargetType, null ) },
                                { "Fasterflect set", () => TargetType.SetPropertyValue( "Counter", 10 ) },
                                { "Fasterflect get", () => TargetType.GetPropertyValue( "Counter" ) },
                                { "Fasterflect cached set", () => setter( null, 10 ) },
                                { "Fasterflect cached get", () => getter( null ) },
                            };
            Execute( "Benchmark for Static Property Access", initMap, actionMap );
        }

        private static void RunIndexerBenchmark()
        {
            MethodInfo setterInfo = null;
            MethodInfo getterInfo = null;
            MethodInvoker setter = null;
            MethodInvoker getter = null;

            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init setter info",
                                  () =>
                                  {
                                      setterInfo = TargetType.GetMethod( "set_Item",
                                                                         new[]
                                                                         { typeof(int), typeof(int), typeof(object) } );
                                  }
                                  },
                              {
                                  "Init getter info",
                                  () => { getterInfo = TargetType.GetMethod( "get_Item", new[] { typeof(int), typeof(int) } ); }
                                  },
                              {
                                  "Init setter",
                                  () =>
                                  {
                                      setter =
                                          TargetType.DelegateForSetIndexer( new[]
                                                                            { typeof(int), typeof(int), typeof(object) } );
                                  }
                                  },
                              {
                                  "Init getter",
                                  () => { getter = TargetType.DelegateForGetIndexer( new[] { typeof(int), typeof(int) } ); }
                                  }
                          };

#if DOT_NET_4
			dynamic tmp = TargetPerson;
#endif
            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct set", () => { TargetPerson[ 1, 2 ] = null; } },
                                { "Direct get", () => { object t = TargetPerson[ 1, 2 ]; } },
#if DOT_NET_4
			                		{"'dynamic' set", () => { tmp[1, 2] = null; }},
			                		{"'dynamic' get", () => { dynamic t = tmp[1, 2]; }},
#endif
                                {
                                    "Reflection set", () => setterInfo.Invoke( TargetPerson, new object[] { 1, 2, null } )
                                    },
                                { "Reflection get", () => getterInfo.Invoke( TargetPerson, new object[] { 1, 2 } ) },
                                {
                                    "Fasterflect set",
                                    () =>
                                    TargetPerson.SetIndexer( new[] { typeof(int), typeof(int), typeof(object) },
                                                             new object[] { 1, 2, null } )
                                    },
                                {
                                    "Fasterflect get",
                                    () =>
                                    TargetPerson.GetIndexer( new[] { typeof(int), typeof(int) }, new object[] { 1, 2 } )
                                    },
                                { "Fasterflect cached set", () => setter( TargetPerson, 1, 2, null ) },
                                { "Fasterflect cached get", () => getter( TargetPerson, 1, 2 ) },
                            };
            Execute( "Benchmark for Indexer Access", initMap, actionMap );
        }

        private static void RunMethodInvocationBenchmark()
        {
            MethodInfo noArgMethodInfo = null;
            MethodInfo argMethodInfo = null;

            MethodInvoker noArgInvoker = null;
            MethodInvoker argInvoker = null;

            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init no-arg info", () =>
                                                      {
                                                          noArgMethodInfo = TargetType.GetMethod( "Walk",
                                                                                                  BindingFlags.NonPublic |
                                                                                                  BindingFlags.Instance,
                                                                                                  null, new Type[0],
                                                                                                  null );
                                                      }
                                  },
                              {
                                  "Init arg info", () =>
                                                   {
                                                       argMethodInfo = TargetType.GetMethod( "Walk",
                                                                                             BindingFlags.NonPublic |
                                                                                             BindingFlags.Instance, null,
                                                                                             new Type[] { typeof(int) },
                                                                                             null );
                                                   }
                                  },
                              {
                                  "Init no-arg invoker",
                                  () => { noArgInvoker = TargetType.DelegateForCallMethod( "Walk" ); }
                                  },
                              {
                                  "Init arg invoker",
                                  () => { argInvoker = TargetType.DelegateForCallMethod( "Walk", new[] { typeof(int) } ); }
                                  }
                          };

#if DOT_NET_4
			dynamic tmp = TargetPerson;
#endif
            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct invoke", () => TargetPerson.Walk() },
                                { "Direct invoke (arg)", () => TargetPerson.Walk( 10 ) },
#if DOT_NET_4
			                		{"'dynamic' invoke", () => { tmp.Walk(); }},
			                		{"'dynamic' invoke (arg)", () => { tmp.Walk(10); }},
#endif
                                { "Reflection invoke", () => noArgMethodInfo.Invoke( TargetPerson, NoArgArray ) },
                                { "Reflection invoke (arg)", () => argMethodInfo.Invoke( TargetPerson, ArgArray ) },
                                { "Fasterflect invoke", () => TargetPerson.CallMethod( "Walk" ) },
                                {
                                    "Fasterflect invoke (arg)",
                                    () => TargetPerson.CallMethod( "Walk", new[] { typeof(int) }, ArgArray )
                                    },
                                { "Fasterflect cached invoke", () => noArgInvoker( TargetPerson, NoArgArray ) },
                                { "Fasterflect cached invoke (arg)", () => argInvoker( TargetPerson, ArgArray ) }
                            };
            Execute( "Benchmark for Method Invocation", initMap, actionMap );
        }

        private static void RunStaticMethodInvocationBenchmark()
        {
            MethodInfo noArgMethodInfo = null;
            MethodInfo argMethodInfo = null;
            MethodInvoker noArgInvoker = null;
            MethodInvoker argInvoker = null;

            var initMap = new Dictionary<string, Action>
                          {
                              {
                                  "Init no-arg info", () =>
                                                      {
                                                          noArgMethodInfo = TargetType.GetMethod( "Generate",
                                                                                                  BindingFlags.NonPublic |
                                                                                                  BindingFlags.Static,
                                                                                                  null, new Type[0],
                                                                                                  null );
                                                      }
                                  },
                              {
                                  "Init arg info", () =>
                                                   {
                                                       argMethodInfo = TargetType.GetMethod( "Generate",
                                                                                             BindingFlags.NonPublic |
                                                                                             BindingFlags.Static,
                                                                                             null, new[] { typeof(int) },
                                                                                             null );
                                                   }
                                  },
                              {
                                  "Init no-arg invoker",
                                  () => { noArgInvoker = TargetType.DelegateForCallMethod( "Generate" ); }
                                  },
                              {
                                  "Init arg invoker",
                                  () =>
                                  {
                                      argInvoker = TargetType.DelegateForCallMethod( "Generate",
                                                                                           new[] { typeof(int) } );
                                  }
                                  }
                          };

            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct invoke", () => Person.Generate() },
                                { "Direct invoke (arg)", () => Person.Generate( 10 ) },
                                { "Reflection invoke", () => noArgMethodInfo.Invoke( TargetType, NoArgArray ) },
                                { "Reflection invoke (arg)", () => argMethodInfo.Invoke( TargetType, ArgArray ) },
                                { "Fasterflect invoke", () => TargetType.CallMethod( "Generate" ) },
                                {
                                    "Fasterflect invoke (arg)",
                                    () => TargetType.CallMethod( "Generate", new[] { typeof(int) }, ArgArray )
                                    },
                                { "Fasterflect cached invoke", () => noArgInvoker( null, NoArgArray ) },
                                { "Fasterflect cached invoke (arg)", () => argInvoker( null, ArgArray ) }
                            };
            Execute( "Benchmark for Static Method Invocation", initMap, actionMap );
        }

        private static void RunArrayBenchmark()
        {
            ArrayElementGetter getter = null;
            ArrayElementSetter setter = null;

            var initMap = new Dictionary<string, Action>
                          {
                              { "Init setter", () => { setter = PeopleArray.GetType().DelegateForSetElement(); } },
                              { "Init getter", () => { getter = PeopleArray.GetType().DelegateForGetElement(); } }
                          };

            var actionMap = new Dictionary<string, Action>
                            {
                                { "Direct set", () => { PeopleArray[ 5 ] = null; } },
                                { "Direct get", () => { var person = PeopleArray[ 5 ]; } },
                                { "Reflection set", () => ((Array) (object) PeopleArray).SetValue( null, 5 ) },
                                { "Reflection get", () => ((Array) (object) PeopleArray).GetValue( 5 ) },
                                { "Fasterflect set", () => PeopleArray.SetElement( 5, null ) },
                                { "Fasterflect get", () => PeopleArray.GetElement( 5 ) },
                                { "Fasterflect cached set", () => setter( PeopleArray, 5, null ) },
                                { "Fasterflect cached get", () => getter( PeopleArray, 5 ) },
                            };
            Execute( "Benchmark for Array Element Access", initMap, actionMap );
        }
        #endregion

        #region Execute & Measure
        private static void Execute( string name, Dictionary<string, Action> initMap,
                                     Dictionary<string, Action> actionMap )
        {
            Console.WriteLine( "!!!! {0}", name );

            Console.WriteLine( "||Initialization|| ||" );
            Measure( Watch, initMap, 1 );
            Console.WriteLine();

            foreach( int iterationCount in Iterations )
            {
                Console.WriteLine( "||Executing for {0} iterations|| ||", iterationCount );
                Measure( Watch, actionMap, iterationCount );
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void Measure( Stopwatch watch, Dictionary<string, Action> actionMap,
                                     int iterationCount )
        {
            foreach( var entry in actionMap )
            {
                watch.Start();
                for( int i = 0; i < iterationCount; i++ )
                {
                    entry.Value();
                }
                watch.Stop();
                Console.WriteLine( "|{0,-35} | {1,6} ms|", entry.Key + ":", watch.ElapsedMilliseconds );
                watch.Reset();
            }
        }
        #endregion

        #region Prepare Environment
		private static void Prepare()
		{
			CollectGarbage();
			IncreaseThreadAndProcessPriority();
		}

    	private static void CollectGarbage()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			GC.WaitForFullGCComplete();
		}

		private static void IncreaseThreadAndProcessPriority()
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
			Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
		}
        #endregion
    }
}