# BaseXML

BaseXML is a library to manipulate xml files. It allows you to evaluate an xpath expression, add new nodes and modify existing ones. By design, all mutable operations aren’t allowed if the document is signed.

BaseXML also offers a set of validators to help you check consistency in your xml documents.

## Usage

### Xml

```csharp
class Note: BaseDocument
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
		
	public void AddPS(string ps)
	{
		var psNode = $"<ps>{ps}</ps>";
		AddSiblingNodeAfterFirstOf(psNode, new XPath("/note/body"));
	}
}
```

#### Evaluate

```csharp
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
```

#### Add New Node

```csharp
note.AddPS("I Love You");

var ps = note.PS;
Assert.AreEqual("I Love You", ps);
```

### Validations

TODO

## Installation

Grab your own copy

## Contributing

Feel free to report any bug, ask for a new feature or just send a pull-request. All contributions are welcome.
	
## License

MIT
