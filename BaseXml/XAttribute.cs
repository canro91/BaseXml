namespace BaseXml
{
    public class XAttribute
    {
        public XAttribute(XPath node, string attributeName)
        {
            NodeXPath = node.Expression;
            AttributeName = attributeName;
        }

        public string NodeXPath { get; set; }
        public string AttributeName { get; set; }

        public XPath ToXPath()
        {
            return new XPath($"{NodeXPath}/@{AttributeName}");
        }
    }
}
