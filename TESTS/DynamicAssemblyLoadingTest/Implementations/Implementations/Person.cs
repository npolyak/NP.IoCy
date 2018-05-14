using NP.IoCy.Attributes;
using DynamicAssemblyLoadingTest.Interfaces;

namespace DynamicAssemblyLoadingTest.Implementations
{
    [Implements]
    public class Person : IPerson
    {
        public string PersonName { get; set; }

        [Part]
        public IAddress Address { get; set; }
    }
}
