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
using Fasterflect.Common;

namespace Fasterflect.ObjectConstruction
{
	internal class SourceInfo
	{
		#region EqualityComparer
		internal class SourceInfoComparer : IEqualityComparer<SourceInfo>
		{
			#region Implementation of IEqualityComparer<SourceInfo>
			public bool Equals( SourceInfo x, SourceInfo y )
			{
				return x.Equals( y );
			}

			public int GetHashCode( SourceInfo obj )
			{
				return obj.hashCode;
			}
			#endregion
		}
		#endregion

		#region Fields
		private static readonly SourceInfoComparer comparer = new SourceInfoComparer();
		private bool[] paramKinds;
		private string[] paramNames;
		private Type[] paramTypes;
		private MemberGetter[] paramValueReaders;
		private Type type;
		private int hashCode;
		#endregion

		#region Constructors
		public SourceInfo( Type type )
		{
			this.type = type;
			ExtractParameterInfo( type );
			hashCode = CalculateHashCode();
		}

		public SourceInfo( Type type, string[] names, Type[] types )
		{
			this.type = type;
			paramNames = names;
			paramTypes = types;
			paramKinds = new bool[names.Length];
			for (int i = 0; i < paramKinds.Length; i++)
			{
				paramKinds[i] = true;
			}
			hashCode = CalculateHashCode();
		}
		#endregion

		#region Properties
		public static SourceInfoComparer Comparer
		{
			get { return comparer; }
		}

		public Type Type
		{
			get { return type; }
		}

		public int HashCode
		{
			get { return hashCode; }
		}

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
		#endregion

		#region Parameter Value Access
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

		internal MemberGetter GetReader(string memberName)
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
		#endregion

		#region Equals + GetHashCode
		public override bool Equals( object obj )
		{
			var other = obj as SourceInfo;
			if (other == null) return false;
			if (other == this) return true;
			if( hashCode != other.GetHashCode() ) return false;

			if( type != other.Type || paramNames.Length != other.ParamNames.Length )
				return false;
			for( int i = 0; i < paramNames.Length; i++ )
			{
				if( paramNames[ i ] != other.ParamNames[ i ] || paramTypes[ i ] != other.ParamTypes[ i ] )
					return false;
			}
			return true;
		}
		public override int GetHashCode()
		{
			return hashCode;
		}
		internal int CalculateHashCode()
		{
			int hash = type.GetHashCode();
			for( int i = 0; i < paramNames.Length; i++ )
			    hash += (i + 31) * paramNames[ i ].GetHashCode() ^ paramTypes[ i ].GetHashCode();
			return hash;
		}
		#endregion

		#region Anonymous Type Helper (ExtractParameterInfo)
		internal void ExtractParameterInfo( Type type )
		{
            IList<MemberInfo> members = type.Members(MemberTypes.Field | MemberTypes.Property, Flags.InstanceCriteria);
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