namespace tidago.apofc.Helpers {

    /// <summary>
    /// Tree node for FolCollection element
    /// </summary>
    public class FormTreeNode : IFormTreeNode {

        /// <summary>
        /// Initialze tree node
        /// </summary>
        /// <param name="key">Element key.</param>
        /// <param name="parent">Parent for this element.</param>
        /// <param name="value">Element value.</param>
        public FormTreeNode(string key, IFormTreeNode parent, string value)
        {
            Key = key;
            Parent = parent;
            Value = value;
        }

        public string Key { get; set; }

        public IFormTreeNode Parent { get; set; }

        public string Value { get; set; }
    }
}