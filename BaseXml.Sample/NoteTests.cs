using BaseXml.Evaluation;
using NUnit.Framework;
using System;

namespace BaseXml.Sample
{
    [TestFixture]
    public class NoteTests
    {
        [Test]
        public void ModifyNote_AnIncompleteNoteXml_LoadsAndModifiesNote()
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

        [Test]
        public void Evaluate_NoteXml_PopulatesPropertiesMappedToNodesAndAttributes()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <metadata language=""en-US"">
    <from>Bob</from>
    <to>Alice</to>
    <subject>Subject</subject>
    <type>Memo</type>
    <delivery>
      <fromIp>127.0.0.1</fromIp>
      <deliverAt>2019-01-01</deliverAt>
    </delivery>
  </metadata>
  <body>Some Note content</body>
</note>";
            var note = new Note(xml);

            var annotatedNote = note.EvaluateNode<Metadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("en-US", annotatedNote.Language);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
            Assert.AreEqual(NoteType.Memo, annotatedNote.Type);
            Assert.IsNotNull(annotatedNote?.Delivery);
            Assert.AreEqual("127.0.0.1", annotatedNote.Delivery.FromIp);
            Assert.AreEqual(new DateTime(2019, 1, 1), annotatedNote.Delivery.DeliverAt);

            Assert.AreEqual("Some Note content", note.Body);
        }
    }

    internal class Note : BaseDocument
    {
        public Note(string xml)
            : base(xml)
        {
        }

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

    [FromNode("metadata")]
    internal class Metadata : INode
    {
        [FromAttr("language")]
        public string Language { get; set; }

        [FromNode("from")]
        public string From { get; set; }

        [FromNode("to")]
        public string To { get; set; }

        [FromNode("subject")]
        public string Subject { get; set; }

        [FromNode("type")]
        public NoteType? Type { get; set; }

        [FromNode("delivery")]
        public Delivery Delivery { get; set; }
    }

    internal enum NoteType
    {
        Reminder,
        Memo
    }

    internal class Delivery : INode
    {
        [FromNode("fromIp")]
        public string FromIp { get; set; }

        [FromNode("deliverAt")]
        public DateTime DeliverAt { get; set; }
    }
}
