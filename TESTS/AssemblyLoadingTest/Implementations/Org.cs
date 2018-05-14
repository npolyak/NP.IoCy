using NP.IoCy.Attributes;
using OrganizationTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationTest.Implementations
{
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
