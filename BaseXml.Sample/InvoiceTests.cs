using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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

        [Test]
        public void Invoice_MultipleProducts_EvaluateLines()
        {
            var xml = @"<Invoice>
  <Billing Number=""1000"" Date=""2015-04-15"" Total=""2,000,000.00"" />
  <Customer Name=""James Rhodey Rhodes"">
    <Address AddressLine=""1-23 Main St."" City=""Los Angeles"" />
  </Customer>
  <Products>
    <Product Code=""WAR-MACHINE-2"" Description=""IRON PATRIOT"" Quantity=""1"" UnitPrice=""1,000,000.00"" Total=""1,000,000.00"" />
    <Product Code=""MARK VII"" Description=""Avengers"" Quantity=""1"" UnitPrice=""1,000,000.00"" Total=""1,000,000.00"" />
  </Products>
</Invoice>";
            var invoice = new Invoice(xml);

            var products = invoice.Products.ToList();

            Assert.AreEqual(2, products.Count);

            var product1 = products[0];
            Assert.AreEqual("WAR-MACHINE-2", product1.Code);
            Assert.AreEqual("1,000,000.00", product1.Price);

            var product2 = products[1];
            Assert.AreEqual("MARK VII", product2.Code);
            Assert.AreEqual("1,000,000.00", product2.Price);
        }
    }

    class Invoice : BaseDocument
    {
        public Invoice(string xml)
            : base(xml)
        {
        }

        public string Contact
            => Evaluate(new XPath("/Invoice/Supplier/@Contact"));

        public string Seller
            => Evaluate(new XPath("/Invoice/Supplier/@Seller"));

        public IEnumerable<Product> Products
        {
            get
            {
                var products = new List<Product>();

                var rawProducts = _xmlDocument.SelectNodes("/Invoice/Products/Product", _xmlNamespaceManager);
                foreach (XmlNode node in rawProducts)
                {
                    var product = new Product
                    {
                        Code = node.SelectSingleNode("@Code", _xmlNamespaceManager)?.InnerText,
                        Price = node.SelectSingleNode("@UnitPrice", _xmlNamespaceManager)?.InnerText
                    };
                    products.Add(product);
                }

                return products;
            }
        }

        public void AddSupplier(string contact, string seller)
        {
            var node = $@"<Supplier Contact=""{contact}"" Seller=""{seller}"" />";
            AddSiblingNodeAfterFirstOf(node, new XPath("/Invoice/Customer"));
        }
    }

    public class Product
    {
        public string Code { get; set; }
        public string Price { get; set; }
    }
}
