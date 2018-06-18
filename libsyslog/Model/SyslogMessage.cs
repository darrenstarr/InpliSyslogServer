using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("libsyslog.tests")]

namespace libsyslog.Model
{
    public class SyslogMessage
    {
        public SyslogMessageHeader Header { get; set; }
        public string Message { get; set; }
    }
}
