using BaseXml.Evaluation;
using NUnit.Framework;

namespace BaseXml.Tests
{
    [TestFixture]
    public class DocumentTestsEvaluate
    {
        [Test]
        public void Evaluate_OnlyStringsAndAllPropertyNamesMatchXmlNodes_PopulatesPropertiesWithXmlNodes()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Note>
  <Metadata>
    <From>Bob</From>
    <To>Alice</To>
    <Subject>Subject</Subject>
  </Metadata>
  <Body>Some Note content</Body>
</Note>");

            var annotatedNote = note.EvaluateNode<Metadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
        }

        [Test]
        public void Evaluate_OnlyStringsAndPropertiesDontMatchXmlNodeNames_PopulatesProperties()
        {
            var note = MakeNamespacedNode(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<ns:note xmlns:ns=""com:basexml:Structures"">
  <ns:metadata>
    <ns:from>Bob</ns:from>
    <ns:to>Alice</ns:to>
    <ns:subject>Subject</ns:subject>
  </ns:metadata>
  <ns:body>Body</ns:body>
</ns:note>");

            var annotatedNote = note.EvaluateNode<AnnotatedMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
        }

        [Test]
        public void Evaluate_OnlyStringsAndDocumentWithAttributes_PopulatesPropertiesMappedToAttributes()
        {
            var note = MakeNamespacedNode(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<ns:note xmlns:ns=""com:basexml:Structures"">
  <ns:metadata lang=""en-US"">
    <ns:from>Bob</ns:from>
    <ns:to>Alice</ns:to>
    <ns:subject>Subject</ns:subject>
  </ns:metadata>
  <ns:body>Body</ns:body>
</ns:note>");

            var annotatedNote = note.EvaluateNode<AnnotatedMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("en-US", annotatedNote.Language);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
        }

        // Different data type

        private Note MakeNote(string xml)
        {
            return new Note(xml.Trim());
        }

        private NamespacedNode MakeNamespacedNode(string xml)
        {
            return new NamespacedNode(xml.Trim());
        }
    }

    internal class Metadata : INode
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
    }

    [FromNode("ns:metadata")]
    internal class AnnotatedMetadata : INode
    {
        [FromAttr("lang")]
        public string Language { get; set; }

        [FromNode("ns:from")]
        public string From { get; set; }

        [FromNode("ns:to")]
        public string To { get; set; }

        [FromNode("ns:subject")]
        public string Subject { get; set; }
    }
}