using DynamicAssemblyLoadingTest.Implementations;
using DynamicAssemblyLoadingTest.Interfaces;
using NP.Utilities.Attributes;

namespace Implementations
{
    [HasFactoryMethods]
    public static class ObjFactory
    {
        [FactoryMethod(isSingleton:true, partKey:"TheOrg")]
        public static IOrg CreateOrg([Part(partKey: "TheConsoleLog")] ILog log)
        {
            return new Org(log);
        }
    }
}
