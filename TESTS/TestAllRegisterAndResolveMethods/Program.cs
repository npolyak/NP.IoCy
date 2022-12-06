﻿// (c) Nick Polyak 2018 - http://awebpros.com/
// License: Apache License 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using FluentAssertions;
using NP.IoCy;
using TestAllRegisterAndResolveMethods.Implementations;
using TestAllRegisterAndResolveMethods.Interfaces;
using System;
using NP.DependencyInjection.Interfaces;
using FluentAssertions.Equivalency;
using System.Reflection;

namespace TestAllRegisterAndResolveMethods
{
    static class Program
    {
        public static bool IsSingleton<T>
        (
            this IDependencyInjectionContainer container, 
            object key = null
        )
        {
            T obj1 = container.Resolve<T>(key);

            obj1.Should().NotBeNull();

            T obj2 = container.Resolve<T>(key);
            obj2.Should().NotBeNull();

            return object.ReferenceEquals(obj1, obj2);
        }

        public static IOrg CreateOrg()
        {
            IOrg org = new Org();

            org.Manager = new Person();

            org.OrgName = "Other Department Store";
            org.Manager.PersonName = "Joe Doe";

            org.Manager.Address = new Address();
            org.Manager.Address.City = "Boston";
            org.Manager.Address.ZipCode = "12345";

            org.Log = new ConsoleLog();

            return org;
        }

        static void Main(string[] args)
        {
            // create container builder
            IContainerBuilder containerBuilder = new ContainerBuilder();

            #region BOOTSTRAPPING
            // bootstrap container 
            // (map the types)
            containerBuilder.RegisterType<IPerson, Person>();
            containerBuilder.RegisterType<IAddress, Address>();
            containerBuilder.RegisterType<IOrg, Org>();
            containerBuilder.RegisterSingletonType<ILog, FileLog>();

            containerBuilder.RegisterType<ILog, ConsoleLog>("MyLog");
            #endregion BOOTSTRAPPING

            // Create container
            IDependencyInjectionContainer container = containerBuilder.Build();

            // resolve and compose organization
            // all its 'Parts' will be added at
            // this stage. 
            IOrg org = container.Resolve<IOrg>();

            #region Set Org Data

            org.ProjLead.Should().BeNull();

            org.Manager.Should().NotBeNull();

            IPerson person = container.Resolve<IPerson>();  

            person.Should().NotBeSameAs(org.Manager);

            ILog log = container.Resolve<ILog>();

            log.Should().BeSameAs(org.Log);

            org.Log2.Should().NotBeNull();
            org.Log2.Should().BeOfType<ConsoleLog>();

            ILog log2 = container.Resolve<ILog>("MyLog");

            log2.Should().NotBeNull();

            log2.Should().NotBeSameAs(org.Log2);
            log2.Should().BeOfType<ConsoleLog>();

            org.OrgName = "Nicks Department Store";
            org.Manager.PersonName = "Nick Polyak";
            org.Manager.Address.City = "Miami";
            org.Manager.Address.ZipCode = "12345";

            #endregion Set Org Data

            // Create file MyLogFile.txt in the same folder as the executable
            // and write department store info in it;
            org.LogOrgInfo();

            ConsoleLog consoleLog = new ConsoleLog();
            containerBuilder.RegisterSingletonInstance<ILog>(consoleLog);

            // replace registration of ILog to ConsoleLog (instead of FileLog) in another container. 
            IDependencyInjectionContainer anotherContainer = containerBuilder.Build();

            // resolve org from another Container.
            IOrg orgWithConsoleLog = anotherContainer.Resolve<IOrg>();

            orgWithConsoleLog.Log.Should().NotBeNull();
            orgWithConsoleLog.Log.Should().BeSameAs(consoleLog);

            #region Set Child Org Data

            orgWithConsoleLog.OrgName = "Nicks Department Store";
            orgWithConsoleLog.Manager.PersonName = "Nick Polyak";
            orgWithConsoleLog.Manager.Address.City = "Miami";
            orgWithConsoleLog.Manager.Address.ZipCode = "12345";

            #endregion Set Child Org Data

            // send org data to console instead of a file.
            orgWithConsoleLog.LogOrgInfo();

            containerBuilder.RegisterFactoryMethod(CreateOrg);
            IDependencyInjectionContainer container3 = containerBuilder.Build();
            container3.TestOrg(false);


            containerBuilder.RegisterFactoryMethod(CreateOrg, "TheOrg");
            container3 = containerBuilder.Build();
            container3.TestOrg(false, "TheOrg");

            containerBuilder.RegisterSingletonFactoryMethod(CreateOrg);
            container3 = containerBuilder.Build();
            container3.TestOrg(true);

            containerBuilder.RegisterSingletonFactoryMethod(CreateOrg, "TheOrg");
            container3 = containerBuilder.Build();
            container3.TestOrg(true, "TheOrg");


            containerBuilder.UnRegister(typeof(IOrg));
            containerBuilder.UnRegister(typeof(IOrg), "TheOrg");
            container3 = containerBuilder.Build();

            org = container3.Resolve<IOrg>();
            org.Should().BeNull();
            org = container3.Resolve<IOrg>("TheOrg");
            org.Should().BeNull();

            MethodInfo createOrgMethodInfo =
                typeof(Program).GetMethod(nameof(CreateOrg));

            containerBuilder.RegisterSingletonFactoryMethodInfo<IOrg>(createOrgMethodInfo, "TheOrg");
            container3 = containerBuilder.Build();
            container3.TestOrg(true, "TheOrg");

            containerBuilder.RegisterFactoryMethodInfo(createOrgMethodInfo, null, "TheOrg");
            container3 = containerBuilder.Build();
            container3.TestOrg(false, "TheOrg");

            IContainerBuilder containerBuilder4 = new ContainerBuilder();

            containerBuilder4.RegisterAttributedType(typeof(AnotherOrg));
            containerBuilder4.RegisterAttributedType(typeof(AnotherPerson));
            containerBuilder4.RegisterAttributedType(typeof(ConsoleLog));
            containerBuilder4.RegisterType<IAddress, Address>();

            var container4 = containerBuilder4.Build();

            IOrgGettersOnly orgGettersOnly = 
                container4.Resolve<IOrgGettersOnly>();

            orgGettersOnly.Manager.Address.Should().NotBeNull();

            // make sure ILog is a singleton.
            container4.IsSingleton<ILog>().Should().BeTrue();

            Console.ReadKey();
        }

        public static void TestOrg(this IDependencyInjectionContainer container, bool isSingleton, object key = null)
        {
            container.IsSingleton<IOrg>(key).Should().Be(isSingleton);
            IOrg org = container.Resolve<IOrg>(key);
            org.OrgName.Should().Be("Other Department Store");

        }

    }
}