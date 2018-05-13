using NP.IoCy.Attributes;
using OrgTestWithAssemblyLoading.Interfaces;
using System.IO;

namespace OrgTestWithAssemblyLoading.Implementations
{
    [Implements]
    public class FileLog : ILog
    {
        const string FileName = "MyLogFile.txt";

        public FileLog()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
        }

        public void WriteLog(string info)
        {
            using(StreamWriter writer = new StreamWriter(FileName, true))
            {
                writer.WriteLine(info);
            }
        }
    }
}
