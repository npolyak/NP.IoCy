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

using NP.DependencyInjection.Attributes;
using System.Collections.Generic;
using TestAllRegisterAndResolveMethods.Interfaces;

namespace TestAllRegisterAndResolveMethods.Implementations
{
    public class Org : IOrg
    {
        public string OrgName { get; set; }

        [Inject]
        public IPerson Manager { get; set; }

        public IPerson ProjLead { get; set; }

        [Inject]
        public ILog Log { get; set; }

        [Inject(resolutionKey:"MyLog")]
        public ILog Log2 { get; set; }

        public void LogOrgInfo()
        {
            Log.WriteLog($"OrgName: {OrgName}");
            Log.WriteLog($"Manager: {Manager.PersonName}");
            Log.WriteLog($"Manager's Address: {Manager.Address.City}, {Manager.Address.ZipCode}");
        }
    }

    [RegisterMultiCellType(cellType: typeof(IOrg), "TheOrgs")]
    public class MyOrg : Org
    {
        public MyOrg()
        {
            OrgName = "MyOrg1";
        }
    }

    [HasRegisterMethods]
    public static class OrgFactory
    {
        [RegisterMultiCellMethod(typeof(IOrg), "TheOrgs")]
        public static IOrg CreateSingleOrg()
        {
            return new MyOrg { OrgName = "MyOrg2" };
        }

        [RegisterMultiCellMethod(typeof(IOrg), "TheOrgs")]
        public static IEnumerable<IOrg> CreateOrgs()
        {
            return new IOrg[]
            {
                new MyOrg { OrgName = "MyOrg3" },
                new MyOrg { OrgName = "MyOrg4" }
            };
        }
    }


    public class OrgsContainer
    {
        public IEnumerable<IOrg> Orgs { get; }

        [CompositeConstructor]
        public OrgsContainer([Inject(resolutionKey:"TheOrgs")] IEnumerable<IOrg> orgs)
        {
            Orgs = orgs;
        }
    }
}
