namespace libsyslog
{
    using libsyslog.Model;
    using System;

    public class SyslogMessageEvent : EventArgs
    {
        public SyslogMessageInfo Message { get; set; }
    }
}