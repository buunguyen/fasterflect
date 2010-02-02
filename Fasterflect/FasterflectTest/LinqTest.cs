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
using System.Linq;
using System.Reflection;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class LinqTest
	{
		#region Sample Reflection Classes
		private class Person
        {
            public int? Id { get; set; }
            public DateTime Birthday { get; set; }
            public string Name { get; set; }
            public int Age { get { return DateTime.Now.Year - Birthday.Year + (DateTime.Now.DayOfYear >= Birthday.DayOfYear ? 1 : 0); } }
        	public int ConstructorInstanceUsed { get; private set; }

            public Person()
            {
            	ConstructorInstanceUsed = 1;
            }

        	public Person( int id )
        	{
        		Id = id;
            	ConstructorInstanceUsed = 2;
        	}

        	public Person( DateTime birthday, string name )
        	{
        		Birthday = birthday;
        		Name = name;
            	ConstructorInstanceUsed = 3;
        	}

        	public Person( int id, DateTime birthday, string name )
        	{
        		Id = id;
        		Birthday = birthday;
        		Name = name;
            	ConstructorInstanceUsed = 4;
        	}
		}

		[AttributeUsage(AttributeTargets.All)]
		private class CodeAttribute : Attribute {
			public string Code { get; set; }

			public CodeAttribute( string code )
			{
				Code = code;
			}
		}

		private enum Department
		{
			[Code("DEV")]
			Development, 
			[Code("MKT")]
			Marketing, 
			[Code("SLS")]
			Sales,
		}

		[Code("Class")]
		private class Employee : Person
		{
			[Code( "Field" )]
			#pragma warning disable 0169, 0649
			private DateTime lastSeen;
			#pragma warning restore 0169, 0649
			[Code("Property")]
			public string Initials { get; private set; }
			public Department Department { get; private set; }

			[Code("Constructor")]
			public Employee( int id, string initials ) : base( id )
			{
				Initials = initials;
			}
			[Code("Constructor")]
			public Employee( int id, string initials, Department department ) : base( id )
			{
				Initials = initials;
				Department = department;
			}
		}
		#endregion

		#region Constructor Methods
		[TestMethod]
        public void TestConstructorLookup()
        {
        	Type type = typeof(Employee);
			IList<ConstructorInfo> constructors = type.Constructors().ToList();
			Assert.IsNotNull( constructors );
			Assert.AreEqual( 2, constructors.Count );
        }
		#endregion

		#region Attribute Methods
		[TestMethod]
        public void TestFindAttributeOnEnumField()
        {
			Department department = Department.Development;
			CodeAttribute attr = department.Attribute<CodeAttribute>();
			Assert.IsNotNull( attr );
			Assert.AreEqual( "DEV", attr.Code );
        }

		[TestMethod]
        public void TestFindAllAttributesOnType()
        {
			Type type = typeof(Employee);
			IList<Attribute> attrs = type.Attributes().ToList();
			Assert.IsNotNull( attrs );
			Assert.AreEqual( 1, attrs.Count );
			Assert.IsTrue( attrs[ 0 ] is CodeAttribute );
        }

		[TestMethod]
        public void TestFindSpecificAttributeOnType()
        {
			Type type = typeof(Employee);
			IList<CodeAttribute> attrs = type.Attributes<CodeAttribute>().ToList();
			Assert.IsNotNull( attrs );
			Assert.AreEqual( 1, attrs.Count );
        }

		[TestMethod]
        public void TestFindAllAttributesOnMember()
        {
			Type type = typeof(Employee);

			PropertyInfo info = type.Property( "Department" );
			Assert.IsNotNull( info );
			Assert.AreEqual( 0, info.Attributes<CodeAttribute>().Count() );

			FieldInfo field = type.Field<DateTime>( "lastSeen" );
			Assert.IsNotNull( field );
			Assert.AreEqual( 1, field.Attributes<CodeAttribute>().Count() );

			PropertyInfo property = type.Property<string>( "Initials" );
			Assert.IsNotNull( property );
			Assert.AreEqual( 1, property.Attributes<CodeAttribute>().Count() );
        }
		#endregion
    }
}
