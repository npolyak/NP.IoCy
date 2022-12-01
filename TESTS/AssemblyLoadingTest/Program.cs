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
using AssemblyLoadingTest.Interfaces;
using System;

namespace AssemblyLoadingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // create container
            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterAssembly(typeof(AssemblyLoadingTest.Implementations.Org).Assembly);

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

            // Create file MyLogFile.txt in the same folder as the executable
            // and write department store info in it;
            org.LogOrgInfo();


            // replace mapping to ILog to ConsoleLog in the new container. 

            containerBuilder.Register<ILog, ConsoleLog>();
            var anotherContainer = containerBuilder.Build();

            // resolve org from the anotherContainer.
            IOrg orgWithConsoleLog = anotherContainer.Resolve<IOrg>();


            #region Set Child Org Data

            orgWithConsoleLog.OrgName = "Nicks Department Store";
            orgWithConsoleLog.Manager.PersonName = "Nick Polyak";
            orgWithConsoleLog.Manager.Address.City = "Miami";
            orgWithConsoleLog.Manager.Address.ZipCode = "33162";

            #endregion Set Child Org Data

            // send org data to console instead of a file.
            orgWithConsoleLog.LogOrgInfo();

            Console.ReadKey();
        }
    }
}
