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

using NP.DependencyInjection.Attributes;
using DynamicAssemblyLoadingTest.Interfaces;

namespace DynamicAssemblyLoadingTest.Implementations
{
    [RegisterType(typeof(IOrg))]
    public class Org : IOrg
    {
        public string OrgName { get; set; }

        [Inject]
        public IPerson Manager { get; set; }

        [Inject(resolutionKey: "TheConsoleLog")]
        public ILog Log { get; private set; }

        public void LogOrgInfo()
        {
            Log.WriteLog($"OrgName: {OrgName}");
            Log.WriteLog($"Manager: {Manager.PersonName}");
            Log.WriteLog($"Manager's Address: {Manager.Address.City}, {Manager.Address.ZipCode}");
        }

        [CompositeConstructor]
        public Org([Inject(resolutionKey: "TheConsoleLog")]ILog log)
        {
            Log = log;
        }
    }
}
