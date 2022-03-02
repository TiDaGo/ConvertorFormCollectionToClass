using System.Collections.Generic;

using tidago.apofc.Helpers;

namespace tidago.apofc.Interfaces
{
	/// <summary>
	/// Interface for customizing model filling
	/// </summary>
	public interface IDynamicFillModel
	{
		/// <summary>
		/// Method for customize fill model
		/// </summary>
		/// <param name="nodes">Items of FormCollection that are prepared for this model.</param>
		/// <param name="propertyValueConverter">Announced in objectpopulator property value converter.</param>
		void DynamicFill(IEnumerable<IFormTreeNode> nodes, IPropertyValueConverter propertyValueConverter);
	}
}
