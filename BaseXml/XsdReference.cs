namespace BaseXml
{
    public class XsdReference
    {
        public XsdReference(string path, string content)
        {
            this.SourcePath = path;
            this.Content = content;
        }

        public string SourcePath { get; internal set; }
        public string Content { get; internal set; }
    }
}
