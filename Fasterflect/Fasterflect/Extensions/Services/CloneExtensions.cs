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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fasterflect
{
    /// <summary>
    /// Extension methods for deep cloning of objects.
    /// </summary>
    public static class CloneExtensions
    {
        /// <summary>
        /// Produces a deep clone of the <paramref name="source"/> object. Reference integrity is maintained and
        /// every unique object in the graph is cloned only once.
        /// A current limitation of this method is that all objects in the graph must have a default constructor.
        /// </summary>
        /// <typeparam name="T">The type of the object to clone.</typeparam>
        /// <param name="source">The object to clone.</param>
        /// <returns>A deep clone of the source object.</returns>
        public static T DeepClone<T>( this T source ) where T : class, new()
        {
            return source.DeepClone( null );
        }

        #region Private Helpers
        private static T DeepClone<T>( this T source, Dictionary<object, object> map ) where T : class, new()
        {
            Type type = source.GetType();
            var clone = type.IsArray ? Activator.CreateInstance( type, ((ICollection) source).Count ) as T : type.CreateInstance() as T;
            map = map ?? new Dictionary<object, object>();
            map[ source ] = clone;
			if( type.IsArray )
			{
				source.CloneArray( clone, map );
				return clone;
			}
        	IList<FieldInfo> fields = type.Fields( Flags.StaticInstanceAnyVisibility ).Where( f => ! f.IsLiteral ).ToList();
            object[] values = fields.Select( f => GetValue( f, source, map ) ).ToArray();
            for( int i = 0; i < fields.Count; i++ )
            {
				fields[ i ].Set( clone, values[ i ] );
            }
            return clone;
        }
        private static void CloneArray<T>( this T source, T clone, Dictionary<object, object> map ) where T : class, new()
        {
        	var sourceList = (IList) source;
        	var cloneList = (IList) clone;
			for( int i=0; i<sourceList.Count; i++ )
			{
				object element = sourceList[ i ];
            	cloneList[ i ] = element.ShouldClone() ? element.DeepClone( map ) : element;
			}
        }

        private static object GetValue( FieldInfo field, object source, Dictionary<object, object> map )
        {
            object result = field.IsLiteral ? field.GetRawConstantValue() : field.Get( source );
			if( result == null || ! result.ShouldClone() )
			{
				return result;
			}
			object clone;
            if( map.TryGetValue( result, out clone ) )
            {
                return clone;
            }
            return result.DeepClone( map );
        }

        private static bool ShouldClone( this object obj )
        {
			if( obj == null )
				return false;
        	Type type = obj.GetType();
			if( type.IsValueType || type == typeof(string) )
				return false;
			if( type.IsGenericTypeDefinition || obj is Type || obj is Delegate )
				return false;
			return true;
		}
        #endregion
    }
}