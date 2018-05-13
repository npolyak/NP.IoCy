using NP.IoCy.Attributes;
using OrgTestWithAssemblyLoading.Interfaces;

namespace OrgTestWithAssemblyLoading.Implementations
{
    [Implements]
    public class Address : IAddress
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}
