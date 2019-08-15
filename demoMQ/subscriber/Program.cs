using System;
using contracts; 
using EasyNetQ; 


namespace subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus(RabbitClusterAzure.ConnectionString))
            {
                var retValue = bus.Subscribe<contracts.Message>("Sample_Topic", HandleTextMessage);

                Console.WriteLine("Listening for messages. Hit  to quit.");
                Console.ReadLine();
            }
        }

        static void HandleTextMessage(contracts.Message textMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got message: {0}", textMessage.Body);
            Console.ResetColor();
        }
    }
}
