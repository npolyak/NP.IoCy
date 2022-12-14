using NP.DependencyInjection.Attributes;
using TestAllRegisterAndResolveMethods.Interfaces;

namespace TestAllRegisterAndResolveMethods.Implementations
{
    [HasRegisterMethods]
    public static class FactoryMethods
    {
        [RegisterMethod(resolutionKey: "TheAddress", ResolvingType = typeof(IAddress))]
        public static IAddress CreateAddress()
        {
            return new Address();
        }
    }
}
