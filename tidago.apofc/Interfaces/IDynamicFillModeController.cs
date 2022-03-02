using System.Collections.Generic;

using tidago.apofc.Helpers;

namespace tidago.apofc.Interfaces
{
	/// <summary>
	/// Interface for process control when filling the model
	/// </summary>
	public interface IDynamicFillModeController
	{
		/// <summary>
		/// This method called when start fill model.
		/// </summary>
		/// <param name="nodes">Items of FormCollection that are prepared for this model.</param>
		void OnStartFillModel(IEnumerable<IFormTreeNode> nodes);

		/// <summary>
		/// This method called before set value to property.
		/// </summary>
		/// <param name="node">FormCollection element.</param>
		/// <param name="propertyName">Affected name of property.</param>
		/// <param name="value">New value for property.</param>
		void OnBeforeSetPropertyValue(IFormTreeNode node, string propertyName, object value);

		/// <summary>
		/// This method is called when the model finishes populating.
		/// </summary>
		/// <param name="affectedProperties">List of suggested fill properties.</param>
		void OnFinishFillModel(string[] affectedProperties);
	}
}
