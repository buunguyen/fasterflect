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
		private readonly ReaderWriterLockSlim synchronizer = new ReaderWriterLockSlim( LockRecursionPolicy.NoRecursion );

		#region ILock
		public void Read(Action action)
		{
			synchronizer.EnterReadLock();
            try 
            {
            	action();
            }
            finally
            {
                synchronizer.ExitReadLock();
            }
		}

		public TResult Read<TResult>( Func<TResult> action )
		{
			synchronizer.EnterReadLock();
            try 
            {
            	return action();
            }
            finally
            {
                synchronizer.ExitReadLock();
            }
		}

        public TResult Read<TResult>(MethodInvoker invoker, object target, params object[] parameters)
		{
			synchronizer.EnterReadLock();
			try
			{
                return (TResult)invoker(target, parameters);
			}
			finally
			{
				synchronizer.ExitReadLock();
			}
		}

		public void Write(Action action)
		{
			synchronizer.EnterWriteLock();
            try 
            {
            	action();
            }
            finally
            {
                synchronizer.ExitWriteLock();
            }
		}

		public TResult Write<TResult>( Func<TResult> action )
		{
			synchronizer.EnterWriteLock();
            try 
            {
            	return action();
            }
            finally
            {
                synchronizer.ExitWriteLock();
            }
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