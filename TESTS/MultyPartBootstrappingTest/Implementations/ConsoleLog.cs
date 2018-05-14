using MultyPartBootstrappingTest.Interfaces;
using System;

namespace MultyPartBootstrappingTest.Implementations
{
    public class ConsoleLog : ILog
    {
        public void WriteLog(string info)
        {
            Console.WriteLine(info);
        }
    }
}
