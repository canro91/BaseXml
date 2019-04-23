using NUnit.Framework;
using System.Collections.Generic;

namespace BaseXml.Sample
{
    [TestFixture]
    public class NoteTests
    {
        [Test]
        public void ModifyNote_AnIncompleteNoteXml_LoadAndModifiesNote()
        {
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>A Body</body>
</note>";
            var note = new Note(xml);

            var body = note.Body;
            Assert.AreEqual("A Body", body);

            note.AddPS("I Love You");

            var ps = note.PS;
            Assert.AreEqual("I Love You", ps);
        }
    }

    class Note : BaseDocument
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

        public string Body
            => Evaluate(new XPath("/note/body"));

        public string PS
            => Evaluate(new XPath("/note/ps"));

        public void AddPS(string ps)
        {
            var psNode = $"<ps>{ps}</ps>";
            AddSiblingNodeAfterFirstOf(psNode, new XPath("/note/body"));
        }
    }
}
