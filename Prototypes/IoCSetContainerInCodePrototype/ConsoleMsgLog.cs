using NP.Utilities;

namespace IoCSetContainerInCodePrototype
{
    public class ConsoleMsgLog : IMessageLog
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
