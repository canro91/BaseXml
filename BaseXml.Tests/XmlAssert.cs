using NUnit.Framework;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;

namespace BaseXml.Tests
{
    public class XmlAssert
    {
        public static void AreEqual(string expected, string actual)
        {
            Diff d = DiffBuilder.Compare(Input.FromString(expected))
                        .WithTest(actual).Build();
            Assert.IsFalse(d.HasDifferences());
        }
    }
}