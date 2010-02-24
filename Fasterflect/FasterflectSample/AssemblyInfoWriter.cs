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
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;

namespace FasterflectSample
{
    internal class AssemblyInfoWriter
    {
        public static string ListExtensions( Assembly assembly )
        {
        	StringBuilder sb = new StringBuilder();
        	var types = assembly.Types( Flags.PartialNameMatch, "Extensions" );
			types.ForEach( t => Write( sb, t, t.Methods( Flags.Static | Flags.Public ).OrderBy( m => m.Name ).ToList() ) );
        	return sb.ToString();
        }

		private static void Write( StringBuilder sb, Type type, IList<MethodInfo> methods )
		{
			sb.AppendFormat( "{1}--- {0}{1}", type.Name, Environment.NewLine );
			methods.ForEach( m => Write( sb, m, m.Parameters() ) );	
		}

		private static void Write( StringBuilder sb, MethodInfo method, IList<ParameterInfo> parameters )
		{
			sb.AppendFormat( "{0}( ", method.Name );
			var last = parameters.LastOrDefault();
			parameters.ForEach( p => sb.AppendFormat( "{0} {1}{2}",
			    p.ParameterType.Name, p.Name, p == last ? "" : ", " ) );
			sb.AppendFormat( " );{0}", Environment.NewLine );
		}
    }
}
