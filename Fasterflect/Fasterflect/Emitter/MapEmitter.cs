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
using System.Reflection.Emit;
using Fasterflect.Caching;

namespace Fasterflect.Emitter
{
    internal class MapEmitter
    {
        private static volatile Cache<long, Delegate> cache = new Cache<long, Delegate>();
        private readonly long cacheKey;
        private readonly Type sourceType;
        private readonly Type targetType;
		private readonly MemberTypes sourceMemberTypes;
		private readonly MemberTypes targetMemberTypes;
        private readonly Flags bindingFlags;
		private readonly string[] names;

		public MapEmitter( Type sourceType, Type targetType, MemberTypes sourceMemberTypes, MemberTypes targetMemberTypes, 
						   Flags bindingFlags, params string[] names )
		{
            this.sourceType = sourceType;
            this.targetType = targetType;
			this.sourceMemberTypes = sourceMemberTypes;
			this.targetMemberTypes = targetMemberTypes;
			this.names = names;
			// auto-apply IgnoreCase if we're mapping from one membertype to another
			bool different = (sourceMemberTypes & targetMemberTypes) != sourceMemberTypes;
			this.bindingFlags = Flags.SetIf( bindingFlags, Flags.IgnoreCase, different );
			// calculate the key for caching of the delegate
            cacheKey = (((long) sourceType.GetHashCode() << 32) + targetType.GetHashCode()) ^
                       (bindingFlags.GetHashCode() ^ sourceMemberTypes.GetHashCode() ^ targetMemberTypes.GetHashCode());
			if( names != null && names.Length > 0 )
			{
				foreach( var name in names )
				{
					cacheKey += name.GetHashCode();
				}
			}
		}

        public MemberCopier GetDelegate()
        {
            MemberCopier action = cache.Get( cacheKey ) as MemberCopier;
            if( action == null )
            {
                action = CreateDelegate();
                cache.Insert( cacheKey, action, CacheStrategy.Temporary );
            }
            return action;
        }

        protected internal MemberCopier CreateDelegate()
        {
            var name = string.Format( "Map_{0}_to_{1}", sourceType.Name, targetType.Name );
            var args = new[] { Constants.ObjectType, Constants.ObjectType };
            DynamicMethod method = BaseEmitter.CreateDynamicMethod( name, sourceType, null, args );
            var generator = new EmitHelper( method.GetILGenerator() );

            foreach( var pair in GetMatchingMembers() )
            {
				generator
				    .ldarg_1
				    .castclass( targetType )
				    .ldarg_0
				    .castclass( sourceType );
				GenerateGetMemberValue( generator, pair.Key );
				GenerateSetMemberValue( generator, pair.Value );
			}
            generator.ret();
            return (MemberCopier) method.CreateDelegate( typeof(MemberCopier) );
        }
		private void GenerateGetMemberValue( EmitHelper generator, MemberInfo member )
		{
			if( member is FieldInfo )
			{
				generator.ldfld( (FieldInfo) member );
			}
			else
			{
				var method = ((PropertyInfo) member).GetGetMethod( true );
				generator.callvirt( method, null );
			}
		}
		private void GenerateSetMemberValue( EmitHelper generator, MemberInfo member )
		{
			if( member is FieldInfo )
			{
				generator.stfld( (FieldInfo) member );
			}
			else
			{
				var method = ((PropertyInfo) member).GetSetMethod( true );
				generator.callvirt( method, null );
			}
		}

        internal IDictionary<MemberInfo, MemberInfo> GetMatchingMembers()
        {
        	StringComparison comparison = bindingFlags.IsSet( Flags.IgnoreCase )
        	                              	? StringComparison.OrdinalIgnoreCase
        	                              	: StringComparison.Ordinal;
            var query = from s in sourceType.Members( sourceMemberTypes, bindingFlags, names )
                        from t in targetType.Members( targetMemberTypes, bindingFlags, names )
                        where s.Name.Equals( t.Name, comparison ) && 
							  t.Type().IsAssignableFrom( s.Type() ) && 
							  s.IsReadable() && t.IsWritable()
                        select new { Source = s, Target = t };
            return query.ToDictionary( k => k.Source, v => v.Target );
        }
    }
}