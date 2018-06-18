namespace libsyslog
{
    using libsyslog.Helpers;
    using libsyslog.Model;
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public class SyslogServer
    {
        private static SyslogServer _instance;
        public static SyslogServer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SyslogServer();
                return _instance;
            }
        }

        private UdpClient Socket { get; set; }
        private UdpClient Socket6 { get; set; }

        private string LocalDomainName { get; set; }
        private string LocalHostName { get; set; }
        private string LocalFQDN { get; set; }

        public EventHandler<SyslogMessageEvent> MessageReceived;
        public EventHandler<SyslogParsingErrorEvent> UnhandledMessageReceived;

        public SyslogServer()
        {
            LocalDomainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            LocalHostName = Dns.GetHostName();

            LocalFQDN = LocalHostName + (string.IsNullOrWhiteSpace(LocalDomainName) ? "" : ("." + LocalDomainName));
        }

        public void Start(int port=514)
        {
            Socket = new UdpClient(port);
            Socket.BeginReceive(new AsyncCallback(OnUdpData), Socket);

            Socket6 = new UdpClient(port, AddressFamily.InterNetworkV6);
            Socket6.BeginReceive(new AsyncCallback(OnUdpData), Socket6);
        }

        private void OnUdpData(IAsyncResult result)
        {
            var timeReceived = DateTimeOffset.Now;
            var socket = result.AsyncState as UdpClient;

            IPEndPoint source = new IPEndPoint(0, 0);
            byte[] messageData = socket.EndReceive(result, ref source);

            try
            {
                MessageReceived?.Invoke(this, 
                    new SyslogMessageEvent
                    {
                        Message = new SyslogMessageInfo
                        {
                            Type = "syslog::message",
                            ReceivedAt = timeReceived,
                            Sender = source,
                            Receiver = new IPEndPoint(LocalRoutingTable.QueryRoutingInterface(source.Address), 514),
                            ReceiverHost = LocalFQDN,
                            Message = SyslogMessageParser.ParseMessage(messageData),
                        }
                    }
                );
            }
            catch(Exception e)
            {
                UnhandledMessageReceived?.Invoke(this,
                    new SyslogParsingErrorEvent
                    {
                        MessageSource = source,
                        ExceptionMessage = e.Message,
                        MessageData = messageData
                    }
                );
            }

            socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
        }
    }
}
