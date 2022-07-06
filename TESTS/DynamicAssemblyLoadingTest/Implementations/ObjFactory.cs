using DynamicAssemblyLoadingTest.Implementations;
using DynamicAssemblyLoadingTest.Interfaces;
using NP.Utilities.Attributes;

namespace Implementations
{
    [HasFactoryMethods]
    public static class ObjFactory
    {
        [FactoryMethod(isSingleton:true)]
        public static IOrg CreateOrg([Part] ILog log)
        {
            return new Org(log);
        }
    }
}
