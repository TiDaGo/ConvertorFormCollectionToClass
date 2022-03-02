using System;
using System.Linq;

using Microsoft.AspNetCore.Http;

namespace tidago.apofc.Helpers
{
	public static class FormTreeCollector
	{
		/// <summary>
		/// Convert FormCollection to tree
		/// </summary>
		/// <param name="collection">FormCollection</param>
		/// <returns>Tree from FormCollection</returns>
		public static IFormTreeNode[] ConvertToTree(this IFormCollection collection)
		{
			FormTreeCollection node = new FormTreeCollection("", null);

			(string[] path, string formKey)[] mkeys = collection.Keys
				.Union(collection.Files.Select(xf => xf.Name))
				.Select(x => (path: GetPath(x), formKey: x))
				.ToArray();

			foreach ((string[] path, string formKey) in mkeys)
			{
				FillCollection(collection, 0, formKey, path, node);
			}
			return node.Childs;
		}

		/// <summary>
		/// Fill chieds
		/// </summary>
		/// <param name="collection">Form collection</param>
		/// <param name="level">Level in path</param>
		/// <param name="formKey">Form collection element key</param>
		/// <param name="path">Form collection element path</param>
		/// <param name="parentNode">Filling parent node</param>
		private static void FillCollection(IFormCollection collection, int level, string formKey, string[] path, FormTreeCollection parentNode)
		{
			bool isCollection = path.Length > level + 1;
			IFormTreeNode newNode = null;
			if (isCollection)
			{
				string collectionName = path[level];
				if (!(parentNode.GetChield(collectionName) is FormTreeCollection currentNode))
				{
					currentNode = new FormTreeCollection(collectionName, parentNode);
					newNode = currentNode;
				}
				FillCollection(collection, level + 1, formKey, path, currentNode);
			}
			else
			{
				string fieldName = GetFieldName(formKey);
				IFormFile file = collection.Files.FirstOrDefault(x => x.Name == formKey);
				if (file != null)
				{
					newNode = new FormTreeNode(fieldName, parentNode, file);
				}
				else
				{
					newNode = new FormTreeNode(fieldName, parentNode, collection[formKey]);
				}
			}

			if (!(newNode is null))
			{
				parentNode.AddChild(newNode);
			}
		}

		/// <summary>
		/// Get field name from key
		/// </summary>
		/// <param name="key">FormCollection element key</param>
		/// <returns>Field name</returns>
		private static string GetFieldName(string key)
		{
			string[] sequence = key.Split('.');
			return sequence.Length == 0
				? string.Empty
				: sequence.LastOrDefault();
		}

		/// <summary>
		/// Get path with to field from key
		/// </summary>
		/// <param name="key">FormCollection element key</param>
		/// <returns>Field path</returns>
		private static string[] GetPath(string key)
		{
			return key?.Split('.') ?? Array.Empty<string>();
		}
	}
}
