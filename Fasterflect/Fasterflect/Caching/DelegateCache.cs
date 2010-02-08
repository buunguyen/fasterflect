﻿#region License

// Copyright 2010 Morten Mertner, Buu Nguyen (http://www.buunguyen.net/blog)
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
using Fasterflect.Emitter;

namespace Fasterflect.Caching
{
	internal static class DelegateCache
	{
		private static volatile Cache<CallInfo, Delegate> cache = new Cache<CallInfo, Delegate>();
		
		/// <summary>
		/// GetValue the corresponding delegate for the specified <param name="callInfo"/>.
		/// </summary>
		/// <returns>A delegate if one was found and null otherwise.</returns>
		public static Delegate Get( CallInfo callInfo )
		{
			return cache.Get( callInfo );
		}
		/// <summary>
		/// Inserts the supplied <paramref name="action"/> delegate into the cache using <paramref name="callInfo"/>
		/// as the key. The <paramref name="strategy"/> parameter can be used to have the action stored using a weak
		/// reference.
		/// </summary>
		public static void Insert( CallInfo callInfo, Delegate action, CacheStrategy strategy )
        {
			cache.Insert( callInfo, action, strategy );
		}
	}

	#region DelegateCache using CacheStore
	internal static class DelegateCacheCacheStore
	{
	    private static readonly ILock synchronizer = new ReaderWriterLock();
		private static volatile CacheStore<CallInfo, Delegate> cache = new CacheStore<CallInfo, Delegate>( LockStrategy.Monitor );
		
		#region Delegate Cache Methods
		/// <summary>
		/// GetValue the corresponding delegate for the specified <param name="callInfo"/>.
		/// If there's no existing delegate, a new one is created by invoking the 
		/// construction method <param name="createDelegateAction" /> and the result is added 
		/// to the map for subsequent accesses.
		/// 
		/// This method assures that one and only one delegate can be created for 
		/// any particular instance of <paramref name="callInfo"/>.
		/// 
		/// Created delegates are cached temporarily using a <see href="WeakReference"/>.
		/// </summary>
		public static Delegate GetDelegate(CallInfo callInfo, Func<Delegate> createDelegateAction)
		{
			return GetDelegate(callInfo, createDelegateAction, CacheStrategy.Temporary);
		}

		/// <summary>
		/// GetValue the corresponding delegate for the specified <param name="callInfo"/>.
		/// If there's no existing delegate, a new one is created by invoking the 
		/// construction method <param name="createDelegateAction" /> and the result is added 
		/// to the map for subsequent accesses.
		/// 
		/// This method assures that one and only one delegate can be created for 
		/// any particular instance of <paramref name="callInfo"/>.
		/// 
		/// Created delegates are cached temporarily using a <see href="WeakReference"/> by 
		/// default. This can be overridden by specifying an alternative option for
		/// <param name="strategy"/>.
		/// </summary>
		public static Delegate GetDelegate(CallInfo callInfo, Func<Delegate> createDelegateAction, CacheStrategy strategy)
        {
            using (synchronizer.ReaderLock)
            {
                Delegate action = cache.Get( callInfo );
                if( action == null )
                {
                    action = createDelegateAction();
                    cache.Insert(callInfo, action, strategy);
                }
                return action;
            }
        }
		#endregion
		
		#region Properties
		public static LockStrategy LockStrategy
		{
            get
            {
                using (synchronizer.ReaderLock)
                {
                    return cache.LockStrategy;
                }
            }
			set
			{
                using (synchronizer.WriterLock)
                {
                    cache.Dispose();
                    cache = new CacheStore<CallInfo, Delegate>( value );
                }
			}
		}
		#endregion
	}
	#endregion
}