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
using System.IO;

using ILog = DynamicAssemblyLoadingTest.Interfaces.ILog;

namespace DynamicAssemblyLoadingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // create container
            IoCContainer container = new IoCContainer();

            container
                .InjectDynamicAssemblyByFullPath
                (
                    Path.Combine
                    (
                        ReflectionUtils.GetCurrentExecutablePath(), 
                        "Plugins\\DynamicAssemblyLoadingTest.Implementations.dll"));

            // after CompleteConfiguration
            // you cannot bootstrap any new types in the container.
            // before CompleteConfiguration call
            // you cannot resolve container types. 
            container.CompleteConfiguration();

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

            // Create file MyLogFile.txt in the same folder as the executable
            // and write department store info in it;
            org.LogOrgInfo();


            //Console.ReadKey();
        }
    }
}
