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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest
{
    [TestClass]
    public class AttributeTest
	{
		#region Sample Reflection Classes
		private class Person
        {
			public int? Id { get; set; }
			[DefaultValue(42)]
			private int status;
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

			public virtual int Status
			{
				get { return status; }
				protected set { status = value; }
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

		[Code("DPT")]
		private enum Department
		{
			[Code("DEV")]
			Development, 
			[Code("MKT")]
			Marketing, 
			[Code("SLS")]
			Sales,
		}

		[Code("Class"), DebuggerDisplay("ID={Id}")]
		private class Employee : Person
		{
			#pragma warning disable 0169, 0649
			[Code( "Field" )]
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

		#region Enumerations
		#region Attribute<T>()
		[TestMethod]
        public void TestFindSingleAttributeOnEnum()
        {
			CodeAttribute attr = typeof(Department).Attribute<CodeAttribute>();
			Assert.IsNotNull( attr );
			Assert.AreEqual( "DPT", attr.Code );
        }

		[TestMethod]
        public void TestFindSingleAttributeOnEnumField()
        {
			Department department = Department.Development;
			CodeAttribute attr = department.Attribute<CodeAttribute>();
			Assert.IsNotNull( attr );
			Assert.AreEqual( "DEV", attr.Code );
        }
		#endregion

		#region Attributes()
		[TestMethod]
        public void TestFindAllAttributesOnEnum()
        {
			IList<Attribute> attrs = typeof(Department).Attributes();
			Assert.IsNotNull( attrs );
			Assert.AreEqual( 1, attrs.Count );
        }

		[TestMethod]
        public void TestFindAllAttributesOnEnumField()
        {
			IList<Attribute> attrs = Department.Development.Attributes();
			Assert.IsNotNull( attrs );
			Assert.AreEqual( 1, attrs.Count );
        }

		[TestMethod]
        public void TestFindSpecificAttributesOnEnumField()
        {
			IList<Attribute> attrs = Department.Development.Attributes( typeof(CodeAttribute) );
			Assert.IsNotNull( attrs );
			Assert.AreEqual( 1, attrs.Count );
			attrs = Department.Development.Attributes( typeof(ConditionalAttribute) );
			Assert.IsNotNull( attrs );
			Assert.AreEqual( 0, attrs.Count );
        }
		#endregion
		#endregion

		#region Types
		#region Attribute<T>()
		[TestMethod]
        public void TestFindSingleAttributeOnType()
        {
			Type type = typeof(Employee);
			CodeAttribute attr = type.Attribute<CodeAttribute>();
			Assert.IsNotNull( attr );
        }
		#endregion

		#region Attributes()
		[TestMethod]
        public void TestFindAllAttributesOnType()
        {
			Type type = typeof(Employee);
			IList<Attribute> attrs = type.Attributes().ToList();
			Assert.IsNotNull( attrs );
			Assert.AreEqual( 2, attrs.Count );
			Assert.IsTrue( attrs[ 0 ] is CodeAttribute || attrs[ 1 ] is CodeAttribute );
			Assert.IsTrue( attrs[ 0 ] is DebuggerDisplayAttribute || attrs[ 1 ] is DebuggerDisplayAttribute );
        }

		[TestMethod]
        public void TestFindSpecifiedAttributesOnType()
        {
			Type type = typeof(Employee);
			IList<CodeAttribute> attrs = type.Attributes<CodeAttribute>().ToList();
			Assert.IsNotNull( attrs );
			Assert.AreEqual( 1, attrs.Count );
        }
		#endregion

		#region TypesWith()
		[TestMethod]
        public void TestFindTypesWith()
        {
			Assembly assembly = Assembly.GetExecutingAssembly();
			IList<Type> types = assembly.TypesWith( typeof(CodeAttribute) );
			Assert.IsNotNull( types );
			Assert.AreEqual( 2, types.Count );
        }

		[TestMethod]
        public void TestFindTypesWithGeneric()
        {
			Assembly assembly = Assembly.GetExecutingAssembly();
			IList<Type> types = assembly.TypesWith<CodeAttribute>();
			Assert.IsNotNull( types );
			Assert.AreEqual( 2, types.Count );
        }
		#endregion

		#region MembersWith()
		[TestMethod]
        public void TestFindMembersWith_PrivateField()
        {
			Type type = typeof(Employee);
			IList<MemberInfo> members = type.MembersWith( MemberTypes.Field, BindingFlags.NonPublic | BindingFlags.Instance, typeof(CodeAttribute) );
			Assert.IsNotNull( members );
			Assert.AreEqual( 1, members.Count );
        }

		[TestMethod]
        public void TestFindMembersWith_InheritedField()
        {
			Type type = typeof(Employee);
			IList<MemberInfo> members = type.MembersWith( MemberTypes.Field, BindingFlags.NonPublic | BindingFlags.Instance, typeof(DefaultValueAttribute) );
			Assert.IsNotNull( members );
			Assert.AreEqual( 1, members.Count );
        }

		[TestMethod]
        public void TestFindMembersWith_GenericWithMemberTypes()
        {
			Type type = typeof(Employee);
			IList<MemberInfo> members = type.MembersWith<CodeAttribute>( MemberTypes.Field );
			Assert.IsNotNull( members );
			Assert.AreEqual( 1, members.Count );
        }

		[TestMethod]
        public void TestFindMembersWith_GenericWithMemberTypesAndBindingFlags()
        {
			Type type = typeof(Employee);
			IList<MemberInfo> members = type.MembersWith<CodeAttribute>( MemberTypes.Field, BindingFlags.NonPublic | BindingFlags.Instance );
			Assert.IsNotNull( members );
			Assert.AreEqual( 1, members.Count );
        }
		#endregion

		#region MembersAndAttributes()
		[TestMethod]
        public void TestFindMembersAndAttributes()
        {
			Type type = typeof(Employee);
			var members = type.MembersAndAttributes( MemberTypes.Field, BindingFlags.NonPublic | BindingFlags.Instance, typeof(CodeAttribute) );
			Assert.IsNotNull( members );
			Assert.AreEqual( 1, members.Count );
			Assert.AreEqual( "lastSeen", members.Keys.First().Name );
        }
		#endregion
		#endregion

		#region Members
		#region Attribute<T>()
		[TestMethod]
        public void TestFindSingleAttributeOnMember()
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

		#region Attributes()
		[TestMethod]
        public void TestFindAllAttributesOnMember()
        {
			Type type = typeof(Employee);

			PropertyInfo info = type.Property( "Department" );
			Assert.IsNotNull( info );
			Assert.AreEqual( 0, info.Attributes().Count() );

			FieldInfo field = type.Field<DateTime>( "lastSeen" );
			Assert.IsNotNull( field );
			Assert.AreEqual( 1, field.Attributes().Count() );

			PropertyInfo property = type.Property<string>( "Initials" );
			Assert.IsNotNull( property );
			Assert.AreEqual( 1, property.Attributes().Count() );
        }

		[TestMethod]
        public void TestFindSpecifiedAttributesOnMember()
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
		#endregion
    }
}
