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

using NP.Utilities.Attributes;
using AssemblyLoadingTest.Interfaces;

namespace AssemblyLoadingTest.Implementations
{
    [Implements(typeof(IOrg))]
    public class Org : IOrg
    {
        public string OrgName { get; set; }

        [Part]
        public IPerson Manager { get; set; }

        [Part]
        public ILog Log { get; set; }

        public void LogOrgInfo()
        {
            Log.WriteLog($"OrgName: {OrgName}");
            Log.WriteLog($"Manager: {Manager.PersonName}");
            Log.WriteLog($"Manager's Address: {Manager.Address.City}, {Manager.Address.ZipCode}");
        }
    }
}
