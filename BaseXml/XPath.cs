namespace BaseXml
{
    public class XPath
    {
        public XPath(string expression)
        {
            Expression = expression;
        }

        public string Expression { get; internal set; }
    }
}
