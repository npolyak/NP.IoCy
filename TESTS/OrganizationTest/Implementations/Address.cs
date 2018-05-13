using OrganizationTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationTest.Implementations
{
    public class Address : IAddress
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}
