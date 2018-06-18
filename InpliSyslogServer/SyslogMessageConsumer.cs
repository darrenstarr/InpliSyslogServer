namespace InpliSyslogServer
{
    using Couchbase;
    using Couchbase.Authentication;
    using Couchbase.Configuration.Client;
    using Couchbase.Core.Serialization;
    using libsyslog.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    // SELECT * FROM BornToFail1 WHERE Type = "syslog::message" ORDER BY STR_TO_MILLIS(ReceivedAt) DESC LIMIT 10

    public class SyslogMessageConsumer
    {
        public IEnumerable<Uri> Servers => DbCluster.Configuration.Servers;
        public string BucketName { get; private set; }

        public string DocumentPrefix { get; private set; } = "syslog::";

        private Cluster DbCluster;
        private BufferBlock<SyslogMessageInfo> MessageQueue = new BufferBlock<SyslogMessageInfo>();
        CancellationTokenSource dbCts = new CancellationTokenSource();

        public SyslogMessageConsumer(List<Uri> bootstrapServers, string bucketName, string bucketUsername, string bucketPassword, string documentPrefix)
        {
            BucketName = bucketName;
            DocumentPrefix = documentPrefix;

            DbCluster = new Cluster(new ClientConfiguration
            {
                Servers = bootstrapServers,
                Serializer = () =>
                {
                    JsonSerializerSettings serializerSettings =
                        new JsonSerializerSettings()
                        {
                            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                            DateTimeZoneHandling = DateTimeZoneHandling.Utc
                        };

                    serializerSettings.Converters.Add(new IPAddressConverter());
                    serializerSettings.Converters.Add(new IPEndPointConverter());

                    return new DefaultSerializer(serializerSettings, serializerSettings);
                }
            });

            var authenticator = new PasswordAuthenticator(bucketUsername, bucketPassword);
            DbCluster.Authenticate(authenticator);

            Task.Factory.StartNew(
                async () => {
                    await ConsumeBuffer(MessageQueue);
                },
                dbCts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            );
        }

        public Task<HttpResponseMessage> PostAsJsonAsync<T>(
            HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        class ConfigRequestObject
        {
            public string DeviceId { get; set; }
            public Guid ConfigurationId { get; set; }
        }

        internal async Task<int> ConsumeBuffer(ISourceBlock<SyslogMessageInfo> source)
        {
            int count = 0;
            using (var bucket = DbCluster.OpenBucket("BornToFail1"))
            {
                while(await source.OutputAvailableAsync())
                {
                    var message = source.Receive();

                    var document = new Document<dynamic>
                    {
                        Id = DocumentPrefix + "message::" + Guid.NewGuid().ToString(),
                        Content = message
                    };

                    var insert = await bucket.InsertAsync(document);
                    if (insert.Success)
                    {
                        Console.WriteLine(document.Id);
                        count++;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(insert.Status.ToString());
                    }

                    if(
                        message.Message != null &&
                        message.Message.Header != null
                        && message.Message.Header.MessageType != null &&
                        message.Message.Header.MessageType.Facility == "SYS"
                        && message.Message.Header.MessageType.Mnemonic == "CONFIG_I"
                    )
                    {
                        var oldColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Received Configuration change notification from " + message.Sender.ToString());
                        Console.ForegroundColor = oldColor;

                        var client = new HttpClient();

                        var response = await PostAsJsonAsync(
                            client,
                            "http://localhost:51954/api/values",
                            new ConfigRequestObject
                            {
                                ConfigurationId = Guid.NewGuid(),
                                DeviceId = message.Sender.Address.ToString()
                            }
                        );
                    }
                }
            }
            return count;
        }

        internal void Consume(SyslogMessageInfo message)
        {
            ITargetBlock<SyslogMessageInfo> target = MessageQueue;
            target.Post(message);
        }
    }
}
