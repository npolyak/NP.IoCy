using NP.IoCy.Attributes;
using MultyPartBootstrappingTest.Interfaces;
using System.Collections.Generic;

namespace MultyPartBootstrappingTest.Implementations
{
    public class Org : IOrg
    {
        public string OrgName { get; set; }

        [Part]
        public IPerson Manager { get; set; }

        [MultiPart]
        public IEnumerable<ILog> Logs { get; set; }

        public void LogOrgInfo()
        {
            foreach(ILog log in Logs)
            {
                log.WriteLog($"OrgName: {OrgName}");
                log.WriteLog($"Manager: {Manager.PersonName}");
                log.WriteLog($"Manager's Address: {Manager.Address.City}, {Manager.Address.ZipCode}");
            }
        }
    }
}
