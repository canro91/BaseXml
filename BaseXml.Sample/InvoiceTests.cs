using NUnit.Framework;
using System.Collections.Generic;

namespace BaseXml.Sample
{
    [TestFixture]
    public class InvoiceTests
    {
        [Test]
        public void ModifyInvoice_AnIncompleteInvoice_AddsAndEvaluatesNode()
        {
            var xml = @"<Invoice>
  <Billing Number=""1000"" Date=""2015-04-15"" Total=""1,000,000.00"" />
  <Customer Name=""James Rhodey Rhodes"">
    <Address AddressLine=""1-23 Main St."" City=""Los Angeles"" />
  </Customer>
  <Products>
    <Product Code=""WAR-MACHINE-2"" Description=""IRON PATRIOT"" Quantity=""1"" UnitPrice=""1,000,000.00"" Total=""1,000,000.00"" />
  </Products>
</Invoice>";
            var invoice = new Invoice(xml);

            invoice.AddSupplier(contact: "Pepper Potts", seller: "Tony Starks");

            var contact = invoice.Contact;
            Assert.AreEqual("Pepper Potts", contact);

            var seller = invoice.Seller;
            Assert.AreEqual("Tony Starks", seller);
        }
    }

    class Invoice : BaseDocument
    {
        public Invoice(string xml)
            : base(xml)
        {
        }

        public override IEnumerable<XsdReference> UblXsds
            => new XsdReference[0];

        public override IEnumerable<XmlNamespace> XmlNamespaces
            => new XmlNamespace[0];

        public override bool XmlIsSigned
            => false;

        public string Contact
            => Evaluate(new XPath("/Invoice/Supplier/@Contact"));

        public string Seller
            => Evaluate(new XPath("/Invoice/Supplier/@Seller"));

        public void AddSupplier(string contact, string seller)
        {
            var node = $@"<Supplier Contact=""{contact}"" Seller=""{seller}"" />";
            AddSiblingNodeAfterFirstOf(node, new XPath("/Invoice/Customer"));
        }
    }
}
