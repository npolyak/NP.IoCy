using OrgTestWithDynamicAssemblyLoading.Interfaces;
using System;

namespace OrgTestWithDynamicAssemblyLoading
{
    public class ConsoleLog : ILog
    {
        public void WriteLog(string info)
        {
            Console.WriteLine(info);
        }
    }
}
