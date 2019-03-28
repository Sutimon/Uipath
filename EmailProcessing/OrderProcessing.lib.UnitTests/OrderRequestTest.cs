using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProcessing.Lib;
using static OrderProcessing.Lib.OrderRequest;

namespace OrderProcessing.lib.UnitTests
{
    [TestClass]
    public class OrderRequestTest
    {
        [TestMethod]
        public void FromEmail()
        {
            string emailBody = File.ReadAllText("OrderRequests\\Order Request-Antonio Moreno Taquería.txt");
            MailMessage email = new MailMessage
            {
                Subject = "Order Request",
                Body = emailBody
            };
            OrderRequest request = OrderRequest.FromEmail(email);
            DateTime date = new DateTime(2018, 12, 04);
            DateTime date2 = new DateTime(2018, 12, 11);
            DateTime date3 = DateTime.Today.AddDays(7);

            Assert.AreEqual("Antonio Moreno Taquería (ANTON)", request.Customer);
            Assert.AreEqual("Michael Suyama (6)", request.SalesRep);
            Assert.AreEqual(date, request.OrderDate);
            Assert.AreEqual(date2, request.RequiredDate);
            Assert.AreEqual("Speedy Express (1)", request.Shipper);
            Assert.AreEqual(1554.98m, request.Freight);
            Assert.AreEqual(date3, request.ShipDate);
            Assert.AreEqual(8, request.ProductDetails.Count);

            ProductDetail product = request.ProductDetails.Last();

            Assert.AreEqual("Röd Kaviar", product.ProductName);
            Assert.AreEqual(73m, product.ProductID);
            Assert.AreEqual(30m, product.Quantity);
            Assert.AreEqual(0.03m, product.Discount);

        }
        [TestMethod]
        public void TestAllEmails()
        {
            foreach (string fpath in Directory.GetFiles("OrderRequests", "*.txt"))
            {
                Console.WriteLine(fpath);
                MailMessage msg = new MailMessage
                {
                    Subject = "Order Request",
                    Body = File.ReadAllText(fpath)
                };
                OrderRequest request = OrderRequest.FromEmail(msg);
                

                Assert.IsFalse(string.IsNullOrEmpty(request.Customer));
                Assert.IsFalse(string.IsNullOrEmpty(request.SalesRep));
                Assert.IsNotNull(request.OrderDate);
                Assert.IsNotNull(request.RequiredDate);
                Assert.IsFalse(string.IsNullOrEmpty(request.Shipper)); ;
                Assert.IsTrue(request.Freight > 0);


                Assert.IsTrue(request.ProductDetails.All(p => !string.IsNullOrEmpty(p.ProductName)));
                Assert.IsTrue(request.ProductDetails.All(p => p.Discount >= 0));
                Assert.IsTrue(request.ProductDetails.All(p => p.ProductID > 0));
                Assert.IsTrue(request.ProductDetails.All(p => p.Quantity > 0));
               
               
            }
        }
    }
}
