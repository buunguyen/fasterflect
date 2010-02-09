using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FasterflectTest.SampleModel
{
	internal sealed class Zoo : Collection<Animal>
	{
		#region Properties
		public IEnumerable<T> Animals<T>()
		{
			return this.Where( a => a is T ).Cast<T>();
		}
		#endregion
	}
}
