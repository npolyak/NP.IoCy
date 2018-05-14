using NP.IoCy.Attributes;
using MultyPartBootstrappingTest.Interfaces;

namespace MultyPartBootstrappingTest.Implementations
{
    public class Person : IPerson
    {
        public string PersonName { get; set; }

        [Part]
        public IAddress Address { get; set; }
    }
}
