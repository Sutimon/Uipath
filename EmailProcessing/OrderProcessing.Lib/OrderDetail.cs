using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Lib
{
    public class OrderDetail
    {
        internal OrderDetail(string detailLine)
        {
            string[] parts = detailLine.Split('\t');
            Description = parts[0];
            Quantity = int.Parse(parts[1]);
            UnitPrice = decimal.Parse(parts[2].Substring(1));
            Total = decimal.Parse(parts[3].Substring(1));
        }

        public string Description { get; internal set; }
        public int Quantity { get; internal set; }
        public decimal UnitPrice { get; internal set; }
        public decimal Total { get; internal set; }
    }
}
