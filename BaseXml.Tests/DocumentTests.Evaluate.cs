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

        private Note MakeNote(string xml)
        {
            return new Note(xml.Trim());
        }
    }

    internal class Metadata : INode
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
    }
}
