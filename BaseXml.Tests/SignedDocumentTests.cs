using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BaseXml.Tests
{
    [TestFixture]
    public class SignedDocumentTests
    {
        [Test]
        public void AddSiblingAfter_SignedDocument_ThrowsException()
        {
            var note = MakeSignedNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
</note>");
            var newNode = "<body>Body</body>";

            Assert.Throws<InvalidOperationException>(() => note.AddSiblingNodeAfterFirstOf(newNode, new XPath("/note/subject")));
        }

        private SignedNote MakeSignedNote(string xml)
        {
            return new SignedNote(xml.Trim());
        }

        [Test]
        public void AddSiblingBefore_SignedDocument_ThrowsException()
        {
            var note = MakeSignedNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <body>Body</body>
</note>");
            var newNode = "<subject>Subject</subject>";

            Assert.Throws<InvalidOperationException>(() => note.AddSiblingNodeBeforeFirstOf(newNode, new XPath("/note/body")));
        }

        [Test]
        public void AddChildren_SignedDocument_ThrowsException()
        {
            var note = MakeSignedNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
</note>");
            var newNode = "<body>Body</body><ps>Something else</ps>";

            Assert.Throws<InvalidOperationException>(() => note.AddChildren(newNode, new XPath("/note/subject")));
        }

        [Test]
        public void ChangeValueOfNode_SignedDocument_ThrowsException()
        {
            var note = MakeSignedNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>");

            Assert.Throws<InvalidOperationException>(() => note.ChangeValueOfNode(new XPath("/note/body"), "Another body"));
        }

        [Test]
        public void ChangeValueOfAttribute_SignedDocument_ThrowsException()
        {
            var note = MakeSignedNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body lang=""en"">Body</body>
</note>");

            Assert.Throws<InvalidOperationException>(() => note.ChangeValueOfAttribute(new XAttribute(new XPath("/note/body"), "lang"), "es"));
        }

        [Test]
        public void AddOrChangeValueOfAttribute_SignedDocument_ThrowsException()
        {
            var note = MakeSignedNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body lang=""en"">Body</body>
</note>");

            Assert.Throws<InvalidOperationException>(() => note.AddOrChangeValueOfAttribute(new XAttribute(new XPath("/note/body"), "lang"), "es"));
        }

        [Test]
        public void AddOrReplaceSiblingAfter_SignedDocument_ThrowsException()
        {
            var note = MakeSignedNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
</note>");
            var newNode = "<body>Body</body>";

            Assert.Throws<InvalidOperationException>(() => note.AddOrReplaceSiblingNodeAfterFirstOf(newNode, new XPath("/note/subject")));
        }
    }

    internal class SignedNote : BaseDocument
    {
        public SignedNote(string xml)
            : base(xml)
        {
        }

        public override IEnumerable<XsdReference> UblXsds
            => new XsdReference[0];

        public override IEnumerable<XmlNamespace> XmlNamespaces
            => new XmlNamespace[0];

        public override bool XmlIsSigned
            => true;
    }
}