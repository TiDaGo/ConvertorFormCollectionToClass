using System;

namespace tidago.apofc.Attributes
{
	/// <summary>
	/// Attribute for define a key in DynamicCollection
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class KeyPropertyFieldAttribute : Attribute
	{
		public KeyPropertyFieldAttribute()
		{
		}
	}
}
