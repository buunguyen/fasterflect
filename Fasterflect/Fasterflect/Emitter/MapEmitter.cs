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

namespace Fasterflect.Emitter
{
    internal class MapEmitter : BaseEmitter
    {
        private readonly Type sourceType;
        private readonly MemberTypes sourceMemberTypes;
        private readonly MemberTypes targetMemberTypes;
        private readonly string[] names;

        public MapEmitter(Type sourceType, Type targetType, MemberTypes sourceMemberTypes, MemberTypes targetMemberTypes,
                           Flags bindingFlags, params string[] names)
        {
            this.sourceType = sourceType;
            this.sourceMemberTypes = sourceMemberTypes;
            this.targetMemberTypes = targetMemberTypes;
            this.names = names;

            // auto-apply IgnoreCase if we're mapping from one membertype to another
            bool different = (sourceMemberTypes & targetMemberTypes) != sourceMemberTypes;
            bindingFlags = Flags.SetIf(bindingFlags, Flags.IgnoreCase, different);

            var parameterTypes = new Type[1 + (names == null ? 0 : names.Length)];
            parameterTypes[0] = sourceType;
            for (int i = 1; i < parameterTypes.Length; i++)
            {
                parameterTypes[i] = typeof(string);
            }
            callInfo = new CallInfo(targetType, bindingFlags, MemberTypes.Custom, "Copier", parameterTypes, null);
        }

        protected internal override int GetCacheKey()
        {
            int key = ((sourceType.GetHashCode() << 32) + callInfo.TargetType.GetHashCode()) ^
                           (callInfo.BindingFlags.GetHashCode() ^ sourceMemberTypes.GetHashCode() ^ targetMemberTypes.GetHashCode());
            if (names != null && names.Length > 0)
            {
                for (int index = 0; index < names.Length; index++)
                {
                    var name = names[index];
                    key += name.GetHashCode();
                }
            }
            return key;
        }

        protected internal override DynamicMethod CreateDynamicMethod()
        {
            return CreateDynamicMethod(sourceType.Name, sourceType, null, new[] { Constants.ObjectType, Constants.ObjectType });
        }

        protected internal override Delegate CreateDelegate()
        {
            bool handleInnerStruct = callInfo.ShouldHandleInnerStruct;
            if (handleInnerStruct)
            {
                generator.ldarg_1.end();                     // load arg-1 (target)
                generator.DeclareLocal(callInfo.TargetType); // TargetType localStr;
                generator
                    .castclass(Constants.StructType) // (ValueTypeHolder)wrappedStruct
                    .callvirt(StructGetMethod) // <stack>.get_Value()
                    .unbox_any(callInfo.TargetType) // unbox <stack>
                    .stloc(0); // localStr = <stack>
            }

            foreach (var pair in GetMatchingMembers())
            {
                if (handleInnerStruct)
                    generator.ldloca_s(0).end(); // load &localStr
                else
                    generator.ldarg_1.castclass(callInfo.TargetType).end(); // ((TargetType)target)
                generator.ldarg_0.castclass(sourceType);
                GenerateGetMemberValue(pair.Key);
                GenerateSetMemberValue(pair.Value);
            }

            if (handleInnerStruct)
            {
                StoreLocalToInnerStruct(1, 0);     // ((ValueTypeHolder)this)).Value = tmpStr
            }

            generator.ret();
            return method.CreateDelegate(typeof(MemberCopier));
        }

        private void GenerateGetMemberValue(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                generator.ldfld((FieldInfo)member);
            }
            else
            {
                var method = ((PropertyInfo)member).GetGetMethod(true);
                generator.callvirt(method, null);
            }
        }
        private void GenerateSetMemberValue(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                generator.stfld((FieldInfo)member);
            }
            else
            {
                var method = ((PropertyInfo)member).GetSetMethod(true);
                generator.callvirt(method, null);
            }
        }

        internal IDictionary<MemberInfo, MemberInfo> GetMatchingMembers()
        {
            StringComparison comparison = callInfo.BindingFlags.IsSet(Flags.IgnoreCase)
                                            ? StringComparison.OrdinalIgnoreCase
                                            : StringComparison.Ordinal;
            var query = from s in sourceType.Members(sourceMemberTypes, callInfo.BindingFlags, names)
                        from t in callInfo.TargetType.Members(targetMemberTypes, callInfo.BindingFlags, names)
                        where s.Name.Equals(t.Name, comparison) &&
                              t.Type().IsAssignableFrom(s.Type()) &&
                              s.IsReadable() && t.IsWritable()
                        select new { Source = s, Target = t };
            return query.ToDictionary(k => k.Source, v => v.Target);
        }
    }
}