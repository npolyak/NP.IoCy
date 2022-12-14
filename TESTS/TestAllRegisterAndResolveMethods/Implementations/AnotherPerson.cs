using NP.DependencyInjection.Attributes;
using TestAllRegisterAndResolveMethods.Interfaces;

namespace TestAllRegisterAndResolveMethods.Implementations
{

    [RegisterType(resolutionKey: "AnotherPerson", IsSingleton = true)]
    public class AnotherPerson : IPersonGettersOnly
    {
        public string PersonName { get; set; }

        public IAddress Address { get; }

        [CompositeConstructor]
        public AnotherPerson([Inject(resolutionKey: "TheAddress")] IAddress address)
        {
            this.Address = address;
        }
    }
}
