using NP.DependencyInjection.Attributes;

namespace TestAllRegisterAndResolveMethods.Interfaces
{
    public interface IPersonGettersOnly
    {
        IAddress Address { get; }
    }
}
