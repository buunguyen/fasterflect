using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

#pragma warning disable 1591
namespace Fasterflect.Common
{
	public class InfoFactory
	{
		public Type Type { get; private set; }
		public Items<PropertyInfo> Properties { get { return new PropertyItems( Type ); } }
		public Items<MethodInfo> Methods { get { return new MethodItems( Type ); } }

		public InfoFactory( Type type )
		{
			Type = type;
		}
	}

	public abstract class Items<T> //: IEnumerable<T>
	{
		public Type Type { get; private set; }

		public Items( Type type )
		{
			Type = type;
		}

		#region Implementation of IEnumerable
		public abstract IEnumerator<T> GetEnumerator();

		//IEnumerator IEnumerable.GetEnumerator()
		//{
		//    return GetEnumerator();
		//}
		#endregion

		#region Linq
		public FilteredItems<T> Where( Expression<Func<T,bool>> predicate )
		{
			return null;
		}
		#endregion
	}
	public class FilteredItems<T> //: IEnumerable<T>
	{
		#region Implementation of IEnumerable
		public IEnumerator<T> GetEnumerator()
		{
		    return null;
		}

		//IEnumerator IEnumerable.GetEnumerator()
		//{
		//    return GetEnumerator();
		//}
		#endregion

		public ProjectedItems<T> Select<R>( Expression<Func<T,R>> projector )
		{
			return null;
		}
		public ProjectedItems<T> OrderBy<TKey>( Expression<Func<T,TKey>> keySelector )
		{
			return null;
		}
	}
	public class ProjectedItems<R> : IEnumerable<R>
	{
		#region Implementation of IEnumerable
		public IEnumerator<R> GetEnumerator()
		{
		    return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
		    return GetEnumerator();
		}
		#endregion
	}

	public class PropertyItems : Items<PropertyInfo>
	{
		public PropertyItems( Type type ) : base( type )
		{
		}

		#region Implementation of IEnumerable
		public override IEnumerator<PropertyInfo> GetEnumerator()
		{
			return Type.Properties( Flags.StaticInstanceAnyVisibility ).GetEnumerator();
		}
		#endregion
	}

	public class MethodItems : Items<MethodInfo>
	{
		public MethodItems( Type type ) : base( type )
		{
		}

		#region Implementation of IEnumerable
		public override IEnumerator<MethodInfo> GetEnumerator()
		{
			return Type.Methods( Flags.StaticInstanceAnyVisibility ).GetEnumerator();
		}
		#endregion
	}


	public class InfoFactoryTest
	{
		public void TestPropertyLookup()
		{
			var type = typeof(InfoFactory);
			var info = new InfoFactory( type );
			var properties = from p in info.Properties
			                 where p.Name == "Type"
							 orderby p.Name
			                 select p;
			var list = properties.ToList();
			var x = list.Count;
		}
	}
}
#pragma warning restore 1591
