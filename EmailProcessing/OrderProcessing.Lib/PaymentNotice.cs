using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderProcessing.Lib
{
    public class PaymentReceived
    {
        private static readonly Regex myRegEx = new Regex(@"\$(.+\.\d{2})", RegexOptions.Compiled);
        private static readonly Regex myRegEx2 = new Regex(@"order (\d+)", RegexOptions.Compiled);
        private static readonly Regex myRegEx3 = new Regex(@"on (\d+\-\d+\-\d+)", RegexOptions.Compiled);
        private static readonly Regex myRegEx4 = new Regex(@"is (\d+)", RegexOptions.Compiled);
        private static readonly char[] delimiters = new char[] { '-' };

        public static PaymentReceived FromEmail(MailMessage email)
        {
            if (email is null) throw new ArgumentNullException(nameof(email));
            if (email.Subject != "Payment Received")
                throw new ArgumentNullException($"Wrong email typ: {email.Subject}");

            Match myCollection = myRegEx.Match(email.Body);
            Match myCollection2 = myRegEx2.Match(email.Body);
            Match myCollection3 = myRegEx3.Match(email.Body);
            Match myCollection4 = myRegEx4.Match(email.Body);

            PaymentReceived notice = new PaymentReceived
            {
                Amount = decimal.Parse(myCollection.Groups[1].Value),
                OrderNumber = int.Parse(myCollection2.Groups[1].Value),
                CheckNumber = decimal.Parse(myCollection4.Groups[1].Value)
            };

            string[] myString = myCollection3.Groups[1].Value.Split(delimiters);
            DateTime myDateTime = new DateTime(int.Parse(myString[2]), int.Parse(myString[0]), int.Parse(myString[1]));
            notice.OrderDate = myDateTime;

            return notice;
        }

        public decimal Amount { get; private set; } 
        public int OrderNumber { get; private set; }
        public DateTime OrderDate { get; private set; }
        public decimal CheckNumber { get; private set; }
    }
}