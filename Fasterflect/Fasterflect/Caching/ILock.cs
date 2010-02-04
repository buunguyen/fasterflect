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
	internal interface ILock : IDisposable
	{
		void Read(Action action);

		TResult Read<TResult>( Func<TResult> action );

		TResult Read<TResult>( MethodInvoker invoker, params object[] parameters );

		void Write( Action action );

		TResult Write<TResult>( Func<TResult> action );
	}
}