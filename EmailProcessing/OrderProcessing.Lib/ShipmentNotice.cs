using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;




namespace OrderProcessing.Lib
{
    public class ShipmentNotice
    {
        private static readonly string[] delimiters = new string[] { "\r\n" };
        private static readonly Regex rxOrderNumber = new Regex(@"Order (\d+) \((.*)\).+\""(.+)\""", RegexOptions.Compiled);
        private static readonly Regex rxCurrency    = new Regex(@"\$(\d+(\.\d+)?)", RegexOptions.Compiled);
        private const string commentPreamble = "Please include the following comments to our customer: ";

        public static ShipmentNotice FromEmail(MailMessage email)
        {
            if (email is null)                            throw new ArgumentNullException(nameof(email));
            if (email.Subject != "Order Shipped")         throw new ArgumentException($"Wrong Email Type: {email.Subject}");
            if (rxCurrency.Matches(email.Body).Count < 5) throw new ArgumentException("Not enough currency symbols found.");

            Match match = rxOrderNumber.Match(email.Body);
            if (!match.Success) throw new ArgumentException($"Invalid email body");

            ShipmentNotice notice = new ShipmentNotice
            {
                OrderNumber      =  int.Parse(match.Groups[1].Value),
                CustomerName     =            match.Groups[2].Value,
                EmailTo          =            match.Groups[3].Value
            };

            string[] lines = email.Body.Split(delimiters, StringSplitOptions.None);
            for (int nLine = 0; nLine < lines.Length; nLine++)
            {
                if(lines[nLine] == notice.CustomerName)
                {
                    List<string> addressLines = new List<string>();
                    addressLines.AddRange(lines.Skip(nLine + 1).TakeWhile(l => l != ""));
                    notice.AddressLines = addressLines.AsReadOnly();
                    continue;
                }
                if(lines[nLine].StartsWith("Item Description"))
                {
                    List<OrderDetail> details = lines.Skip(nLine + 1).TakeWhile(l => l != "").Select(l => new OrderDetail(l)).ToList();
                    notice.OrderDetails = details.AsReadOnly();
                    continue;
                }
                if(lines[nLine].StartsWith("The tax"))
                {
                    MatchCollection matches = rxCurrency.Matches(lines[nLine]);
                    notice.Tax        = decimal.Parse(matches[0].Groups[1].Value);
                    notice.Shipping   = decimal.Parse(matches[1].Groups[1].Value);
                    notice.OrderTotal = decimal.Parse(matches[2].Groups[1].Value);
                    continue;
                }
                if (lines[nLine].StartsWith(commentPreamble))
                {
                    notice.CustomerComments = lines[nLine].Substring(commentPreamble.Length).Replace("\"", "");
                    break;
                }
            }
            return notice;
        }

        public int                              OrderNumber      { get; private set; }
        public string                           CustomerName     { get; private set; }
        public string                           EmailTo          { get; private set; }
        public IReadOnlyCollection<string>      AddressLines     { get; private set; }
        public IReadOnlyCollection<OrderDetail> OrderDetails     { get; private set; }
        public decimal                          Tax              { get; private set; }
        public decimal                          Shipping         { get; private set; }
        public decimal                          OrderTotal       { get; private set; }
        public string                           CustomerComments { get; private set; }

        
    }
}