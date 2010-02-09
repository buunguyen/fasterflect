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
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace FasterflectSample
{
    class Program
    {
        static void Main()
        {
            // Load a type reflectively, just to look like real-life scenario
            var types = new[]
                            {
                                Assembly.GetExecutingAssembly().GetType("FasterflectSample.PersonClass"),
                                Assembly.GetExecutingAssembly().GetType("FasterflectSample.PersonStruct")
                            };
            Array.ForEach(types, type =>
                                     {
                                         ExecuteNormalApi(type);
                                         ExecuteCacheApi(type);
                                     });
        }

        private static void ExecuteNormalApi(Type type)
        {
            bool isStruct = type.IsValueType;

            // Person.InstanceCount should be 0 since no instance is created yet
            AssertTrue((int)type.GetFieldValue("InstanceCount") == 0);
            
            // Invokes the no-arg constructor
            object obj = type.CreateInstance();

            // Double-check if the constructor is invoked successfully or not
            AssertTrue(null != obj);

            // struct's no-arg constructor cannot be overriden, thus the following checking
            // is not applicable to struct type
            if (!isStruct)
            {
                // Now, Person.InstanceCount should be 1
                AssertTrue(1 == (int)type.GetFieldValue("InstanceCount"));

                // What if we don't know the type of InstanceCount?  
                // Just specify object as the type parameter
                AssertTrue(type.GetFieldValue("InstanceCount") != null);
            }

            // We can bypass the constructor to change the value of Person.InstanceCount
            type.SetFieldValue("InstanceCount", 2);
            AssertTrue(2 == (int)type.GetFieldValue("InstanceCount"));

            // Let's invoke Person.IncreaseCounter() static method to increase the counter
            // In fact, let's chain the calls to increase 2 times
            type.Invoke("IncreaseInstanceCount")
                .Invoke("IncreaseInstanceCount");
            AssertTrue(4 == (int)type.GetFieldValue("InstanceCount"));

            // Now, let's retrieve Person.InstanceCount via the static method GetInstanceCount
            AssertTrue(4 == (int)type.Invoke("GetInstanceCount"));

            // Invoke method receiving ref/out params, we need to put arguments in an array
            var arguments = new object[] { 1, 2 };
            type.Invoke("Swap", 
                // Parameter types must be set to the appropriate ref type
                new[] { typeof(int).MakeByRefType(), typeof(int).MakeByRefType() },
                arguments);
            AssertTrue(2 == (int)arguments[0]);
            AssertTrue(1 == (int)arguments[1]);

            // Now, invoke the 2-arg constructor.  We don't even have to specify parameter types
            // if we know that the arguments are not null (Fasterflect will internally retrieve type info).
            obj = type.CreateInstance(1, "Doe");

            // Due to struct type's pass-by-value nature, in order for struct to be used 
            // properly with Fasterflect, you need to convert it into a holder (wrapper) first.  
            // The call below does nothing if obj is reference type so when unsure, just call it.
            obj = obj.WrapIfValueType();

            // id and name should have been set properly
            AssertTrue(1 == (int)obj.GetFieldValue("id"));
            AssertTrue("Doe" == obj.GetPropertyValue("Name").ToString());

            // Let's use the indexer to retrieve the character at index 1
            AssertTrue('o' == (char)obj.GetIndexer(1));

            // If there's null argument, or when we're unsure whether there's a null argument
            // we must explicitly specify the param type array
            obj = type.CreateInstance(new[] { typeof(int), typeof(string) }, new object[] { 1, null })
                .WrapIfValueType();

            // id and name should have been set properly
            AssertTrue(1 == (int)obj.GetFieldValue("id"));
            AssertTrue(null == obj.GetPropertyValue("Name"));

            // Now, modify the id
            obj.SetFieldValue("id", 2);
            AssertTrue(2 == (int)obj.GetFieldValue("id"));
            AssertTrue(2 == (int)obj.GetPropertyValue("Id"));

            // We can chain calls
            obj.SetFieldValue("id", 3).SetPropertyValue("Name", "Buu");
            AssertTrue(3 == (int)obj.GetPropertyValue("Id"));
            AssertTrue("Buu" == obj.GetPropertyValue("Name").ToString());
             
            // How about modifying both properties at the same time using an anonymous sample
            obj.SetProperties(new {
                                      Id = 4, 
                                      Name = "Nguyen"
                                  });
            AssertTrue(4 == (int)obj.GetPropertyValue("Id"));
            AssertTrue("Nguyen" == obj.GetPropertyValue("Name").ToString());

            // Let's have the folk walk 6 miles (and try chaining again)
            obj.Invoke("Walk", 1).Invoke("Walk", 2).Invoke("Walk", 3);

            // Double-check the current value of the milesTravelled field
            AssertTrue(6 == (int)obj.GetFieldValue("milesTraveled"));

            // Construct an array of 10 elements for current type
            var arr = type.MakeArrayType().CreateInstance(10);

            // GetValue & set element of array
            obj = type.CreateInstance();
            arr.SetElement(4, obj).SetElement(9, obj);

            if (isStruct) // struct, won't have same reference
            {
                AssertTrue(obj.Equals(arr.GetElement(4)));
                AssertTrue(obj.Equals(arr.GetElement(9)));
            }
            else 
            {
                AssertTrue(obj == arr.GetElement(4));
                AssertTrue(obj == arr.GetElement(9));
            }

            // Remember, struct array doesn't have null element 
            // (instead always initialized to default struct)
            if (!isStruct)
            {
                AssertTrue(null == arr.GetElement(0));
            }
        }

        private static void ExecuteCacheApi(Type type)
        {
            var range = Enumerable.Range(0, 10).ToList();

            // Let's cache the getter for InstanceCount
            StaticMemberGetter count = type.DelegateForGetStaticFieldValue("InstanceCount");

            // Now cache the 2-arg constructor of Person and playaround with the delegate returned
            int currentInstanceCount = (int)count();
            ConstructorInvoker ctor = type.DelegateForCreateInstance(new[] { typeof(int), typeof(string) });
            range.ForEach(i =>
            {
                object obj = ctor(i, "_" + i).WrapIfValueType();
                AssertTrue(++currentInstanceCount == (int)count());
                AssertTrue(i == (int)obj.GetFieldValue("id"));
                AssertTrue("_" + i == obj.GetPropertyValue("Name").ToString());
            });

            // Whatever thing we can do with the normal API, we can do with the cache API.
            // For example:
            MemberSetter nameSetter = type.DelegateForSetPropertyValue("Name");
            MemberGetter nameGetter = type.DelegateForGetPropertyValue("Name");

            object person = ctor(1, "Buu").WrapIfValueType();
            AssertTrue("Buu" == (string)nameGetter(person));
            nameSetter(person, "Doe");
            AssertTrue("Doe" == (string)nameGetter(person));

            // Another example
            person = type.CreateInstance().WrapIfValueType();
            MethodInvoker walk = type.DelegateForInvoke("Walk", new[] { typeof(int) });
            range.ForEach(i => walk(person, i));
            AssertTrue(range.Sum() == (int)person.GetFieldValue("milesTraveled"));
        }

        public static void AssertTrue(bool expression)
        {
            if (!expression)
                throw new Exception("Not true");
            Console.WriteLine("Ok!");
        }
    }
}
