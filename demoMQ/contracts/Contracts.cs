using System;

namespace contracts
{
    public class RabbitClusterAzure
    {
        public const string ConnectionString = @"host=40.118.207.6:5672;username=test;password=test";
        // public const string ConnectionString = @"host=52.175.245.75:8883;username=test;password=test";
        // public const string ConnectionString = @"host=40.78.6.182:5672;username=test;password=test";
        
        // 52.175.245.75
    }

    public class Message
    {
        public string Body { get; set; }
    }
}
