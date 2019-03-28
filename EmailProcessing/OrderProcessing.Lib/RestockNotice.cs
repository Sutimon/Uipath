using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderProcessing.Lib
{
    public class RestockNotice
    {
        private static readonly Regex myRegEx = new Regex(@"(.*)\t(.*)\t(\d+)", RegexOptions.Compiled);
        private static readonly Regex myRegEx2 = new Regex(@"supplier (.+)\:", RegexOptions.Compiled);
        private static readonly char[] delimiters = new char[] { '\t' };

        public static RestockNotice FromEmail(MailMessage email)
        {
            if (email is null) throw new ArgumentNullException(nameof(email));
            if (email.Subject != "Shipment Received")
                throw new ArgumentNullException($"Wrong email type: {email.Subject}");

            MatchCollection myCollection = myRegEx.Matches(email.Body);
            Match myCollection2 = myRegEx2.Match(email.Body);

            RestockNotice notice = new RestockNotice
            {
                Supplier = myCollection2.Groups[1].Value
            };

            MatchCollection myMatchCollection = myRegEx.Matches(email.Body);
            List<RestockDetails> RestockDetailsList = new List<RestockDetails>();

            foreach (Match a in myMatchCollection)
            {
                RestockDetails myDetails = new RestockDetails(a.Value);
                RestockDetailsList.Add(myDetails);
            }
            notice.RestockDetails = RestockDetailsList;
            return notice;
        }

        public int ProductID { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public string Supplier { get; private set; }
        public IReadOnlyCollection<RestockDetails> RestockDetails { get; private set; }
    }
}
namespace OrderProcessing.Lib
{
    public class RestockDetails
    {
        internal RestockDetails(string detailLine)
        {
            string[] parts = detailLine.Split('\t');
            ProductID = int.Parse(parts[0]);
            ProductName = parts[1];
            Quantity = int.Parse(parts[2]);
        }

        public int ProductID { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
    }
}