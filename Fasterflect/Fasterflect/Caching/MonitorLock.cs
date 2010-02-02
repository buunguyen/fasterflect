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
	internal class MonitorLock : ILock
	{
		private readonly object synchronizer = new object();

		#region ILock
		public void Read(Action action)
		{
			lock( synchronizer )
			{
				action();
			}
		}

		public T Read<T>(Func<T> action)
		{
			lock( synchronizer )
			{
				return action();
			}
		}

		public void Write(Action action)
		{
			lock( synchronizer )
			{
				action();
			}
		}

		public T Write<T>(Func<T> action)
		{
			lock( synchronizer )
			{
				return action();
			}
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
		}
		#endregion
	}
}