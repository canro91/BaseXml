using System;

namespace BaseXml.Evaluation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class FromNode : Attribute
    {
        public string Node { get; set; }

        public FromNode(string name)
        {
            Node = name;
        }
    }
}
