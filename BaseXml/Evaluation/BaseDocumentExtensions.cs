using BaseXml.Extensions;
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
                var hasAttrName = property.GetCustomAttribute<FromAttr>(true) != null;
                var attrName = hasAttrName
                                    ? property.GetCustomAttribute<FromAttr>(true)?.Node ?? property.Name
                                    : null;
                var nodeName = property.GetCustomAttribute<FromNode>(true)?.Node
                                ?? property.Name;

                var xpath = !string.IsNullOrEmpty(attrName)
                                ? new XPath($"{document.Root}/{parentNodeName}/@{attrName}")
                                : new XPath($"{document.Root}/{parentNodeName}/{nodeName}");

                var camelCaseXpath = !string.IsNullOrEmpty(attrName)
                                        ? new XPath($"{document.Root}/{parentNodeName.ToCamelCase()}/@{attrName.ToCamelCase()}")
                                        : new XPath($"{document.Root}/{parentNodeName.ToCamelCase()}/{nodeName.ToCamelCase()}");

                if (property.PropertyType.GetInterfaces().Any(t => t == typeof(INode)))
                {
                    var value = EvaluateNestedNode(document, property, xpath, camelCaseXpath);
                    property.SetValue(node, value);
                }
                else
                {
                    var value = document.Evaluate(xpath, property.PropertyType)
                                    ?? document.Evaluate(camelCaseXpath, property.PropertyType);
                    property.SetValue(node, value);
                }
            }

            return node;
        }

        private static object EvaluateNestedNode(BaseDocument document, PropertyInfo baseProperty, XPath baseXPath, XPath camelCaseBaseXPath)
        {
            var node = Activator.CreateInstance(baseProperty.PropertyType);

            foreach (PropertyInfo property in baseProperty.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var hasAttrName = property.GetCustomAttribute<FromAttr>(true) != null;
                var attrName = hasAttrName
                                    ? property.GetCustomAttribute<FromAttr>(true)?.Node ?? property.Name
                                    : null;
                var nodeName = property.GetCustomAttribute<FromNode>(true)?.Node
                                ?? property.Name;

                var xpath = !string.IsNullOrEmpty(attrName)
                                ? new XPath($"{baseXPath.Expression}/@{attrName}")
                                : new XPath($"{baseXPath.Expression}/{nodeName}");

                var camelCaseXpath = !string.IsNullOrEmpty(attrName)
                        ? new XPath($"{camelCaseBaseXPath.Expression}/@{attrName.ToCamelCase()}")
                        : new XPath($"{camelCaseBaseXPath.Expression}/{nodeName.ToCamelCase()}");

                var value = document.Evaluate(xpath, property.PropertyType)
                                ?? document.Evaluate(camelCaseXpath, property.PropertyType);
                property.SetValue(node, value);
            }

            return node;
        }
    }
}