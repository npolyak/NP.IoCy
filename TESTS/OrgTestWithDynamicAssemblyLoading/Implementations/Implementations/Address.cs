using NP.IoCy.Attributes;
using DynamicAssemblyLoadingTest.Interfaces;

namespace DynamicAssemblyLoadingTest.Implementations
{
    [Implements]
    public class Address : IAddress
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}
