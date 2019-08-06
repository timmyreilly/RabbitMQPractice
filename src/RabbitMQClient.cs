using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Collections.Generic;

namespace azure_pub_sub.src
{
    public delegate void MessageReceived(string message);

    public class RabbitMQClient
    {
        private static ConnectionFactory factory;
        private MessageReceived _messageReceived;
        private string _currentMessage;


        public RabbitMQClient(string hostname = "localhost")
        {
            factory = new ConnectionFactory() { HostName = hostname };
        }

        public bool checkConnection()
        {
            if (factory != null)
            {
                return true;
            }
            return false;
        }

        public void RegisterMessageWatcher(MessageReceived watcher)
        {

            _messageReceived += watcher;
        }

        public IEnumerable<string> generateMessagesFromRabbitMQ(string queueName = "test")
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                string message = "done";
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
                yield return message;
            }
        }

        public void TurnOnReceiver(string queueName = "test")
        {
            while (true)
            {

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);
                }
            }
        }

        public string MostRecentMessage
        {
            get
            {
                return _currentMessage;
            }
            set
            {
                var previous = _currentMessage;
                _currentMessage = value;
                OnMessageReceived(value);
            }
        }

        private void OnMessageReceived(string message)
        {
            if (_messageReceived != null)
            {
                _messageReceived(message);
            }

        }

        public string publishMessage(string queueName = "hello", string messageContent = "Hello World")
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = messageContent;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);
                return $"Sent {message} to {channel} ";

            }
        }

    }
}