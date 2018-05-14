using System.Collections.Generic;

namespace MultyPartBootstrappingTest.Interfaces
{
    public interface IOrg
    {
        string OrgName { get; set; }

        IPerson Manager { get; set; }

        IEnumerable<ILog> Logs { get; set; }

        void LogOrgInfo();
    }
}
