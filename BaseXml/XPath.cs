using BaseXml.Extensions;
using System.Collections.Generic;

namespace BaseXml
{
    public class XPath
    {
        public XPath(string expression)
        {
            Expression = expression;
        }

        public string Expression { get; internal set; }

        public XPath Append(string expression)
        {
            return new XPath($"{Expression}/{expression}");
        }

        public XPath ToCamelCase()
        {
            var camelCase = new List<string>();

            var splitted = Expression.Split('/');
            foreach (var node in splitted)
            {
                var dotted = node.Split(':');
                if (dotted.Length == 1)
                {
                    var nodeName = dotted[0];
                    var camelCaseNodeName = nodeName.StartsWith("@")
                                                ? $"@{nodeName.Substring(1).ToCamelCase()}"
                                                : nodeName.ToCamelCase();
                    camelCase.Add(camelCaseNodeName);
                }
                else
                {
                    var ns = dotted[0];
                    var nodeName = dotted[1];
                    camelCase.Add($"{ns}:{nodeName.ToCamelCase()}");
                }
            }

            var expression = string.Join("/", camelCase);
            return new XPath(expression);
        }
    }
}
