#### NuGet
Fasterflect is available on NuGet: https://www.nuget.org/packages/fasterflect/

#### Overview
If you frequently use .NET Reflection in your application, you would immediately be aware of its two big issues: usability of the API as a whole and performance of reflection invocations (e.g. method invocation or member access). Fasterflect addresses the usability issue by providing intuitive & highly flexible extension methods to Type, Object and various metadata classes (such as FieldInfo, MethodInfo, etc.) which not only preserve but also enhance the .NET Reflection metadata lookup capability. The performance issue of reflection invocation is addressed by Fasterflect through the use of lightweight dynamic code generation, which performs anywhere from 5 to many hundreds times faster than native .NET Reflection invocations.

Fasterflect offers 3 major areas of functionality:

* *Querying:* Fasterflect allows you to query .NET metadata, such as looking-up types in an assembly, searching for methods matching a partial name, finding all constructors of a type, etc.  
* *Accessing:* Fasterflect allows you to perform reflective invocations on constructors, indexers, fields, properties and methods for both reference types (including arrays) and struct types.  Fasterflect also supports working with ref/out parameters, inferring parameter types to simplify invocations. These are backed by the dynamic code generation mechanism.
* *Services:* Built on top of the core API for querying and accessing, Fasterflect includes several extensions at at higher level of abstraction. This includes functionality for deep object cloning, object materialization (construction) without relying on default constructors, various object mapping scenarios, XML serialization, etc.

Besides being simple, Fasterflect is heavily documented and unit-tested, so you'll find it to be very easy to learn and use. Kickstart your reflection skills by downloading Fasterflect today. We recommend that you start by spending a bit of time with the extensive [documentation](http://fasterflect.codeplex.com/documentation), but the impatient need only write _using Fasterflect;_. We also recommend looking at the unit tests in case the documentation proves to be insufficient. 

To get a feel of what Fasterflect is like, without first having to download anything or spend time reading the [documentation](http://fasterflect.codeplex.com/documentation), below is some sample code using Fasterflect to construct objects and access members.

```csharp
class Person
{
    private int id;
    private int milesTraveled;
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    public string Name { get; private set; }
    private static int InstanceCount;

    public Person() : this(0) { }

    public Person(int id) : this(id, string.Empty) { }

    public Person(int id, string name)
    {
        Id = id;
        Name = name;
        InstanceCount++;
    }

    public char this[int index]
    {
        get { return Name[index]; }
    }

    private void Walk(int miles)
    {
        milesTraveled += miles;
    }

    private static void IncreaseInstanceCount()
    {
        InstanceCount++;
    }

    private static int GetInstanceCount()
    {
        return InstanceCount;
    }

    public static void Swap(ref int i, ref int j)
    {
        int tmp = i;
        i = j;
        j = tmp;
    }
}

class Program
{
    static void Main()
    {
        var type = Assembly.GetExecutingAssembly().GetType( "FasterflectSample.Person" );
        ExecuteNormalApi(type);
        ExecuteCacheApi(type);
    }

    private static void ExecuteNormalApi(Type type)
    {
        // Person.InstanceCount should be 0 since no instance is created yet
        AssertTrue((int)type.GetFieldValue("InstanceCount") == 0);
        
        // Invokes the no-arg constructor
        object obj = type.CreateInstance();

        // Double-check if the constructor is invoked successfully or not
        AssertTrue(null != obj);

        // Now, Person.InstanceCount should be 1
        AssertTrue(1 == (int)type.GetFieldValue("InstanceCount"));

        // We can bypass the constructor to change the value of Person.InstanceCount directly
        type.SetFieldValue("InstanceCount", 2);
        AssertTrue(2 == (int)type.GetFieldValue("InstanceCount"));

        // Let's invoke Person.IncreaseCounter() static method to increase the counter
        type.CallMethod("IncreaseInstanceCount");
        AssertTrue(3 == (int)type.GetFieldValue("InstanceCount"));

        // Now, let's retrieve Person.InstanceCount via the static method GetInstanceCount
        AssertTrue(3 == (int)type.CallMethod("GetInstanceCount"));

        // Invoke method receiving ref/out params, we need to put arguments in an array
        var arguments = new object[] { 1, 2 };
        type.CallMethod("Swap", 
            // Parameter types must be set to the appropriate ref type
            new[] { typeof(int).MakeByRefType(), typeof(int).MakeByRefType() },
            arguments);
        AssertTrue(2 == (int)arguments[0]);
        AssertTrue(1 == (int)arguments[1]);

        // Now, invoke the 2-arg constructor.  We don't even have to specify parameter types
        // if we know that the arguments are not null (Fasterflect will call arg[n].GetType() internally).
        obj = type.CreateInstance(1, "Doe");

        // id and name should have been set properly
        AssertTrue(1 == (int)obj.GetFieldValue("id"));
        AssertTrue("Doe" == obj.GetPropertyValue("Name").ToString());

        // Let's use the indexer to retrieve the character at index 1
        AssertTrue('o' == (char)obj.GetIndexer(1));

        // If there's null argument, or when we're unsure whether there's a null argument
        // we must explicitly specify the param type array
        obj = type.CreateInstance( new[] { typeof(int), typeof(string) }, 1, null );

        // id and name should have been set properly
        AssertTrue(1 == (int)obj.GetFieldValue("id"));
        AssertTrue(null == obj.GetPropertyValue("Name"));

        // Now, modify the id
        obj.SetFieldValue("id", 2);
        AssertTrue(2 == (int)obj.GetFieldValue("id"));
        AssertTrue(2 == (int)obj.GetPropertyValue("Id"));

        // We can chain calls
        obj.SetFieldValue("id", 3)
           .SetPropertyValue("Name", "Buu");
        AssertTrue(3 == (int)obj.GetPropertyValue("Id"));
        AssertTrue("Buu" == (string)obj.GetPropertyValue("Name"));
         
        // Map a set of properties from a source to a target
        new { Id = 4, Name = "Nguyen" }.MapProperties( obj );
        AssertTrue(4 == (int)obj.GetPropertyValue("Id"));
        AssertTrue("Nguyen" == (string)obj.GetPropertyValue("Name"));

        // Let's have the folk walk 6 miles
	obj.CallMethod("Walk", 6);
		
        // Double-check the current value of the milesTravelled field
        AssertTrue(6 == (int)obj.GetFieldValue("milesTraveled"));

        // Construct an array of 10 elements for current type
        var arr = type.MakeArrayType().CreateInstance(10);

        // GetValue & set element of array
        obj = type.CreateInstance();
        arr.SetElement(4, obj)
           .SetElement(9, obj);

        AssertTrue(obj == arr.GetElement(4));
        AssertTrue(obj == arr.GetElement(9));
        AssertTrue(null == arr.GetElement(0));
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
            object obj = ctor(i, "_" + i);
            AssertTrue(++currentInstanceCount == (int)count());
            AssertTrue(i == (int)obj.GetFieldValue("id"));
            AssertTrue("_" + i == obj.GetPropertyValue("Name").ToString());
        });

        // Getter/setter
        MemberSetter nameSetter = type.DelegateForSetPropertyValue("Name");
        MemberGetter nameGetter = type.DelegateForGetPropertyValue("Name");

        object person = ctor(1, "John");
        AssertTrue("John" == (string)nameGetter(person));
        nameSetter(person, "Jane");
        AssertTrue("Jane" == (string)nameGetter(person));

        // Invoke method
        person = type.CreateInstance();
        MethodInvoker walk = type.DelegateForCallMethod("Walk", new[] { typeof(int) });
        range.ForEach(i => walk(person, i));
        AssertTrue(range.Sum() == (int)person.GetFieldValue("milesTraveled"));
        
        // Map properties
        var ano = new { Id = 4, Name = "Doe" };
        var mapper = ano.GetType().DelegateForMap( type );
        mapper(ano, person);
        AssertTrue(4 == (int)person.GetPropertyValue("Id"));
        AssertTrue("Doe" == (string)person.GetPropertyValue("Name"));

    }

    public static void AssertTrue(bool expression)
    {
        if (!expression)
            throw new Exception("Not true");
        Console.WriteLine("Ok!");
    }
}
```

#### Copyright

Copyright 2010 Buu Nguyen, Morten Mertner
