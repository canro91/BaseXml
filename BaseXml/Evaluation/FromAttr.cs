using System;

namespace BaseXml.Evaluation
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FromAttr : Attribute
    {
        public string Node { get; set; }

        public FromAttr()
        {
        }

        public FromAttr(string node)
        {
            Node = node;
        }
    }
}
