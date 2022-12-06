﻿using NP.DependencyInjection.Attributes;
using TestAllRegisterAndResolveMethods.Interfaces;

namespace TestAllRegisterAndResolveMethods.Implementations
{

    [RegisterType(resolutionKey:"AnotherPerson")]
    public class AnotherPerson : IPersonGettersOnly
    {
        [Inject]
        public IAddress Address { get; }

        [CompositeConstructor]
        public AnotherPerson([Inject] IAddress address)
        {
            this.Address = address;
        }
    }
}
