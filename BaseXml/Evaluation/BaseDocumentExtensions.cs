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
                var name = property.GetCustomAttribute<FromNode>(true)?.Node
                                ?? property.Name;
                var value = document.Evaluate(new XPath($"{document.Root}/{parentNodeName}/{name}"));
                property.SetValue(node, value);
            }

            return node;
        }
    }
}