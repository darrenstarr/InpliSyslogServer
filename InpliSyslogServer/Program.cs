namespace InpliSyslogServer
{
    using Couchbase;
    using System;
    using System.Collections.Generic;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            SyslogMessageConsumer consumer =
                new SyslogMessageConsumer(
                    new List<Uri> { new Uri("http://10.100.11.97") },
                    "BornToFail1",
                    "Monkey",
                    "Minions12345",
                    "syslog::Inpli::Helsfyr::"
                    );

            void MessageReceivedHandler(object sender, libsyslog.SyslogMessageEvent ev)
            {
                consumer.Consume(ev.Message);
            }

            void UnhandledMessageReceivedHandler(object sender, libsyslog.SyslogParsingErrorEvent ev)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to parse message: " + ev.ExceptionMessage);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ev.MessageSource.Address.ToString() + " - " + Encoding.UTF8.GetString(ev.MessageData));
                Console.ForegroundColor = oldColor;
            }

            libsyslog.SyslogServer.Instance.MessageReceived += MessageReceivedHandler;
            libsyslog.SyslogServer.Instance.UnhandledMessageReceived += UnhandledMessageReceivedHandler;


            libsyslog.SyslogServer.Instance.Start();

            Console.ReadKey();
        }
    }
}
