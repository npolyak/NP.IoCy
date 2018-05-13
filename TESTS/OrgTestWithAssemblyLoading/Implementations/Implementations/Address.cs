using NP.IoCy.Attributes;
using AssemblyLoadingTest.Interfaces;

namespace AssemblyLoadingTest.Implementations
{
    [Implements]
    public class Address : IAddress
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}
