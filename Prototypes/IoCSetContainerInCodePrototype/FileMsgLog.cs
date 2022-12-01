using NP.IoC.Attributes;
using NP.Utilities;

namespace IoCSetContainerInCodePrototype
{
    public class FileMsgLog : IMessageLog
    {
        string? _fileName;

        private StreamWriter? _stream;

        [Inject(ResolvingType = typeof(string), ResolutionKey = "LogFileName")]
        public string? FileName
        {
            get => _fileName;
            set
            {
                if (_fileName == value)
                    return;

                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }

                _fileName = value;

                if (_fileName != null)
                {
                    _stream = new StreamWriter(_fileName);
                }
            }
        }

        public FileMsgLog()
        {

        }

        public FileMsgLog(string fileName)
        {
            FileName = fileName;
        }

        public void Log(string message)
        {
            _stream?.Write(message);
            _stream?.Flush();
        }
    }
}
