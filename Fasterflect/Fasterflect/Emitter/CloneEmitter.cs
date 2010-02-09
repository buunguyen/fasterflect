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
using System.Reflection.Emit;
using Fasterflect.Common;

namespace Fasterflect.Emitter
{
	// TODO cache created delegates
	internal class CloneEmitter
	{
		private readonly Type sourceType;
		private readonly Type targetType;
		private readonly BindingFlags flags;

		public CloneEmitter( Type sourceType, Type targetType, BindingFlags flags )
		{
			this.sourceType = sourceType;
			this.targetType = targetType;
			this.flags = flags;
		}

		protected internal Delegate CreateDelegate()
		{
			var name = string.Format( "Copy_{0}_To_{1}", sourceType.Name, targetType.Name );
    		var args = new[] { Constants.ObjectType, Constants.ObjectType };
			DynamicMethod method = BaseEmitter.CreateDynamicMethod( name, sourceType, null, args );
			ILGenerator generator = method.GetILGenerator();
 
		    foreach( var pair in GetMatchingProperties( sourceType, targetType, flags ) )
		    {
		        generator.Emit( OpCodes.Ldarg_1 );
		        generator.Emit( OpCodes.Ldarg_0 );
		        generator.EmitCall( OpCodes.Callvirt, pair.Key.GetGetMethod(), null );
		        generator.EmitCall( OpCodes.Callvirt, pair.Value.GetSetMethod(), null );
		    }
		    generator.Emit(OpCodes.Ret);
			// TODO introduce own delegate even if signature matches
			return method.CreateDelegate( typeof(MemberSetter) );
		}

		internal static IDictionary<PropertyInfo,PropertyInfo> GetMatchingProperties( Type sourceType, Type targetType, BindingFlags flags )
		{
		    var query = from s in sourceType.Properties( flags )
		                from t in targetType.Properties( flags )
		                where s.Name == t.Name && t.PropertyType.IsAssignableFrom( t.PropertyType ) && s.CanRead() && t.CanWrite()
		                select new { SourceProperty=s, TargetProperty=t };
		    return query.ToDictionary( k => k.SourceProperty, v => v.TargetProperty );
		}
	}
}