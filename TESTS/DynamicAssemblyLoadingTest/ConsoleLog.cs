using DynamicAssemblyLoadingTest.Interfaces;
using System;

namespace DynamicAssemblyLoadingTest
{
    public class ConsoleLog : ILog
    {
        public void WriteLog(string info)
        {
            Console.WriteLine(info);
        }
    }
}
