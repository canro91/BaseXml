using System;

namespace BaseXml.Evaluation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class FromAttribute : Attribute
    {
        public string Node { get; set; }
    }
}
