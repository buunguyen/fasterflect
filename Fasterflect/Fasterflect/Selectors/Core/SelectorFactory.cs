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

using System.Collections.Generic;
using System.Linq;
using Fasterflect.Selectors.Interfaces;

namespace Fasterflect.Selectors.Core
{
	internal static class SelectorFactory
	{
        private static readonly IDictionary<Flags, ISelector> selectorMap = new
            Dictionary<Flags, ISelector>
               {
                   { Flags.NameMatch, new NameMatch() },
                   { Flags.PartialNameMatch, new PartialNameMatch() },
                   { Flags.ExcludeExplicitlyImplemented, new ExcludeExplicitlyImplemented() },
                   { Flags.ParameterMatch, new ParameterMatch() },
                   // { Flags.ExactParameterMatch, new ExactParameterMatch() },
				   { Flags.ExcludeBackingMembers, new ExcludeBackingMembers() }
               };

        public static IEnumerable<ISelector> GetSelectors(Flags flags)
        {
            return GetSelectorsOfType<ISelector>(flags);
		}

        public static IEnumerable<IMemberSelector> GetMemberSelectors(Flags flags)
        {
            return GetSelectorsOfType<IMemberSelector>( flags );
        }

        public static IEnumerable<IMethodSelector> GetMethodSelectors(Flags flags)
        {
		    return GetSelectorsOfType<IMethodSelector>( flags );
		}

        private static IEnumerable<T> GetSelectorsOfType<T>(Flags flags)
        {
            return from key in selectorMap.Keys
                   where flags.IsSet( key ) && typeof(T).IsAssignableFrom( selectorMap[ key ].GetType() )
                   select (T)selectorMap[ key ];
        }
	}
}
