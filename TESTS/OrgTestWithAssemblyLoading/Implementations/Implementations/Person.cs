using NP.IoCy.Attributes;
using OrgTestWithAssemblyLoading.Interfaces;

namespace OrgTestWithAssemblyLoading.Implementations
{
    [Implements]
    public class Person : IPerson
    {
        public string PersonName { get; set; }

        [Part]
        public IAddress Address { get; set; }
    }
}
