using System;
using EasyNetQ; 
using contracts; 

namespace publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus(RabbitClusterAzure.ConnectionString))
            {
                var input = "";
                Console.WriteLine("Enter a message. 'Quit' to quit.");
                while ((input = Console.ReadLine()) != "Quit")
                {
                    Publish(bus, input);
                }
            }
        }

        private static void Publish(IBus bus, string input)
        {
            bus.Publish(new contracts.Message
            {
                Body = input
            });
        }
    }
}
