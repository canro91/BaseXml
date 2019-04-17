namespace BaseXml
{
    public class XmlNamespace
    {
        public XmlNamespace(string prefix, string uri)
        {
            Prefix = prefix;
            Uri = uri;
        }

        public string Prefix { get; internal set; }
        public string Uri { get; internal set; }
    }
}
