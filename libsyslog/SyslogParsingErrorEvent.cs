namespace libsyslog
{
    using System;
    using System.Net;

    public class SyslogParsingErrorEvent : EventArgs
    {
        public string ExceptionMessage { get; set; }
        public IPEndPoint MessageSource { get; set; }
        public byte [] MessageData { get; set; }
    }
}