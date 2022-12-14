using NP.DependencyInjection.Attributes;

namespace TestAllRegisterAndResolveMethods.Interfaces
{
    public interface IPersonGettersOnly
    {
        string PersonName { get; set; }

        IAddress Address { get; }
    }
}
