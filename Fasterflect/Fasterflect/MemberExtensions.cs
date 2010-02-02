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
using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect
{
	/// <summary>
	/// Extension methods for locating and accessing fields or properties, for situations where
	/// you do not care which it is.
	/// </summary>
	public static class MemberExtensions
	{
		#region Member Access

		#region Instance Getters

		public static TReturn GetFieldOrProperty<TReturn>(this object target, MemberTypes memberTypes,
		                                                  string fieldOrPropertyName)
		{
			return
				(TReturn)
				DelegateForGetFieldOrProperty(target.GetTypeAdjusted(), memberTypes, fieldOrPropertyName)(target);
		}

		public static AttributeGetter DelegateForGetFieldOrProperty(this Type targetType, MemberTypes memberTypes,
		                                                            string fieldOrPropertyName)
		{
			return (AttributeGetter) new MemberGetEmitter(
			                         	targetType, memberTypes, fieldOrPropertyName, false).GetDelegate();
		}

		#endregion

		#region Instance Setters

		public static object SetFieldOrProperty(this object target, MemberTypes memberTypes,
		                                        string fieldOrProperty, object value)
		{
			DelegateForSetFieldOrProperty(target.GetTypeAdjusted(), memberTypes, fieldOrProperty)(target, value);
			return target;
		}

		public static AttributeSetter DelegateForSetFieldOrProperty(this Type targetType, MemberTypes memberTypes,
		                                                            string fieldOrProperty)
		{
			return (AttributeSetter) new MemberSetEmitter(targetType, memberTypes, fieldOrProperty, false).GetDelegate();
		}

		#endregion

		#region Static Getters

		public static TReturn GetFieldOrProperty<TReturn>(this Type targetType, MemberTypes memberTypes,
		                                                  string fieldOrPropertyName)
		{
			return (TReturn) DelegateForGetStaticFieldOrProperty(targetType, memberTypes, fieldOrPropertyName)();
		}

		public static StaticAttributeGetter DelegateForGetStaticFieldOrProperty(this Type targetType, MemberTypes memberTypes,
		                                                                        string fieldOrPropertyName)
		{
			return
				(StaticAttributeGetter) new MemberGetEmitter(targetType, memberTypes, fieldOrPropertyName, true).GetDelegate();
		}

		#endregion

		#region Static Setters

		public static Type SetFieldOrProperty(this Type targetType, MemberTypes memberTypes,
		                                      string fieldOrProperty, object value)
		{
			DelegateForSetStaticFieldOrProperty(targetType, memberTypes, fieldOrProperty)(value);
			return targetType;
		}

		public static StaticAttributeSetter DelegateForSetStaticFieldOrProperty(this Type targetType,
		                                                                        MemberTypes memberTypes,
		                                                                        string fieldOrProperty)
		{
			return (StaticAttributeSetter)
			       new MemberSetEmitter(targetType, memberTypes, fieldOrProperty, true).GetDelegate();
		}

		#endregion

		#endregion

		#region Member Lookup

		#endregion
	}
}