using OrgTestWithAssemblyLoading.Interfaces;
using System;

namespace OrgTestWithAssemblyLoading
{
    public class ConsoleLog : ILog
    {
        public void WriteLog(string info)
        {
            Console.WriteLine(info);
        }
    }
}
