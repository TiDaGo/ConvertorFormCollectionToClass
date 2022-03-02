using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Http;

using tidago.apofc.Convertors;
using tidago.apofc.Helpers;
using tidago.apofc.Interfaces;

namespace tidago.apofc
{
	/// <summary>
	/// Filling properties into object from a FormCollection elements
	/// </summary>
	public class ObjectPopulator
	{
		/// <summary>
		/// Initialize populator with default property value converter
		/// </summary>
		public ObjectPopulator() : this(new DefaultPropertyValueConverter())
		{
		}

		/// <summary>
		/// Initialize populator with custom property converter
		/// </summary>
		public ObjectPopulator(IPropertyValueConverter valueConverter)
		{
			Converter = valueConverter;
		}

		/// <summary>
		/// Property value converter
		/// </summary>
		protected virtual IPropertyValueConverter Converter { get; }

		/// <summary>
		/// Filling entities from FormCollection with default property converter
		/// </summary>
		/// <typeparam name="T">Type of filling object.</typeparam>
		/// <param name="collection">The FormCollection.</param>
		/// <param name="fillTObject">The filling object.</param>
		/// <returns>Object after fill value.</returns>
		public T Populate<T>(IFormCollection collection, T fillTObject)
		{
			IFormTreeNode[] nodes = FormTreeCollector.ConvertToTree(collection);
			DynamicFill(nodes, fillTObject);

			return fillTObject;
		}

		/// <summary>
		/// Filling entities from FormCollection with default property converter
		/// </summary>
		/// <typeparam name="T">Type of filling object</typeparam>
		/// <param name="nodes">Forn tree nodes</param>
		/// <param name="fillTObject">The filling object</param>
		/// <returns>Object after fill value</returns>
		public T Populate<T>(IEnumerable<IFormTreeNode> nodes, T fillTObject)
		{
			DynamicFill(nodes, fillTObject);
			return fillTObject;
		}

		private bool AutoSetValueMember(object obj, string fieldName, MemberInfo member, Type memberType, string dataValue, IPropertyValueConverter converter)
		{
			// If used read-only model, with other type fields, need use the original type
			Type originalPropType = obj.GetType().GetProperty(fieldName)?.PropertyType ?? member.DeclaringType;
			// This convertation release for check and normalize value from formcollection to property
			object propValue = converter.ConvertToPropertyType(originalPropType, dataValue);
			// Convert property to field
			object fieldValue = Converter.ConvertToFieldType(memberType, propValue);

			return AutoSetValueMember(obj, member, fieldValue);
		}

		private bool AutoSetValueMember(object obj, MemberInfo member, object fieldValue)
		{
			// Set new field value
			obj.SetValue(member, fieldValue);
			return true;
		}

		/// <summary>
		/// Dynamic fill model
		/// </summary>
		/// <typeparam name="T">Type of filling object.</typeparam>
		/// <param name="nodes">Items of FormCollection that are prepared for this model.</param>
		/// <param name="obj">Object for set values from items</param>
		private void DynamicFill<T>(IEnumerable<IFormTreeNode> nodes, T obj)
		{
			if (obj is IDynamicFillModel dynamicFillModel)
			{
				dynamicFillModel.DynamicFill(nodes, Converter);
				return;
			}

			if (!(obj is IDynamicFillModeController dynamicFillModeController))
				dynamicFillModeController = null;

			dynamicFillModeController?.OnStartFillModel(nodes);

			foreach (IFormTreeNode node in nodes)
			{
				// Simple property
				if (node is FormTreeNode fte)
				{
					// Search fields
					(MemberInfo member, Type declaringType) = MemberHelpers.GetPropertyField(obj, fte.Key);

					// If FormCollection have other system fields
					if (member == null)
						continue;
					if (declaringType == typeof(IFormFile))
					{
						dynamicFillModeController?.OnBeforeSetPropertyValue(node, fte.Key, fte.FileValue);
						AutoSetValueMember(obj, member, fte.FileValue);
					}
					else
					{
						dynamicFillModeController?.OnBeforeSetPropertyValue(node, fte.Key, fte.StringValue);
						AutoSetValueMember(obj, fte.Key, member, declaringType, fte.StringValue, Converter);
					}
				}
				// Properties collection
				else if (node is FormTreeCollection ftc)
				{
					// Search fields
					(MemberInfo member, Type declaringType) = MemberHelpers.GetPropertyField(obj, node.Key);

					// If FormCollection have other system fields
					if (member == null)
						continue;

					object includedModel = null;
					if (member is FieldInfo field)
					{
						includedModel = field.GetValue(obj);
						if (includedModel == null)
						{
							includedModel = Activator.CreateInstance(field.FieldType);
						}
						dynamicFillModeController?.OnBeforeSetPropertyValue(node, node.Key, includedModel);
						field.SetValue(obj, includedModel);
					}
					else if (member is PropertyInfo property)
					{
						includedModel = property.GetValue(obj);
						if (includedModel == null)
						{
							includedModel = Activator.CreateInstance(property.PropertyType);
						}
						dynamicFillModeController?.OnBeforeSetPropertyValue(node, node.Key, includedModel);
						property.SetValue(obj, includedModel);
					}
					else
					{
						throw new NotImplementedException();
					}

					if (includedModel is IDynamicFillModel dynamicCollection)
					{
						dynamicCollection.DynamicFill(ftc.Childs, Converter);
					}
					else
					{
						DynamicFill(ftc.Childs, includedModel);
					}
					//TODO: No implement Array, List, and other IEnumerable class
				}
			}
			dynamicFillModeController?.OnFinishFillModel(nodes.Select(x => x.Key).ToArray());
		}
	}
}
