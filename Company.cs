using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raven_bootcamp
{
    public class Location
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Line1 { get; set; }
        public object Line2 { get; set; }
        public Location Location { get; set; }
        public string PostalCode { get; set; }
        public object Region { get; set; }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }

    public class Company
    {
        public Address Address { get; set; }
        public Contact Contact { get; set; }
        public string ExternalId { get; set; }
        public string Fax { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}