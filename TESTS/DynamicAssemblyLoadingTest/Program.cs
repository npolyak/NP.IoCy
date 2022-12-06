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
using NP.Utilities;
using DynamicAssemblyLoadingTest.Interfaces;
using System;
// (c) Nick Polyak 2022 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using System.IO;

using ILog = DynamicAssemblyLoadingTest.Interfaces.ILog;
using NP.DependencyInjection.Interfaces;

namespace DynamicAssemblyLoadingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // create container builder
            IContainerBuilder builder = new ContainerBuilder();

            builder
                .RegisterDynamicAssemblyByFullPath
                (
                    Path.Combine
                    (
                        ReflectionUtils.GetCurrentExecutablePath(), 
                        "Plugins\\DynamicAssemblyLoadingTest.Implementations.dll"));

            // create container
            IDependencyInjectionContainer container = builder.Build ();

            // resolve and compose organization
            // all its 'Parts' will be added at
            // this stage. 
            IOrg org = container.Resolve<IOrg>("TheOrg");


            #region Set Org Data

            org.OrgName = "Nicks Department Store";
            org.Manager.PersonName = "Nick Polyak";
            org.Manager.Address.City = "Miami";
            org.Manager.Address.ZipCode = "33162";

            #endregion Set Org Data

            //use the propper logger to print the org info (Console log in our case).
            org.LogOrgInfo();


            //Console.ReadKey();
        }
    }
}
