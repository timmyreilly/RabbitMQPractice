using System;
using azure_pub_sub.src;

namespace azure_pub_sub
{
    class Program
    {
        static void Main(string[] args)
        {
            // give me the data 
            // I'm going to go with rabbitMQ 

            string queueName = "testQueue";
            string message = "Don't mess with the stig";

            RabbitMQClient rabbit = new RabbitMQRepository("rabbitDemo.westus.azurecontainer.io");
            string response = rabbit.publishMessage(queueName, message);
            Console.WriteLine(response);

            // get data from rabbitMQ: 

            rabbit.ReceiveMessagesWithEvents(); 

            // rabbit.RegisterMessageWatcher(LogMessageToConsole);
            // foreach (var n in rabbit.generateMessagesFromRabbitMQ())
            // {
            //     Console.WriteLine(n);
            // }



            // authorize with the function


            // authorize with the storage service

            // edit the application settings5
            // update the URL
            // upsert the MAX_CONCURRENT_SCALE_OUT

            // edit the host.json 
            // deserialize the json
            // update the object
            // write to the file 
            // upload the file 
            Console.WriteLine("All Done, press any key to exit"); 
            Console.Read();

        }

        private static void LogMessageToConsole(string message)
        {
            Console.WriteLine($"Most Recent Message {message}");
        }
    }
}
