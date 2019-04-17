using NUnit.Framework;
using System.Collections.Generic;

namespace BaseXml.Tests
{
    [TestFixture]
    public class DocumentTests
    {
        [Test]
        public void AddSiblingNodeAfter_ExistingSiblingNode_AddsNewNode()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
</note>");
            var newNode = "<body>Body</body>";

            note.AddSiblingNodeAfterFirstOf(newNode, new XPath("/note/subject"));

            var body = note.Evaluate(new XPath("/note/body"));
            Assert.AreEqual("Body", body);
        }

        private Note MakeNote(string xml)
        {
            return new Note(xml.Trim());
        }

        [Test]
        public void AddSiblingNodeAfter_ExistingSiblingNode_AddsNewNodeAfterGivenNode()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
</note>");
            var newNode = "<body>Body</body>";
            var after = new XPath("/note/subject");

            note.AddSiblingNodeAfterFirstOf(newNode, after);

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>";
            Assert.AreEqual(expected, note.Xml);
        }

        [Test]
        public void AddSiblingNodeAfter_NonExistingSiblingNode_DoesNotAddNewNode()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
</note>");
            var newNode = "<body>Body</body>";

            note.AddSiblingNodeAfterFirstOf(newNode, new XPath("/note/not-exist-node"));

            var body = note.Evaluate(new XPath("/note/body"));
            Assert.IsNull(body);
        }

        [Test]
        public void AddSiblingNodeBefore_ExistingSiblingNode_AddsNewNode()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <body>Body</body>
</note>");
            var newNode = "<subject>Subject</subject>";

            note.AddSiblingNodeBeforeFirstOf(newNode, new XPath("/note/body"));

            var subject = note.Evaluate(new XPath("/note/subject"));
            Assert.AreEqual("Subject", subject);
        }

        [Test]
        public void AddSiblingNodeBefore_ExistingSiblingNode_AddsNewNodeBeforeGivenNode()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <body>Body</body>
</note>");
            var newNode = "<subject>Subject</subject>";
            var before = new XPath("/note/body");

            note.AddSiblingNodeBeforeFirstOf(newNode, before);

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>";
            Assert.AreEqual(expected, note.Xml);
        }

        [Test]
        public void AddChildren_ExistingSiblingNode_AddChildrenAfterGivenNode()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
</note>");
            var newNode = "<subject>Subject</subject><body>Body</body>";

            note.AddChildren(newNode, new XPath("/note"));

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>";
            Assert.AreEqual(expected, note.Xml);
        }

        [Test]
        public void ChangeValueOfNode_ExistingNode_ChangesValueOfNode()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>");
            var xPath = new XPath("/note/body");

            note.ChangeValueOfNode(xPath, "Another body");

            string body = note.Evaluate(xPath);
            Assert.AreEqual("Another body", body);
        }

        [Test]
        public void ChangeValueOfNode_NonExistingNode_DoesNotChangeValueOfNode()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>");
            var xPath = new XPath("/note/non-existing");

            note.ChangeValueOfNode(xPath, "Another body");

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>";
            Assert.AreEqual(expected, note.Xml);
        }

        [Test]
        public void ChangeValueOfAttribute_ExistingAttributeInNode_ChangesValueOfAttribute()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body lang=""en"">Body</body>
</note>");
            var attribute = new XAttribute(new XPath("/note/body"), "lang");

            note.ChangeValueOfAttribute(attribute, "es");

            string lang = note.Evaluate(attribute);
            Assert.AreEqual("es", lang);
        }

        [Test]
        public void ChangeValueOfAttribute_NonExistingAttributeInNode_DoesNotChangeValueOfAttribute()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>");
            var attribute = new XAttribute(new XPath("/note/body"), "lang");

            note.ChangeValueOfAttribute(attribute, "es");

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>";
            Assert.AreEqual(expected, note.Xml);
        }
    }

    public class Note : BaseDocument
    {
        public Note(string xml)
            : base(xml)
        {
        }

        public override IEnumerable<XsdReference> UblXsds
            => new XsdReference[0];

        public override IEnumerable<XmlNamespace> XmlNamespaces
            => new XmlNamespace[0];

        public override bool XmlIsSigned
            => false;
    }
}
