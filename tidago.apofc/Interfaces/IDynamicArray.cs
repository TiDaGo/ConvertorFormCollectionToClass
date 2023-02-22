using System.Collections;
using System.Collections.Generic;

namespace tidago.apofc.Interfaces
{
	public interface IDynamicArray : IEnumerable
	{
		IReadOnlyCollection<object> GetKeys();

		bool HasKey(object key);

		object this[object key] { get; }
	}
}
