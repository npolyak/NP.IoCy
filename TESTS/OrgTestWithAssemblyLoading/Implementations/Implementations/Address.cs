using NP.IoCy.Attributes;
using OrgTestWithAssemblyLoading.Interfaces;

namespace AssemblyLoadingTest.Implementations
{
    [Implements]
    public class Address : IAddress
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}
