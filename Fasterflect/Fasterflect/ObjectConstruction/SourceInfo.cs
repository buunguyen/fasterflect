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

namespace Fasterflect.ObjectConstruction
{
	internal class SourceInfo
	{
		private bool[] paramKinds;
		private string[] paramNames;
		private Type[] paramTypes;
		private MemberGetter[] paramValueReaders;
		private Type type;

		public SourceInfo(Type type)
		{
			this.type = type;
			ExtractParameterInfo(type);
		}

		public SourceInfo(Type type, string[] names, Type[] types)
		{
			this.type = type;
			paramNames = names;
			paramTypes = types;
			paramKinds = new bool[names.Length];
			for (int i = 0; i < paramKinds.Length; i++)
			{
				paramKinds[i] = true;
			}
		}

		#region Properties

		public string[] ParamNames
		{
			get { return paramNames; }
		}

		public Type[] ParamTypes
		{
			get { return paramTypes; }
		}

		public bool[] ParamKinds
		{
			get { return paramKinds; }
		}

		public MemberGetter[] ParamValueReaders
		{
			get
			{
				InitializeParameterValueReaders();
				return paramValueReaders;
			}
		}

		public MemberGetter GetReader(string memberName)
		{
			int index = Array.IndexOf(paramNames, memberName);
			MemberGetter reader = paramValueReaders[index];
			if (reader == null)
			{
				reader = paramKinds[index] ? type.DelegateForGetField(memberName) : type.DelegateForGetProperty(memberName);
				paramValueReaders[index] = reader;
			}
			return reader;
		}

		#endregion

		public object[] GetParameterValues(object source)
		{
			InitializeParameterValueReaders();
			var paramValues = new object[paramNames.Length];
			for (int i = 0; i < paramNames.Length; i++)
			{
				paramValues[i] = paramValueReaders[i](source);
			}
			return paramValues;
		}

		private void InitializeParameterValueReaders()
		{
			if (paramValueReaders == null)
			{
				paramValueReaders = new MemberGetter[paramNames.Length];
				for (int i = 0; i < paramNames.Length; i++)
				{
					string name = paramNames[i];
					paramValueReaders[i] = paramKinds[i] ? type.DelegateForGetField(name) : type.DelegateForGetProperty(name);
				}
			}
		}

		#region Anonymous Type Helper

		internal void ExtractParameterInfo(Type type)
		{
			IList<MemberInfo> members = type.Members(MemberTypes.Field | MemberTypes.Property, Reflector.InstanceCriteria);
			var names = new List<string>(members.Count);
			var types = new List<Type>(members.Count);
			var kinds = new List<bool>(members.Count);
			for (int i = 0; i < members.Count; i++)
			{
				MemberInfo mi = members[i];
				bool include = mi is FieldInfo && mi.Name[0] != '<'; // exclude auto-generated backing fields
				include |= mi is PropertyInfo && (mi as PropertyInfo).CanRead; // exclude write-only properties
				if (include)
				{
					names.Add(mi.Name);
					bool isField = mi is FieldInfo;
					kinds.Add(isField);
					types.Add(isField ? (mi as FieldInfo).FieldType : (mi as PropertyInfo).PropertyType);
				}
			}
			paramNames = names.ToArray();
			paramTypes = types.ToArray();
			paramKinds = kinds.ToArray();
		}

		#endregion
	}
}