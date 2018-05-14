using NP.IoCy;
using MultyPartBootstrappingTest.Implementations;
using MultyPartBootstrappingTest.Interfaces;

namespace MultyPartBootstrappingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // create container
            IoCContainer container = new IoCContainer();

            #region BOOTSTRAPPING
            // bootstrap container 
            // (map the types)
            container.Map<IPerson, Person>();
            container.Map<IAddress, Address>();
            container.Map<IOrg, Org>();
            container.MapMultiType<ILog, FileLog>();
            container.MapMultiType<ILog, ConsoleLog>();
            #endregion BOOTSTRAPPING

            // after CompleteConfiguration
            // you cannot bootstrap any new types in the container.
            // before CompleteConfiguration call
            // you cannot resolve container types. 
            container.CompleteConfiguration();

            // resolve and compose organization
            // all its 'Parts' will be added at
            // this stage. 
            IOrg org = container.Resolve<IOrg>();


            #region Set Org Data

            org.OrgName = "Nicks Department Store";
            org.Manager.PersonName = "Nick Polyak";
            org.Manager.Address.City = "Miami";
            org.Manager.Address.ZipCode = "33162";

            #endregion Set Org Data

            // Create file MyLogFile.txt in the same folder as the executable
            // and write department store info in it;
            org.LogOrgInfo();
        }
    }
}
