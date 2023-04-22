using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQRecieverBak
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

            //basic quality of service
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(3)).Wait();
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Message Recieved: {0}", message);

                channel.BasicAck(args.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume(queueName, false, consumer);

            Console.ReadLine();

            channel.Close();
            conn.Close();
        }
    }
}
