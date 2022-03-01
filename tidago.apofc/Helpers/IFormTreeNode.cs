namespace tidago.apofc.Helpers {

    /// <summary>
    /// Interface of node from FormCollection element 
    /// </summary>
    public interface IFormTreeNode {
        /// <summary>
        /// Element key from FormCollection element for property name
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Parent form tree node for this element
        /// </summary>
        IFormTreeNode Parent { get; }
    }
}