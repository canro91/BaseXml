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
            foreach (PropertyInfo property in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attrName = property.GetCustomAttribute<FromAttr>(true)?.Node;
                var nodeName = property.GetCustomAttribute<FromNode>(true)?.Node
                                ?? property.Name;

                var xpath = !string.IsNullOrEmpty(attrName)
                                ? new XPath($"{document.Root}/{parentNodeName}/@{attrName}")
                                : new XPath($"{document.Root}/{parentNodeName}/{nodeName}");

                if (property.PropertyType.GetInterfaces().Any(t => t == typeof(INode)))
                {
                    var value = EvaluateNestedNode(document, property, xpath);
                    property.SetValue(node, value);
                }
                else
                {
                    var value = document.Evaluate(xpath, property.PropertyType);
                    property.SetValue(node, value);
                }
            }

            return node;
        }

        private static object EvaluateNestedNode(BaseDocument document, PropertyInfo baseProperty, XPath baseXPath)
        {
            var node = Activator.CreateInstance(baseProperty.PropertyType);

            foreach (PropertyInfo property in baseProperty.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attrName = property.GetCustomAttribute<FromAttr>(true)?.Node;
                var nodeName = property.GetCustomAttribute<FromNode>(true)?.Node
                                ?? property.Name;

                var xpath = !string.IsNullOrEmpty(attrName)
                                ? new XPath($"{baseXPath.Expression}/@{attrName}")
                                : new XPath($"{baseXPath.Expression}/{nodeName}");

                var value = document.Evaluate(xpath, property.PropertyType);
                property.SetValue(node, value);
            }

            return node;
        }
    }
}