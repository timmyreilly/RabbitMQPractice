using azure_pub_sub.src;

namespace azure_pub_sub
{
    public class PubSubSpecification : EntityBase 
    {
        public string ServiceBusConnectionString { get; set; }
        public int ThroughPut { get; set; }
        public string Url { get; set; }
         
    }
}