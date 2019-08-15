using System;

namespace contracts
{
    public class RabbitClusterAzure
    {
        public const string ConnectionString =
            @"host=40.118.230.64:5672;username=test;password=test";
    }


    public class Message
    {
        public string Body { get; set; }
    }
}
