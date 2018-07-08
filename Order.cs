using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raven_bootcamp
{
    public class Lines
    {
        public float Discount { get; set; }
        public float PricePerUnit { get; set; }
        public string Product { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class ShipTo
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Line1 { get; set; }
        public object Line2 { get; set; }
        public object Location { get; set; }
        public int PostalCode { get; set; }
        public object Region { get; set; }
    }

    public class Order
    {
        public string Company { get; set; }
        public string Employee { get; set; }
        public float Freight { get; set; }
        public List<Lines> Lines { get; set; }
        public DateTimeOffset OrderedAt { get; set; }
        public DateTimeOffset RequireAt { get; set; }
        public ShipTo ShipTo { get; set; }
        public string ShipVia { get; set; }
        public object ShippedAt { get; set; }
    }
}