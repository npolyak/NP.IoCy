using DynamicAssemblyLoadingTest.Interfaces;
using NP.Utilities.Attributes;
using System;

namespace OrganizationTest.Implementations
{
    [Implements]
    public class ConsoleLog : ILog
    {
        public void WriteLog(string info)
        {
            Console.WriteLine(info);
        }
    }
}
