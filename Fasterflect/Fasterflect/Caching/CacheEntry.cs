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

namespace Fasterflect.Caching
{
	/// <summary>
	/// This class is used to wrap entries stored in the cache.
	/// </summary>
	internal class CacheEntry<TKey,TValue> where TValue : class
	{
		private readonly TKey key;
		private readonly WeakReference weakReference;
		private TValue strongReference;

		#region Constructors
		public CacheEntry( TKey key, TValue value, CacheStrategy strategy )
		{
			this.key = key;
			if( strategy == CacheStrategy.Permanent )
			{
				strongReference = value;
			}
			else if( strategy == CacheStrategy.Temporary )
			{
				weakReference = new WeakReference( value );
			}
		}
		#endregion

		#region Properties
		public TKey Key
		{
			get { return key; }
		}

		public TValue Value
		{
			get { return strongReference ?? weakReference.Target as TValue; }
			set
			{
				if( strongReference != null )
				{
					strongReference = value;
				}
				else if( weakReference != null && weakReference.IsAlive )
				{
					weakReference.Target = value;
				}
			}
		}
		public bool IsCollectable
		{
			get { return strongReference == null; }
		}
		public bool IsCollected
		{
			get { return IsCollectable && ! weakReference.IsAlive; }
		}
		#endregion
	}
}