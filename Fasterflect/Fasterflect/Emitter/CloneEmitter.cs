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
    internal class CloneEmitter
    {
        private static volatile Cache<long, Delegate> cache = new Cache<long, Delegate>();
        private readonly Type sourceType;
        private readonly Type targetType;
        private readonly Flags bindingFlags;
        private readonly MemberTypes memberTypeToCopy;
        private readonly long cacheKey;

        public CloneEmitter( Type sourceType, Type targetType, Flags bindingFlags, MemberTypes memberTypeToCopy )
        {
            this.sourceType = sourceType;
            this.targetType = targetType;
            this.bindingFlags = bindingFlags;
            this.memberTypeToCopy = memberTypeToCopy;
            cacheKey = (((long) sourceType.GetHashCode() << 32) + targetType.GetHashCode()) ^
                       (bindingFlags.GetHashCode() ^ memberTypeToCopy.GetHashCode());
        }

        public Delegate GetDelegate()
        {
            Delegate action = cache.Get( cacheKey );
            if( action == null )
            {
                action = CreateDelegate();
                cache.Insert( cacheKey, action, CacheStrategy.Temporary );
            }
            return action;
        }

        protected internal Delegate CreateDelegate()
        {
            var name = string.Format( "Copy_{0}_to_{1}", sourceType.Name, targetType.Name );
            var args = new[] { Constants.ObjectType, Constants.ObjectType };
            DynamicMethod method = BaseEmitter.CreateDynamicMethod( name, sourceType, null, args );
            var generator = new EmitHelper( method.GetILGenerator() );

            if( memberTypeToCopy == MemberTypes.Field )
            {
                foreach( var pair in GetMatchingFields( sourceType, targetType, bindingFlags ) )
                {
                    generator
                        .ldarg_1
                        .castclass( targetType )
                        .ldarg_0
                        .castclass( sourceType )
                        .ldfld( pair.Key )
                        .stfld( pair.Value );
                }
            }
            else
            {
                foreach( var pair in GetMatchingProperties( sourceType, targetType, bindingFlags ) )
                {
                    generator
                        .ldarg_1
                        .ldarg_0
                        .callvirt( pair.Key.GetGetMethod(), null )
                        .callvirt( pair.Value.GetSetMethod(), null );
                }
            }
            generator.ret();
            return method.CreateDelegate( typeof(MemberCopier) );
        }

        internal static IDictionary<FieldInfo, FieldInfo> GetMatchingFields( Type sourceType, Type targetType,
                                                                             Flags bindingFlags )
        {
            var query = from s in sourceType.Fields( bindingFlags )
                        from t in targetType.Fields( bindingFlags )
                        where
                            s.Name == t.Name && t.FieldType.IsAssignableFrom( s.FieldType ) && s.IsReadable() &&
                            t.IsWritable()
                        select new { Source = s, Target = t };
            return query.ToDictionary( k => k.Source, v => v.Target );
        }

        internal static IDictionary<PropertyInfo, PropertyInfo> GetMatchingProperties( Type sourceType, Type targetType,
                                                                                       Flags bindingFlags )
        {
            var query = from s in sourceType.Properties( bindingFlags )
                        from t in targetType.Properties( bindingFlags )
                        where
                            s.Name == t.Name && t.PropertyType.IsAssignableFrom( s.PropertyType ) && s.IsReadable() &&
                            t.IsWritable()
                        select new { Source = s, Target = t };
            return query.ToDictionary( k => k.Source, v => v.Target );
        }
    }
}