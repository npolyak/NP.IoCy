using NP.IoCy.Attributes;
using OrgTestWithDynamicAssemblyLoading.Interfaces;

namespace OrgTestWithDynamicAssemblyLoading.Implementations
{
    [Implements]
    public class Person : IPerson
    {
        public string PersonName { get; set; }

        [Part]
        public IAddress Address { get; set; }
    }
}
