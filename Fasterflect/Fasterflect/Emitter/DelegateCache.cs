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
using System.Threading;

namespace Fasterflect.Emitter
{
    internal class DelegateCache : IDisposable
    {
        private readonly Dictionary<CallInfo, Delegate> map = new Dictionary<CallInfo, Delegate>();
        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Get the corresponding delegate for the specified <param name="callInfo"/>.
        /// If there's no existing delegate, a new one is created by invoking the 
        /// construction method <param name="func" /> and the result is added to the map
        /// for subsequent accesses.
        /// 
        /// This method assures that one and only one delegate can be created for 
        /// any particular instance of <param name="callInfo"/>.
        /// </summary>
        public Delegate GetDelegate(CallInfo callInfo, Func<Delegate> func)
        {
            Delegate action;
            bool exist;
            rwLock.EnterReadLock();
            try 
            {
                exist = map.TryGetValue(callInfo, out action);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
            
            if (exist) return action;

            rwLock.EnterWriteLock();
            try
            {
                if (map.ContainsKey(callInfo))
                {
                    action = map[callInfo];
                }
                else
                {
                    action = func();
                    map.Add(callInfo, action);
                }
            } 
            finally
            {
                rwLock.ExitWriteLock();
            }
            return action;
        }

        public void Dispose()
        {
            rwLock.Dispose();
        }
    }
}
