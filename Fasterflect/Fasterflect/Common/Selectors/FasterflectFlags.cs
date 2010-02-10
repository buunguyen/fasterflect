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

namespace Fasterflect.Selectors
{
	[Flags]
	public enum FasterflectFlags : long
	{
		// BindingFlags
	    None                 = 0, // BindingFlags.Default
	    IgnoreCase           = 0x1,
	    DeclaredOnly         = 0x2,
	    Instance             = 0x4,
	    Static               = 0x8,
	    Public               = 0x10,
	    NonPublic            = 0x20,
	    //FlattenHierarchy     = 0x40,
		//InvokeMethod         = 0x100,
		//CreateInstance       = 0x200,
		//GetField             = 0x400,
		//SetField             = 0x800,
		//GetProperty          = 0x1000,
		//SetProperty          = 0x2000,
		//PutDispProperty      = 0x4000,
		//PutRefDispProperty   = 0x8000,
	    ExactBinding         = 0x10000,
	    SuppressChangeType   = 0x20000,
	    OptionalParamBinding = 0x40000,
	    IgnoreReturn         = 0x1000000,

		// Fasterflect
		PartialNameMatch     = 0x100000000,
	    ExplicitNameMatch    = 0x200000000,
	    ParameterMatch       = 0x400000000,
	    ExactParameterMatch  = 0x800000000,
	}
}
