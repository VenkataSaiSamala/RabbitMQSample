using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace RabbitMQSender
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            factory.ClientProvidedName = "Rabbit Mq Sender";

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            string exchangeName = "DemoExchange";
            string routingkey = "demo-routing-key";
            string queueName = "DemoQueue";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingkey, null);

            for (int i = 0; i < 60; i++)
            {
                Console.WriteLine($"Sending mesesage: {i}");
                byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"RabbitMQ Message: {i}");
                channel.BasicPublish(exchangeName, routingkey, null, messageBodyBytes);

                Thread.Sleep(1000);
            }


            channel.Close();
            conn.Close();
        }
    }
}
