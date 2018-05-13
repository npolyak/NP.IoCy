using AssemblyLoadingTest.Interfaces;
using System;

namespace AssemblyLoadingTest
{
    public class ConsoleLog : ILog
    {
        public void WriteLog(string info)
        {
            Console.WriteLine(info);
        }
    }
}
