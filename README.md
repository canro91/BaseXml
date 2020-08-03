# BaseXML

![](https://img.shields.io/badge/netstandard-2.0-brightgreen.svg) ![](https://github.com/canro91/BaseXml/workflows/Build/badge.svg) ![](https://img.shields.io/github/license/canro91/BaseXml)

BaseXML is a library to manipulate and validate xml files. It allows you to evaluate xpath expressions, add new nodes or attributes and modify existing ones. By design, all mutable operations aren’t allowed if the document is signed. BaseXML offers a set of validators to check nodes of your xml documents.

## Usage

### Manipulate documents

BaseXML has these methods to modify xml documents:

* Evaluate
* MultipleEvaluate
* AddChildren
* PrependChildren
* AddOrReplaceSiblingNodeAfterFirstOf
* AddSiblingNodeAfterFirstOf
* AddSiblingNodeBeforeFirstOf
* ChangeValueOfNode
* AddOrChangeValueOfAttribute
* ChangeValueOfAttribute

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

#### Evaluate an xpath expression

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

#### Add a new node

```csharp
note.AddPS("I Love You");

var ps = note.PS;
Assert.AreEqual("I Love You", ps);
```

Please, take a look at the [Sample project](https://github.com/canro91/BaseXml/tree/master/BaseXml.Sample) to see how to add new nodes, evaluate an xpath and map a POCO to an xml node.

### Validations

BaseXml provides some per-node validations on top of a [FluentValidation](https://github.com/FluentValidation/FluentValidation) validator. These are some of them:

* Required
* Value
* Matches
* InKeyValue
* Length
* AreEqual
* ValidateIf

## Installation

Grab your own copy

## Contributing

Feel free to report any bug, ask for a new feature or just send a pull-request. All contributions are welcome.
	
## License

MIT
