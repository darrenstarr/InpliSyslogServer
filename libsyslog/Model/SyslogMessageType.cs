using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("libsyslog.tests")]

namespace libsyslog.Model
{
    public class SyslogMessageType
    {
        public string Facility { get; set; }
        public ESyslogSeverityLevel Severity { get; set; }
        public string Mnemonic { get; set; }
    }
}
