using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserCSV.Models
{
    public class SaleRecord
    {
        public System.Guid Id { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string ItemType { get; set; }
        public string SalesChannel { get; set; }
        public string OrderPriority { get; set; }
        public string OrderDate { get; set; }
        public string OrderID { get; set; }
        public DateTime ShipDate { get; set; }
        public Nullable<int> UnitsSold { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<double> UnitCost { get; set; }
        public Nullable<double> TotalRevenue { get; set; }
        public Nullable<double> TotalCost { get; set; }
        public Nullable<double> TotalProfit { get; set; }
    }
}