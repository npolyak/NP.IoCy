using OrganizationTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationTest.Implementations
{
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
