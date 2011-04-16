#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
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

namespace Fasterflect
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class DynamicHandler
    {
        public static Type AddHandler(this Type targetType, string fieldName,
            Func<object[], object> func)
        {
            return InternalAddHandler(targetType, fieldName, func, null, false);
        }

        public static Type AddHandler(this object target, string fieldName,
            Func<object[], object> func)
        {
            return InternalAddHandler(target.GetType(), fieldName, func, target, false);
        }

        public static Type AssignHandler(this Type targetType, string fieldName,
            Func<object[], object> func)
        {
            return InternalAddHandler(targetType, fieldName, func, null, true);
        }

        public static Type AssignHandler(this object target, string fieldName,
            Func<object[], object> func)
        {
            return InternalAddHandler(target.GetType(), fieldName, func, target, true);
        }

        private static Type InternalAddHandler(Type targetType, string fieldName,
            Func<object[], object> func, object target, bool assignHandler)
        {
            Type delegateType;
            var eventInfo = targetType.GetEvent(fieldName);
            if (eventInfo != null && assignHandler)
                throw new ArgumentException("Event can be assigned.  Use AddHandler() overloads instead.");

            if (eventInfo != null)
            {
                delegateType = eventInfo.EventHandlerType;
                var dynamicHandler = BuildDynamicHandler(targetType, delegateType, func);
                eventInfo.GetAddMethod().Invoke(target, new Object[] { dynamicHandler });
            }
            else
            {
                var fieldInfo = targetType.Field(fieldName,
                                                    target == null
                                                        ? Flags.StaticAnyVisibility
                                                        : Flags.InstanceAnyVisibility);
                delegateType = fieldInfo.FieldType;
                var dynamicHandler = BuildDynamicHandler(targetType, delegateType, func);
                var field = assignHandler ? null : target == null
                                ? (Delegate)fieldInfo.Get()
                                : (Delegate)fieldInfo.Get(target);
                field = field == null
                            ? dynamicHandler
                            : Delegate.Combine(field, dynamicHandler);
                (target ?? targetType).SetFieldValue(fieldName, field);
            }
            return delegateType;
        }

        public static Delegate BuildDynamicHandler(this Type delegateOwnerType, Type delegateType, Func<object[], object> func)
        {
            var invokeMethod = delegateType.GetMethod("Invoke");
            var parameters = invokeMethod.GetParameters().Select(parm =>
                Expression.Parameter(parm.ParameterType, parm.Name)).ToArray();
            var instance = func.Target == null ? null : Expression.Constant(func.Target);
            var convertedParameters = parameters.Select(parm => Expression.Convert(parm, typeof(object)));
            var call = Expression.Call(instance, func.Method, Expression.NewArrayInit(typeof(object), convertedParameters));
            var body = invokeMethod.ReturnType == typeof(void)
                ? (Expression)call
                : Expression.Convert(call, invokeMethod.ReturnType);
            var expr = Expression.Lambda(delegateType, body, parameters);
            return expr.Compile();
        }
    }
}
