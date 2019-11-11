using BaseXml.Evaluation;
using NUnit.Framework;
using System;

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
        public void Evaluate_OnlyStringsAndXmlDocumentWithCustomNamespaceAndAttribute_PopulatesPropertiesMappedToAttributes()
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

            var annotatedNote = note.EvaluateNode<MappedToXmlAttributeMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("en-US", annotatedNote.Language);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
        }

        [Test]
        public void Evaluate_OnlyStringsAndXmlDocumentWithAttribute_PopulatesPropertiesMappedToAttributes()
        {
            var note = MakeNamespacedNode(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Note>
  <Metadata Language=""en-US"">
    <From>Bob</From>
    <To>Alice</To>
    <Subject>Subject</Subject>
  </Metadata>
  <Body>Body</Body>
</Note>");

            var annotatedNote = note.EvaluateNode<WithXmlAttributeMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("en-US", annotatedNote.Language);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
        }

        [Test]
        public void Evaluate_DateAndStringsAndAllPropertyNamesMatchXmlNodes_PopulatesPropertiesWithXmlNodes()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Note>
  <Metadata>
    <From>Bob</From>
    <To>Alice</To>
    <Subject>Subject</Subject>
    <Date>2019-01-01</Date>
  </Metadata>
  <Body>Some Note content</Body>
</Note>");

            var annotatedNote = note.EvaluateNode<WithDateMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
            Assert.AreEqual(new DateTime(2019, 1, 1), annotatedNote.Date);
        }

        [Test]
        public void Evaluate_EnumAndStringsAndAllPropertyNamesMatchXmlNodes_PopulatesPropertiesWithXmlNodes()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Note>
  <Metadata>
    <From>Bob</From>
    <To>Alice</To>
    <Subject>Subject</Subject>
    <Type>Memo</Type>
  </Metadata>
  <Body>Some Note content</Body>
</Note>");

            var annotatedNote = note.EvaluateNode<WithEnumMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
            Assert.AreEqual(NoteType.Memo, annotatedNote.Type);
        }

        [Test]
        public void Evaluate_XmlDocumentWithNestedNode_PopulatesPropertiesWithXmlNodes()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Note>
  <Metadata>
    <From>Bob</From>
    <To>Alice</To>
    <Subject>Subject</Subject>
    <Delivery>
      <FromIp>127.0.0.1</FromIp>
      <DeliverAt>2019-01-01</DeliverAt>
    </Delivery>
  </Metadata>
  <Body>Some Note content</Body>
</Note>");

            var annotatedNote = note.EvaluateNode<WithNestedNodeMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
            Assert.IsNotNull(annotatedNote?.Delivery);
            Assert.AreEqual("127.0.0.1", annotatedNote.Delivery.FromIp);
            Assert.AreEqual(new DateTime(2019, 1, 1), annotatedNote.Delivery.DeliverAt);
        }

        [Test]
        public void Evaluate_XmlDocumentWithTwoLevelNestedNode_PopulatesPropertiesWithXmlNodes()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Note>
  <Metadata>
    <From>Bob</From>
    <To>Alice</To>
    <Subject>Subject</Subject>
    <FirstNode>
      <SecondNode>
        <Name>A name</Name>
      </SecondNode>
    </FirstNode>
  </Metadata>
  <Body>Some Note content</Body>
</Note>");

            var annotatedNote = note.EvaluateNode<WithTwoLevelNestedNodeMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
            Assert.IsNotNull(annotatedNote?.FirstNode);
            Assert.IsNotNull(annotatedNote.FirstNode?.SecondNode);
            Assert.AreEqual("A name", annotatedNote.FirstNode.SecondNode.Name);
        }

        [Test]
        public void Evaluate_PropertyNamesMatchXmlNodesWithCamelCaseNames_PopulatesPropertiesWithXmlNodes()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <metadata>
    <from>Bob</from>
    <to>Alice</to>
    <subject>Subject</subject>
  </metadata>
  <body>Some note content</body>
</note>");

            var annotatedNote = note.EvaluateNode<Metadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
        }

        [Test]
        public void Evaluate_PropertyNamesMatchXmlAttributesWithCamelCaseNames_PopulatesPropertiesWithXmlNodes()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <metadata language=""en-US"">
    <from>Bob</from>
    <to>Alice</to>
    <subject>Subject</subject>
  </metadata>
  <body>Some note content</body>
</note>");

            var annotatedNote = note.EvaluateNode<WithXmlAttributeMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("en-US", annotatedNote.Language);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
        }

        [Test]
        public void Evaluate_XmlDocumentWithNestedNodeAndCamelCaseNames_PopulatesPropertiesWithXmlNodes()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <metadata>
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
</note>");

            var annotatedNote = note.EvaluateNode<ToMatchCamelCaseNodeNamesWithNestedNodeMetadata>();

            Assert.IsNotNull(annotatedNote);
            Assert.AreEqual("Bob", annotatedNote.From);
            Assert.AreEqual("Alice", annotatedNote.To);
            Assert.AreEqual("Subject", annotatedNote.Subject);
            Assert.IsNotNull(annotatedNote?.Delivery);
            Assert.AreEqual("127.0.0.1", annotatedNote.Delivery.FromIp);
            Assert.AreEqual(new DateTime(2019, 1, 1), annotatedNote.Delivery.DeliverAt);
        }

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

    [FromNode("Metadata")]
    internal class WithDateMetadata : INode
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
    }

    [FromNode("Metadata")]
    internal class WithEnumMetadata : INode
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public NoteType? Type { get; set; }
    }

    internal enum NoteType
    {
        Salutation,
        Reminder,
        Memo
    }

    [FromNode("Metadata")]
    internal class WithXmlAttributeMetadata : INode
    {
        [FromAttr]
        public string Language { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
    }

    [FromNode("ns:metadata")]
    internal class AnnotatedMetadata : INode
    {
        [FromNode("ns:from")]
        public string From { get; set; }

        [FromNode("ns:to")]
        public string To { get; set; }

        [FromNode("ns:subject")]
        public string Subject { get; set; }
    }

    [FromNode("ns:metadata")]
    internal class MappedToXmlAttributeMetadata : INode
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

    [FromNode("metadata")]
    internal class ToMatchCamelCaseNodeNamesWithNestedNodeMetadata : INode
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public Delivery Delivery { get; set; }
    }

    [FromNode("Metadata")]
    internal class WithNestedNodeMetadata : INode
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public Delivery Delivery { get; set; }
    }

    internal class Delivery : INode
    {
        public string FromIp { get; set; }
        public DateTime DeliverAt { get; set; }
    }

    [FromNode("Metadata")]
    internal class WithTwoLevelNestedNodeMetadata : INode
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public FirstNode FirstNode { get; set; }
    }

    internal class FirstNode : INode
    {
        public SecondNode SecondNode { get; set; }
    }

    internal class SecondNode : INode
    {
        public string Name { get; set; }
    }
}