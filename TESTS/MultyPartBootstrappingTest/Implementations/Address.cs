using MultyPartBootstrappingTest.Interfaces;

namespace MultyPartBootstrappingTest.Implementations
{
    public class Address : IAddress
    {
        public string City { get; set; }

        public string ZipCode { get; set; }
    }
}
