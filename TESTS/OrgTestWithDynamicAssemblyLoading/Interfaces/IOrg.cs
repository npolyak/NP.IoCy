using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationTest.Interfaces
{
    public interface IOrg
    {
        string OrgName { get; set; }

        IPerson Manager { get; set; }

        ILog Log { get; set; }

        void LogOrgInfo();
    }
}
