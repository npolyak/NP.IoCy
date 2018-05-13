using NP.IoCy.Attributes;
using OrgTestWithAssemblyLoading.Interfaces;

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
