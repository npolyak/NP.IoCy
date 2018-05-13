using NP.IoCy.Attributes;
using DynamicAssemblyLoadingTest.Interfaces;

namespace DynamicAssemblyLoadingTest.Implementations
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
