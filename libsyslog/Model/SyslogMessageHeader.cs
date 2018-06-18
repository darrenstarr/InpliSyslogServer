using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("libsyslog.tests")]

namespace libsyslog.Model
{
    using System;

    public class SyslogMessageHeader
    {
        public int Pri { get; set; }
        public int Facility => Pri >> 3;
        public ESyslogSeverityLevel Severity => (ESyslogSeverityLevel)(Pri & 0x7);
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public SyslogMessageType MessageType { get; set; }
    }
}
