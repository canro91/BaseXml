using BaseXml.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace BaseXml
{
    public abstract class BaseDocument
    {
        protected XmlDocument _xmlDocument;
        protected XmlNamespaceManager _xmlNamespaceManager;
        protected XPathNavigator _navigator;

        public BaseDocument(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            this._xmlDocument = doc;

            XPathNavigator navigator = _xmlDocument.CreateNavigator();
            XmlNamespaceManager ns = new XmlNamespaceManager(navigator.NameTable);
            foreach (var item in XmlNamespaces)
            {
                ns.AddNamespace(item.Prefix, item.Uri);
            }

            this._xmlNamespaceManager = ns;
            this._navigator = navigator;
        }

        public abstract IEnumerable<XsdReference> UblXsds { get; }
        public abstract IEnumerable<XmlNamespace> XmlNamespaces { get; }
        public abstract bool XmlIsSigned { get; }

        public string Xml
        {
            get
            {
                using (StringWriter sw = new UTF8StringWriter())
                {
                    _xmlDocument.Save(sw);
                    return sw.ToString();
                }
            }
        }

        public string Root
        {
            get => _xmlDocument.DocumentElement.Name;
        }

        public string Evaluate(XPath xPath)
        {
            return Evaluate<string>(xPath);
        }

        public string Evaluate(XAttribute xAttribute)
        {
            return Evaluate<string>(xAttribute.ToXPath());
        }

        public T Evaluate<T>(XPath xPath, T @default = default)
        {
            var result = _navigator.Evaluate(xPath.Expression, _xmlNamespaceManager);
            var obj = Converter.GetValue(result);
            var converted = Converter.ChangeType(obj, typeof(T));
            return converted != null ? (T)converted : @default;
        }

        internal object Evaluate(XPath xPath, Type type)
        {
            var result = _navigator.Evaluate(xPath.Expression, _xmlNamespaceManager);
            var obj = Converter.GetValue(result);
            var converted = Converter.ChangeType(obj, type);
            return converted;
        }

        // TODO Add test for multiple evaluate
        protected internal IEnumerable<T> MultipleEvaluate<T>(XPath xPath)
        {
            var result = _navigator.Evaluate(xPath.Expression, _xmlNamespaceManager);
            var obj = Converter.GetFromMultipleValues(result);
            return obj?.Select(t => (T)Converter.ChangeType(t, typeof(T)));
        }

        public void AddChildren(string xml, params XPath[] xPaths)
        {
            if (XmlIsSigned)
                throw new InvalidOperationException("Document cannot be modified if signed");

            var allNamespaces = string.Join(" ", this.XmlNamespaces.Select(t => $"xmlns:{t.Prefix}=\"{t.Uri}\""));
            var dummyRoot = $@"<root {allNamespaces}>
    {xml}
</root>";
            var temp = new XmlDocument();
            temp.LoadXml(dummyRoot);

            XmlNode parentNode = xPaths.Select(t => _xmlDocument.SelectSingleNode(t.Expression, _xmlNamespaceManager))
                                       .FirstOrDefault(t => t != null);
            if (parentNode != null)
            {
                foreach (XmlNode item in temp.DocumentElement.ChildNodes)
                {
                    XmlNode newNode = parentNode.OwnerDocument.ImportNode(item, true);
                    parentNode.AppendChild(newNode);
                }
            }
        }

        public void AddOrReplaceSiblingNodeAfterFirstOf(string xml, params XPath[] xPaths)
        {
            // WARNING: Document shouldn't be modified afer signed
            if (XmlIsSigned)
                throw new InvalidOperationException("Document cannot be modified if signed");

            var allNamespaces = string.Join(" ", this.XmlNamespaces.Select(t => $"xmlns:{t.Prefix}=\"{t.Uri}\""));
            var dummyRoot = $@"<root {allNamespaces}>
    {xml}
</root>";
            var temp = new XmlDocument();
            temp.LoadXml(dummyRoot);

            var existingNodeName = temp.FirstChild?.FirstChild?.Name;
            var existingNodes = _xmlDocument.GetElementsByTagName(existingNodeName);
            if (existingNodes.Count > 0)
            {
                for (int i = existingNodes.Count - 1; i >= 0; i--)
                {
                    var node = existingNodes[i];
                    if (node.ParentNode.ParentNode.Name == "#document") node.ParentNode.RemoveChild(node);
                }
            }

            XmlNode parentNode = xPaths.Select(t => _xmlDocument.SelectNodes(t.Expression, _xmlNamespaceManager)
                                                                .Cast<XmlNode>()
                                                                .LastOrDefault())
                                       .FirstOrDefault(t => t != null);
            if (parentNode != null)
            {
                XmlNode newNode = parentNode.OwnerDocument.ImportNode(temp.DocumentElement.FirstChild, true);
                _xmlDocument.DocumentElement.InsertAfter(newNode, parentNode);
            }
        }

        public void AddSiblingNodeAfterFirstOf(string xml, params XPath[] xPaths)
        {
            if (XmlIsSigned)
                throw new InvalidOperationException("Document cannot be modified if signed");

            // WARNING: Add a dummy root node to include a portion of xml without
            // namespace declaration
            var allNamespaces = string.Join(" ", this.XmlNamespaces.Select(t => $"xmlns:{t.Prefix}=\"{t.Uri}\""));
            var dummyRoot = $@"<root {allNamespaces}>
    {xml}
</root>";
            var temp = new XmlDocument();
            temp.LoadXml(dummyRoot);

            XmlNode parentNode = xPaths.Select(t => _xmlDocument.SelectNodes(t.Expression, _xmlNamespaceManager)
                                                                .Cast<XmlNode>()
                                                                .LastOrDefault())
                                       .FirstOrDefault(t => t != null);
            if (parentNode != null)
            {
                XmlNode newNode = parentNode.OwnerDocument.ImportNode(temp.DocumentElement.FirstChild, true);
                parentNode.ParentNode.InsertAfter(newNode, parentNode);
            }
        }

        public void AddSiblingNodeBeforeFirstOf(string xml, params XPath[] xPaths)
        {
            if (XmlIsSigned)
                throw new InvalidOperationException("Document cannot be modified if signed");

            var allNamespaces = string.Join(" ", this.XmlNamespaces.Select(t => $"xmlns:{t.Prefix}=\"{t.Uri}\""));
            var dummyRoot = $@"<root {allNamespaces}>
    {xml}
</root>";
            var temp = new XmlDocument();
            temp.LoadXml(dummyRoot);

            XmlNode parentNode = xPaths.Select(t => _xmlDocument.SelectSingleNode(t.Expression, _xmlNamespaceManager))
                                       .FirstOrDefault(t => t != null);
            if (parentNode != null)
            {
                XmlNode newNode = parentNode.OwnerDocument.ImportNode(temp.DocumentElement.FirstChild, true);
                parentNode.ParentNode.InsertBefore(newNode, parentNode);
            }
        }

        public void ChangeValueOfNode(XPath xPath, string value)
        {
            if (XmlIsSigned)
                throw new InvalidOperationException("Document cannot be modified if signed");

            XmlNode node = _xmlDocument.SelectSingleNode(xPath.Expression, _xmlNamespaceManager);
            if (node != null)
            {
                node.InnerText = value;
            }
        }

        public void AddOrChangeValueOfAttribute(XAttribute attribute, string value)
        {
            // WARNING: Document shouldn't be modified afer signed
            if (XmlIsSigned)
                throw new InvalidOperationException("Document cannot be modified if signed");

            XmlNode node = _xmlDocument.SelectSingleNode(attribute.NodeXPath, _xmlNamespaceManager);
            if (node.Attributes[attribute.AttributeName] == null)
            {
                var attr = _xmlDocument.CreateAttribute(attribute.AttributeName);
                attr.InnerText = value;
                node.Attributes.Append(attr);
            }
            else
            {
                node.Attributes[attribute.AttributeName].Value = value;
            }
        }

        public void ChangeValueOfAttribute(XAttribute attribute, string value)
        {
            if (XmlIsSigned)
                throw new InvalidOperationException("Document cannot be modified if signed");

            XmlNode node = _xmlDocument.SelectSingleNode(attribute.NodeXPath, _xmlNamespaceManager);
            var attr = node.Attributes[attribute.AttributeName];
            if (attr != null)
            {
                attr.Value = value;
            }
        }
    }

    public class UTF8StringWriter : StringWriter
    {
        public UTF8StringWriter() { }
        public UTF8StringWriter(IFormatProvider formatProvider) : base(formatProvider) { }
        public UTF8StringWriter(StringBuilder sb) : base(sb) { }
        public UTF8StringWriter(StringBuilder sb, IFormatProvider formatProvider) : base(sb, formatProvider) { }

        public override Encoding Encoding { get => Encoding.UTF8; }
    }
}