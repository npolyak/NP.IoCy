// (c) Nick Polyak 2018 - http://awebpros.com/
// License: Apache License 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using NP.IoCy;
using BootstrappingSingletonTest.Implementations;
using BootstrappingSingletonTest.Interfaces;
using System;

namespace BootstrappingSingletonTest
{
    class Program
    {
        public static IOrg CreateOrg(ILog log)
        {
            return new Org(log);
        }

        static void Main(string[] args)
        {
            // create container
            ContainerBuilder containerBuilder = new ContainerBuilder();

            #region BOOTSTRAPPING
            // bootstrap container builder
            // (register the types)
            containerBuilder.RegisterSingleton<IPerson, Person>();
            containerBuilder.RegisterSingleton<IAddress, Address>();
            //container.RegisterSingleton<IOrg, Org>();
            containerBuilder.RegisterSingleton<ILog, ConsoleLog>();

            // use a factory method Program.CreateOrg to create IOrg object
            containerBuilder.RegisterSingletonFactoryMethodInfo<IOrg>(typeof(Program).GetMethod(nameof(CreateOrg)));
            #endregion BOOTSTRAPPING

            // create container
            Container container = containerBuilder.Build();

            // resolve and compose organization
            // all its 'Parts' will be added at
            // this stage. 
            IOrg org = container.Resolve<IOrg>();


            #region Set Org Data

            org.OrgName = "Nicks Department Store";
            org.Manager.PersonName = "Nick Polyak";
            org.Manager.Address.City = "Miami";
            org.Manager.Address.ZipCode = "33162";

            #endregion Set Org Data

            // use the propper Logger to log the message (currently it is ConsoleLog so the message will be 
            // printed to console.
            org.LogOrgInfo();


            Console.ReadKey();
        }
    }
}
