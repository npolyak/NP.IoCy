using BootstrappingTest.Interfaces;
using System;

namespace BootstrappingTest.Implementations
{
    public class ConsoleLog : ILog
    {
        public void WriteLog(string info)
        {
            Console.WriteLine(info);
        }
    }
}
