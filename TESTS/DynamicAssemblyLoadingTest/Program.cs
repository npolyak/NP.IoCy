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
using DynamicAssemblyLoadingTest.Interfaces;
using System.IO;
using System.Collections.Generic;
using System;

namespace DynamicAssemblyLoadingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // create container builder
            var builder = new ContainerBuilder<string>();

            builder
                .RegisterDynamicAssemblyByFullPath
                (
                    Path.Combine
                    (
                        System.AppDomain.CurrentDomain.BaseDirectory, 
                        "Plugins\\DynamicAssemblyLoadingTest.Implementations.dll"));

            // create container
            var container = builder.Build ();

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

            IEnumerable<string> strs = container.Resolve<IEnumerable<string>>("MyStrs");

            foreach(string str in strs)
            {
                Console.WriteLine(str);
            }

            Console.ReadKey();
        }
    }
}
