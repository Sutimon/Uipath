using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SendMail
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter O (Order Request), P (Payment Received), R (Restock Notices), S (Shipment Notices)");
            string input = Console.ReadLine().ToUpper();
            string folder = null;
            string subject = null;
            switch(input)
            {
                case "O":
                    folder = "OrderRequests";
                    subject = "Order Request";
                    break;
                case "P":
                    folder = "PaymentReceived";
                    subject = "Payment Received";
                    break;
                case "R":
                    folder = "RestockNotices";
                    subject = "Shipment Received";
                    break;
                case "S":
                    folder = "ShipmentNotices";
                    subject = "Order Shipped";
                    break;
                default: Console.WriteLine("Noting to do! Bye."); return;
            }
            foreach (string fpath in Directory.GetFiles(folder))
                SendMessage(fpath, subject);
        }

        static void SendMessage(string fileName, string subject)
        {
            string body = File.ReadAllText(fileName);
            MailMessage msg = new MailMessage("nwadmin@platformbronx.com", "rpa11@platformbronx.com", subject, body);
            SmtpClient client = new SmtpClient("10.0.0.35");
            client.Send(msg);
        }
    }
}

