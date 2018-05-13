using NP.IoCy.Attributes;
using BootstrappingTest.Interfaces;

namespace BootstrappingTest.Implementations
{
    public class Person : IPerson
    {
        public string PersonName { get; set; }

        [Part]
        public IAddress Address { get; set; }
    }
}
