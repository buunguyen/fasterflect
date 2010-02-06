#region License

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
		private static CacheStore<CallInfo, Delegate> cache = new CacheStore<CallInfo, Delegate>( LockStrategy.Monitor );
		
		#region Delegate Cache Methods
		/// <summary>
		/// Get the corresponding delegate for the specified <param name="callInfo"/>.
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
		/// Get the corresponding delegate for the specified <param name="callInfo"/>.
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
			Delegate action = cache.Get(callInfo);
			if( action == null )
			{
				action = createDelegateAction();
				cache.Insert(callInfo, action, strategy);
			}
			return action;
		}
		#endregion
		
		#region Properties
		public static LockStrategy LockStrategy
		{
			get { return cache.LockStrategy; }
			set
			{
				// presumably this isn't thread safe so should not really be set once cache is in use
				var oldCache = cache;
				cache = new CacheStore<CallInfo, Delegate>( value );
				oldCache.Dispose();
			}
		}
		#endregion
	}
}