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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasterflect.Caching
{
	internal sealed class CacheStore<TKey,TValue> : IEnumerable<CacheEntry<TKey,TValue>> where TValue : class
	{
		private readonly Dictionary<TKey,CacheEntry<TKey,TValue>> entries = new Dictionary<TKey,CacheEntry<TKey,TValue>>();
		private LockStrategy currentLockStrategy;
		private ILock synchronizer;

		#region Constructors
		public CacheStore( LockStrategy lockStrategy )
		{
			InitializeLock( lockStrategy );
		}

		/// <summary>
		/// This method will create or replace the underlying locking mechanism used. Calling this method is not
		/// thread safe. Accessing the cache from other threads while this method executes will almost certainly
		/// cause an exception. 
		/// </summary>
		/// <param name="strategy">The locking mechanism to switch to if it is not currently being used.</param>
		private void InitializeLock( LockStrategy strategy )
		{
			bool createLock = synchronizer == null || currentLockStrategy != strategy;
			if( createLock )
			{
				if( synchronizer != null )
				{
					synchronizer.Dispose();
				}
				currentLockStrategy = strategy;
				synchronizer = strategy == LockStrategy.Monitor ? new MonitorLock() : new ReaderWriterLock() as ILock;
			}
		}
		#endregion

		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		public IEnumerator<CacheEntry<TKey,TValue>> GetEnumerator()
		{
			var list = synchronizer.Read<IList<CacheEntry<TKey,TValue>>>( () => entries.Values.Where( e => ! e.IsCollected ).ToList() );
			return list.GetEnumerator();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the number of entries currently stored in the cache. Accessing this property
		/// causes a check of all entries in the cache to ensure collected entries are not counted.
		/// </summary>
		public int Count
		{
			get { return ClearCollected(); }
		}

		/// <summary>
		/// This property returns the underlying locking mechanism used. It is possible to assign a
		/// new value, which will replace the internal locking mechanism with the specified value. 
		/// Replacing the lock is not thread safe. Accessing the cache from other threads while this
		/// property is being set will almost certainly cause an exception.
		/// </summary>
		public LockStrategy LockStrategy
		{
			get { return currentLockStrategy; }
			set { InitializeLock( value ); }
		}
		#endregion

		#region Indexers
		/// <summary>
		/// Indexer for accessing or adding cache entries.
		/// </summary>
		public TValue this[ TKey key ]
		{
			get { return Get( key ); }
			set { Insert( key, value, CacheStrategy.Temporary ); }
		}

		/// <summary>
		/// Indexer for adding a cache item using the specified strategy.
		/// </summary>
		public TValue this[ TKey key, CacheStrategy strategy ]
		{
			set { Insert( key, value, strategy ); }
		}
		#endregion

		#region Insert Methods
		/// <summary>
		/// Insert a collectible object into the cache.
		/// </summary>
		/// <param name="key">The cache key used to reference the item.</param>
		/// <param name="value">The object to be inserted into the cache.</param>
		public void Insert( TKey key, TValue value )
		{
			Insert( key, value, CacheStrategy.Temporary );
		}

		/// <summary>
		/// Insert an object into the cache using the specified cache strategy (lifetime management).
		/// </summary>
		/// <param name="key">The cache key used to reference the item.</param>
		/// <param name="value">The object to be inserted into the cache.</param>
		/// <param name="strategy">The strategy to apply for the inserted item (use Temporary for objects 
		/// that are collectible and Permanent for objects you wish to keep forever).</param>
		public void Insert( TKey key, TValue value, CacheStrategy strategy )
		{
			synchronizer.Write( () => entries[ key ] = new CacheEntry<TKey,TValue>( key, value, strategy ) );
		}
		#endregion

		#region Get Methods
		/// <summary>
		/// Retrieves an entry from the cache using the given key.
		/// </summary>
		/// <param name="key">The cache key of the item to retrieve.</param>
		/// <returns>The retrieved cache item or null if not found.</returns>
		public TValue Get( TKey key )
		{
			return synchronizer.Read( () => { CacheEntry<TKey,TValue> entry;
											  return entries.TryGetValue(key,out entry) ? entry.Value : null; } );
		}

		private CacheEntry<TKey,TValue> GetEntry( TKey key )
		{
			return synchronizer.Read( () => { CacheEntry<TKey,TValue> entry;
											  return entries.TryGetValue(key,out entry) ? entry : null; } );
		}

		private CacheEntry<TKey,TValue> GetByValue( TValue instance )
		{
			return synchronizer.Read(() => entries.Values.FirstOrDefault(v => v == instance));
		}
		#endregion

		#region Remove Methods
		/// <summary>
		/// Removes the object associated with the given key from the cache.
		/// </summary>
		/// <param name="key">The cache key of the item to remove.</param>
		/// <returns>True if an item removed from the cache and false otherwise.</returns>
		public bool Remove( TKey key )
		{
			return synchronizer.Write( () => entries.Remove(key) );
		}
		#endregion

		#region Clear Methods
		/// <summary>
		/// Removes all entries from the cache.
		/// </summary>
		public void Clear()
		{
			synchronizer.Write( entries.Clear );
		}

		/// <summary>
		/// Process all entries in the cache and remove entries that refer to collected entries.
		/// </summary>
		/// <returns>The number of live cache entries still in the cache.</returns>
		private int ClearCollected()
		{
			return synchronizer.Write( () => ClearCollected( entries, entries.Values.ToList().Where(e => e.IsCollected) ) );
		}
		private static int ClearCollected( Dictionary<TKey,CacheEntry<TKey,TValue>> entries, IEnumerable<CacheEntry<TKey,TValue>> collectedEntries )
		{
			foreach (var entry in collectedEntries)
			{
				entries.Remove(entry.Key);
			}
			return entries.Count;
		}
		#endregion

		#region ToString
		/// <summary>
		/// This method returns a string with information on the cache contents (number of contained objects).
		/// </summary>
		public override string ToString()
		{
			int count = ClearCollected();
			return count > 0 ? String.Format( "Cache contains {0} live objects.", count ) : "Cache is empty.";
		}
		#endregion
	}
}