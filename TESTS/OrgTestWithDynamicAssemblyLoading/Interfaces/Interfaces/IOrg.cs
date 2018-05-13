namespace DynamicAssemblyLoadingTest.Interfaces
{
    public interface IOrg
    {
        string OrgName { get; set; }

        IPerson Manager { get; set; }

        ILog Log { get; set; }

        void LogOrgInfo();
    }
}
