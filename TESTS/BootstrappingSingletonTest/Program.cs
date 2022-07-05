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
            IoCContainer container = new IoCContainer();

            #region BOOTSTRAPPING
            // bootstrap container 
            // (map the types)
            container.MapSingleton<IPerson, Person>();
            container.MapSingleton<IAddress, Address>();
            //container.MapSingleton<IOrg, Org>();
            container.MapSingleton<ILog, ConsoleLog>();

            container.MapSingletonFactoryMethodInfo<IOrg>(typeof(Program).GetMethod(nameof(CreateOrg)));
            #endregion BOOTSTRAPPING

            // after CompleteConfiguration
            // you cannot bootstrap any new types in the container.
            // before CompleteConfiguration call
            // you cannot resolve container types. 
            container.CompleteConfiguration();

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

            // Create file MyLogFile.txt in the same folder as the executable
            // and write department store info in it;
            org.LogOrgInfo();


            Console.ReadKey();
        }
    }
}
