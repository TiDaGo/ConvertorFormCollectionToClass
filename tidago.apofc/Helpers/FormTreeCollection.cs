using System;
using System.Collections.Generic;
using System.Linq;

namespace tidago.apofc.Helpers
{
	/// <summary>
	/// Form tree nodes collection
	/// </summary>
	public class FormTreeCollection : IFormTreeNode
	{
		/// <summary>
		/// Initialize node collection
		/// </summary>
		/// <param name="key">Collection key</param>
		/// <param name="parent">Parent element for this collection</param>
		public FormTreeCollection(string key, IFormTreeNode parent)
		{
			Key = key;
			Parent = parent;
		}

		/// <summary>
		/// Child nodes for this collection
		/// </summary>
		public IFormTreeNode[] Childs { get => _nodes?.ToArray() ?? Array.Empty<IFormTreeNode>(); }

		public string Key { get; set; }

		public IFormTreeNode Parent { get; set; }

		/// <summary>
		/// Elements of collection
		/// </summary>
		private List<IFormTreeNode> _nodes { get; set; }

		/// <summary>
		/// Add node to this collection.
		/// </summary>
		/// <param name="node">Added node.</param>
		public void AddChild(IFormTreeNode node)
		{
			if (_nodes == null)
				_nodes = new List<IFormTreeNode>();
			_nodes.Add(node);
		}

		/// <summary>
		/// Get node from this collection by key
		/// </summary>
		/// <param name="key">The key of the collection item to retrieve.</param>
		/// <returns>Child node from collection for key</returns>
		public IFormTreeNode GetChield(string key)
		{
			return Childs?.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.InvariantCulture));
		}
	}
}
