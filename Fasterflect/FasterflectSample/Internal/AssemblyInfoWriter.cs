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

namespace FasterflectSample.Internal
{
	/// <summary>
	/// Internally use by Fasterflect team
	/// </summary>
	internal class AssemblyInfoWriter
	{
		//static void Main()
		//{
		//    Console.WriteLine( AssemblyInfoWriter.ListExtensions( Assembly.GetAssembly( typeof(Flags) ) ) );
		//}

		public static string ListExtensions( Assembly assembly )
		{
			var sb = new StringBuilder();
			sb.AppendFormat( "=== Extension methods (all):{0}", Environment.NewLine );
			IList<Type> types = assembly.Types( Flags.PartialNameMatch, "Extensions" );
			types.ForEach( t => Write( sb, t, t.Methods( Flags.Static | Flags.Public ).OrderBy( m => m.Name ).ToList() ) );
			return sb.ToString();
		}

		public static string ListExtensionMethodsWithSuperfluousOverloads( Assembly assembly )
		{
			var sb = new StringBuilder();
			sb.AppendFormat( "=== Extension methods that are superfluous:{0}", Environment.NewLine );
			IList<Type> types = assembly.Types( Flags.PartialNameMatch, "Extensions" );
			types.ForEach( t => Write( sb, t, t.Methods( Flags.Static | Flags.Public )
			                                  	.OrderBy( m => m.Name )
			                                  	.Where( m => IsSuperfluous( m, t.Methods( Flags.Static | Flags.Public, m.Name ) ) )
			                                  	.ToList() ) );
			return sb.ToString();
		}

		private static bool IsSuperfluous( MethodInfo method, IEnumerable<MethodInfo> methods )
		{
			IList<ParameterInfo> parameters = method.Parameters();
			bool lastIsParams = HasParamsParameter( parameters );
			if( lastIsParams )
			{
				return false;
			}
			return methods.Where( m => method.IsGenericMethod
			                           	? m.IsGenericMethod &&
			                           	  method.GetGenericArguments().SequenceEqual( m.GetGenericArguments() )
			                           	: !m.IsGenericMethod )
				.Select( m => m.Parameters() )
				.Where( ps => parameters.Count == ps.Count - 1 && HasParamsParameter( ps ) )
				.Any(
					ps =>
					parameters.Select( p => p.ParameterType ).SequenceEqual( ps.Take( parameters.Count ).Select( p => p.ParameterType ) ) );
		}

		public static string ListExtensionMethodsWhereParametersViolateConventions( Assembly assembly )
		{
			var sb = new StringBuilder();
			sb.AppendFormat( "=== Extension methods with naming violations:{0}", Environment.NewLine );
			IList<Type> types = assembly.Types( Flags.PartialNameMatch, "Extensions" );
			types.ForEach( t => Write( sb, t, t.Methods( Flags.Static | Flags.Public )
			                                  	.OrderBy( m => m.Name )
			                                  	.Where(
			                                  		m => IsParameterViolation( typeof(Flags), "bindingFlags", true, m.Parameters() ) )
			                                  	.ToList() ) );
			return sb.ToString();
		}

		private static bool IsParameterViolation( Type parameterType, string expectedName, bool shouldBeLast,
		                                          IList<ParameterInfo> parameters )
		{
			int lastIndex = parameters.Count - 1;
			bool lastIsParams = HasParamsParameter( parameters );
			for( int i = 0; i < parameters.Count; i++ )
			{
				ParameterInfo parameter = parameters[ i ];
				if( parameter.ParameterType == parameterType )
				{
					bool valid = parameter.Name == expectedName;
					valid &= !shouldBeLast || (i == lastIndex || (lastIsParams && i == lastIndex - 1));
					if( !valid )
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool HasParamsParameter( IList<ParameterInfo> parameters )
		{
			int lastIndex = parameters.Count - 1;
			bool lastIsParams = lastIndex >= 0 &&
			                    parameters[ lastIndex ].ParameterType.IsArray &&
			                    parameters[ lastIndex ].HasAttribute<ParamArrayAttribute>();
			return lastIsParams;
		}

		private static void Write( StringBuilder sb, Type type, IList<MethodInfo> methods )
		{
			if( methods.Count > 0 )
			{
				sb.AppendFormat( "{1}--- {0}{1}", type.Name, Environment.NewLine );
				methods.ForEach( m => Write( sb, m, m.Parameters() ) );
			}
		}

		private static void Write( StringBuilder sb, MethodInfo method, IList<ParameterInfo> parameters )
		{
			sb.AppendFormat( "{0} ", method.ReturnType.Name() );
			sb.AppendFormat( "{0}{1}( ", method.Name, GetGenericParameterText( method ) );
			ParameterInfo first = parameters.FirstOrDefault();
			ParameterInfo last = parameters.LastOrDefault();
			bool addPrefix = method.IsStatic && method.DeclaringType.Name.EndsWith( "Extensions" );
			parameters.ForEach( p => sb.AppendFormat( "{0}{1} {2}{3}",
			                                          p == first && addPrefix ? "this " : "",
			                                          GetType( p ), p.Name, p == last ? "" : ", " ) );
			sb.AppendFormat( " );{0}", Environment.NewLine );
		}

		private static string GetGenericParameterText( MethodInfo method )
		{
			if( ! method.ContainsGenericParameters )
			{
				return string.Empty;
			}
			Type[] genericParameters = method.GetGenericArguments();
			string args = string.Join( ",", genericParameters.Select( t => t.Name() ) );
			return "<" + args + ">";
		}

		private static string GetType( ParameterInfo parameter )
		{
			bool isParams = parameter.ParameterType.IsArray && parameter.HasAttribute<ParamArrayAttribute>();
			string prefix = isParams ? "params " : string.Empty;
			return prefix + parameter.ParameterType.Name();
		}
	}
}
