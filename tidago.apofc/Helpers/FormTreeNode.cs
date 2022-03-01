using Microsoft.AspNetCore.Http;

namespace tidago.apofc.Helpers
{
    /// <summary>
    /// Tree node for FolCollection element
    /// </summary>
    public class FormTreeNode : IFormTreeNode
    {
        /// <summary>
        /// Initialze tree node
        /// </summary>
        /// <param name="key">Element key.</param>
        /// <param name="parent">Parent for this element.</param>
        /// <param name="value">Element value.</param>
        public FormTreeNode(string key, IFormTreeNode parent, object value)
        {
            Key = key;
            Parent = parent;
            if (value is IFormFile fileValue)
            {
                FileValue = fileValue;
                IsFileContent = true;
            }
            else
            {
                StringValue = value.ToString();
                IsFileContent = false;
            }
        }

        public IFormFile FileValue { get; }
        public bool IsFileContent { get; }
        public string Key { get; }

        public IFormTreeNode Parent { get; }

        public string StringValue { get; }
    }
}