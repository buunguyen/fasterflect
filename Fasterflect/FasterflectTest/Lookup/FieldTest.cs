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
using Fasterflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Lookup
{
    [TestClass]
    public class FieldTest
	{
		#region Test Data
		private enum Color
        {
            Red, Green, Blue
        }

        private class PersonClass
        {
			#pragma warning disable 0169, 0649
            private static int counter;
            private int age;
            public PersonClass peer;
            internal Color[] favoriteColors;
			#pragma warning restore 0169, 0649
        }

        private struct PersonStruct
        {
			#pragma warning disable 0169, 0649
            private static int counter;
            private int age;
            public PersonClass peer; // can't use PersonStruct here (infinite initialization)
            internal Color[] favoriteColors;
			#pragma warning restore 0169, 0649
        }

        private static readonly List<Type> TypeList = new List<Type>
                {
                    typeof(PersonClass), 
                    typeof(PersonStruct)
                };
		#endregion

        #region Field Lookup
        #region Single Field
        //public static FieldInfo Field<T>( this Type type, string name )
		//public static FieldInfo Field<T>( this Type type, string name, BindingFlags flags )
		//public static FieldInfo FieldDeclared<T>( this Type type, string name )
		//public static FieldInfo FieldDeclared<T>( this Type type, string name, BindingFlags flags )
		#endregion

		#region Multiple Fields
		//public static IList<FieldInfo> Fields( this Type type )
		//public static IList<FieldInfo> Fields( this Type type, BindingFlags flags )
		//public static IList<FieldInfo> FieldsDeclared( this Type type )
		//public static IList<FieldInfo> FieldsDeclared( this Type type, BindingFlags flags )
		#endregion
		#endregion
	}
}
