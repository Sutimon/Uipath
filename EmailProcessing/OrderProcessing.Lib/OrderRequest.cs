
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderProcessing.Lib
{
    public class OrderRequest
    {
        private static readonly Regex rxRequestInfo = new Regex(@":\t+(.*)\s",         RegexOptions.Compiled);
        private static readonly Regex rxProductInfo = new Regex(@"(.*)\t(.*)\t(.*)", RegexOptions.Compiled);
        private static readonly string[] delimiters = new string[] { "\r\n" };

        public static OrderRequest FromEmail(MailMessage email)
        {
            if (email is null) throw new ArgumentNullException(nameof(email));
            if (email.Subject != "Order Request") throw new ArgumentException($"Wrong Email Type: {email.Subject}");

            MatchCollection matches = rxRequestInfo.Matches(email.Body);
            OrderRequest request = new OrderRequest
            {

                Customer = matches[0].Groups[1].Value.Trim(),
                SalesRep = matches[1].Groups[1].Value.Trim(),
                OrderDate = DateTime.Parse(matches[2].Groups[1].Value.Trim()),
                RequiredDate = DateTime.Parse(matches[3].Groups[1].Value.Trim()),
                Shipper = matches[4].Groups[1].Value.Trim(),
                Freight = Decimal.Parse(matches[5].Groups[1].Value.Trim().Substring(1))
                
            };
            if (matches[6].Groups[1].Value.Trim() == "default")
            {
                request.ShipDate = DateTime.Today.AddDays(7);
            }

            string[] lines = email.Body.Split(delimiters, StringSplitOptions.None);
            for (int nLine = 0; nLine < lines.Length; nLine++)
            {
                if (lines[nLine].StartsWith("Product"))
                {
                    List<ProductDetail> details = lines.Skip(nLine + 1).TakeWhile(l => l != "").Select(l => new ProductDetail(l)).ToList();
                    request.ProductDetails = details.AsReadOnly();
                    continue;
                }

            }return request;
        }

        public string                             Customer        { get; private set; }
        public string                             SalesRep        { get; private set; }
        public DateTime                           OrderDate      { get; private set; }
        public DateTime                           RequiredDate   { get; private set; }
        public string                             Shipper        { get; private set; }
        public decimal                            Freight        { get; private set; }
        public DateTime                           ShipDate       { get; private set; }
        public IReadOnlyCollection<ProductDetail> ProductDetails { get; private set; }
        

       
    }

    public class ProductDetail
    {
        static readonly Regex rxProductDetal = new Regex(@"(.*)\s\((\d+)\)\t(\d+)\t(\d+(\.\d+)?)");

        internal ProductDetail(string detailLine)
        {
            Match match = rxProductDetal.Match(detailLine);

            ProductName = match.Groups[1].Value;
            ProductID = int.Parse(match.Groups[2].Value);
            Quantity = int.Parse(match.Groups[3].Value);
            Discount = decimal.Parse(match.Groups[4].Value);
        }

        public string ProductName { get; internal set; }
        public int ProductID { get; internal set; }
        public int Quantity { get; internal set; }
        public decimal Discount { get; internal set; }
    }

}
