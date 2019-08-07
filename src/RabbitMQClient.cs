using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace azure_pub_sub.src
{
    public delegate void MessageReceived(string message);

    public class RabbitMQMessagesRepository<T> : IMessagingRepository<T> where T : PubSubSpecification
    {
        private static ConnectionFactory factory;
        private MessageReceived _messageReceived;
        private string _currentMessage;

        RabbitMQContext _dbContext;  

        public RabbitMQMessagesRepository(RabbitMQContext dbContext)
        {
            // factory = new ConnectionFactory() { HostName = hostname };
            _dbContext = dbContext; 
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
                                         consumer: new EventingBasicConsumer(channel));
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

        public string publishMessage(string queueName = "testQueue", string messageContent = "Hello World")
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = messageContent;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                return $"Sent {message} to {channel} ";

            }
        }

        public void ReceiveMessagesWithEvents() 
        {
            using (var connection = factory.CreateConnection()) 
            using(IModel channel = connection.CreateModel())
            {
                channel.BasicQos(0, 1, false); 
                EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel); 

                eventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) => 
                {
                    IBasicProperties basicProperties = basicDeliveryEventArgs.BasicProperties; 
                    Console.WriteLine("Message received by the event based consumer"); 
                    Debug.WriteLine($"Message: {Encoding.UTF8.GetString(basicDeliveryEventArgs.Body)}");  
                    channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false); 
                }; 

                channel.BasicConsume("testQueue", false, eventingBasicConsumer); 

            }
        }

        public T GetNextMessage()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> List()
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is RabbitMQMessagesRepository<T> repository &&
                   EqualityComparer<MessageReceived>.Default.Equals(_messageReceived, repository._messageReceived) &&
                   _currentMessage == repository._currentMessage &&
                   MostRecentMessage == repository.MostRecentMessage;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}