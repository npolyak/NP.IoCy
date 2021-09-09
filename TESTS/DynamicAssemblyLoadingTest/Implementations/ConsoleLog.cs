using DynamicAssemblyLoadingTest.Interfaces;
using System;

namespace OrganizationTest.Implementations
{
    public class ConsoleLog : ILog
    {
        public void WriteLog(string info)
        {
            Console.WriteLine(info);
        }
    }
}
