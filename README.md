# BaseXML

![](https://img.shields.io/badge/netstandard-2.0-brightgreen.svg) ![](https://github.com/canro91/BaseXml/workflows/Build/badge.svg) ![](https://img.shields.io/github/license/canro91/BaseXml)

BaseXML is a library to manipulate and validate XML files. It evaluates XPath expressions, adds new nodes or attributes, and modifies existing ones. By design, BaseXML doesn't perform mutable operations on signed XML documents.

Also, BaseXML offers a set of validators to check the nodes of XML documents. 

## Usage

### Manipulate documents

To use BaseXML, create a class inheriting from `BaseDocument` to define mutable custom operations.

```csharp
class Note: BaseDocument
{
    public Note(string xml)
        : base(xml)
    {
    }
        
    public string Body
        => Evaluate(new XPath("/note/body"));
        
    public void AddPS(string ps)
    {
        var psNode = $"<ps>{ps}</ps>";
        AddSiblingNodeAfterFirstOf(psNode, new XPath("/note/body"));
    }
}
```

BaseXML has methods to add or prepend child nodes, add sibling nodes before or after a given node, and change the value of existing nodes and attributes. These methods are:

* AddChildren
* PrependChildren
* AddSiblingNodeBeforeFirstOf
* AddSiblingNodeAfterFirstOf
* AddOrReplaceSiblingNodeAfterFirstOf
* ChangeValueOfNode
* ChangeValueOfAttribute
* AddOrChangeValueOfAttribute

### Evaluate an xpath expression

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
// "A Body"
```

#### Add a new node

```csharp
note.AddPS("I Love You");

var ps = note.PS;
// "I Love You"
```

Please, look at the [Sample project](https://github.com/canro91/BaseXml/tree/master/BaseXml.Sample) to see how to add new nodes, evaluate an XPath and map a POCO to an XML node.

### Validations

BaseXml provides some per-node validations on top of a [FluentValidation](https://github.com/FluentValidation/FluentValidation) validator.

```csharp
string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<note>
    <from>Bob</from>
    <to>Alice</to>
    <subject>Subject</subject>
    <body>A Body</body>
</note>";
var note = new Note(xml);

var validator = new CheckDocument(new Dictionary<XPath, IEnumerable<IValidateNode>>
{
    { new XPath("/note/subject"), new List<IValidateNode> { new Length(min: 1, max: 10) } },
    { new XPath("/note/body"), new List<IValidateNode> { new Required() } }
});

ValidationResult results = validator.Validate(note);

results.IsValid
// false
```

These are BaseXML node validators:

* AreEqual
* InKeyValue
* Length
* Matches
* Required
* ValidateIf
* Value

## Installation

Grab your own copy

## Contributing

Feel free to report any bug, ask for a new feature or just send a pull request. All contributions are welcome.
	
## License

MIT
