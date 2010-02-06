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
using System.Threading;

namespace Fasterflect.Caching
{
	internal class ReaderWriterLock : ILock
	{
		private sealed class ReadLock : IDisposable
		{
			private readonly ReaderWriterLockSlim synchronizer;
			public ReadLock( ReaderWriterLockSlim synchronizer )
			{
				this.synchronizer = synchronizer;
				synchronizer.EnterReadLock();
			}
			public void Dispose()
			{
                synchronizer.ExitReadLock();
			}
		}
		private sealed class WriteLock : IDisposable
		{
			private readonly ReaderWriterLockSlim synchronizer;
			public WriteLock( ReaderWriterLockSlim synchronizer )
			{
				this.synchronizer = synchronizer;
				synchronizer.EnterWriteLock();
			}
			public void Dispose()
			{
                synchronizer.ExitWriteLock();
			}
		}
		private readonly ReaderWriterLockSlim synchronizer = new ReaderWriterLockSlim( LockRecursionPolicy.NoRecursion );

		#region ILock
		public IDisposable ReaderLock
		{
			get { return new ReadLock( synchronizer ); }
		}

		public IDisposable WriterLock
		{
			get { return new WriteLock( synchronizer ); }
		}		
		#endregion

		#region IDisposable
		public void Dispose()
		{
			synchronizer.Dispose();
		}
		#endregion
	}
}