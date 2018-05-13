using NP.IoCy.Attributes;
using OrgTestWithDynamicAssemblyLoading.Interfaces;

namespace OrgTestWithDynamicAssemblyLoading.Implementations
{
    [Implements]
    public class Address : IAddress
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}
