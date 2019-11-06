using System;
using System.Linq;
using System.Reflection;

namespace BaseXml.Evaluation
{
    public static class BaseDocumentExtensions
    {
        public static T EvaluateNode<T>(this BaseDocument document) where T : INode, new()
        {
            var node = new T();

            var parentNodeName = node.GetType().GetCustomAttribute<FromNode>(true)?.Node
                                    ?? node.GetType().Name;
            var root = new XPath(document.Root).Append(parentNodeName);
            EvaluateProperties(document, root, node);

            return node;
        }

        private static void EvaluateProperties(BaseDocument document, XPath root, object obj)
        {
            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                XPath xpath = XPathForProperty(root, property);

                if (property.PropertyType.GetInterfaces().Any(t => t == typeof(INode)))
                {
                    var node = Activator.CreateInstance(property.PropertyType);
                    EvaluateProperties(document, xpath, node);
                    property.SetValue(obj, node);
                }
                else
                {
                    var value = document.Evaluate(xpath, property.PropertyType)
                                    ?? document.Evaluate(xpath.ToCamelCase(), property.PropertyType);
                    property.SetValue(obj, value);
                }
            }
        }

        private static XPath XPathForProperty(XPath root, PropertyInfo property)
        {
            var hasAttrName = property.GetCustomAttribute<FromAttr>(true) != null;
            var attrName = hasAttrName
                                ? property.GetCustomAttribute<FromAttr>(true)?.Node ?? property.Name
                                : null;
            var nodeName = property.GetCustomAttribute<FromNode>(true)?.Node
                            ?? property.Name;

            var xpath = !string.IsNullOrEmpty(attrName)
                            ? root.Append($"@{attrName}")
                            : root.Append(nodeName);
            return xpath;
        }
    }
}