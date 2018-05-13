using NP.IoCy.Attributes;
using OrganizationTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationTest.Implementations
{
    public class Person : IPerson
    {
        public string PersonName { get; set; }

        [Part]
        public IAddress Address { get; set; }
    }
}
