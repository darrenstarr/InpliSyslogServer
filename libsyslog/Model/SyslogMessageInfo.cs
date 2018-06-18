namespace libsyslog.Model
{
    using System;
    using System.Net;

    public class SyslogMessageInfo
    {
        public string Type { get; set; }
        public DateTimeOffset ReceivedAt { get; set; }
        public IPEndPoint Sender { get; set; }
        public IPEndPoint Receiver { get; set; }
        public string ReceiverHost { get; set; }
        public SyslogMessage Message { get; set; }
        public Guid Tenant { get; set; }
        public Guid Site { get; set; }
    }
}
