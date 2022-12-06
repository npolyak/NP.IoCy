using NP.DependencyInjection.Attributes;
using TestAllRegisterAndResolveMethods.Interfaces;

namespace TestAllRegisterAndResolveMethods.Implementations
{
    [RegisterType()]
    public class AnotherOrg : IOrgGettersOnly
    {
        public IPersonGettersOnly Manager { get; }

        public ILog Log { get; }

        [CompositeConstructor]
        public AnotherOrg([Inject(resolutionKey:"AnotherPerson")] IPersonGettersOnly manager, [Inject]ILog log)
        {
            Manager = manager;
            Log = log;
        }
    }
}
