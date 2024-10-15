using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://admin:admin123@localhost:5672");
factory.ClientProvidedName = "RabbitMQ Sender Console App";

IConnection connection = factory.CreateConnection();

IModel channel = connection.CreateModel();

string exchangeName = "MyTopic";
string routingKey = "*.mytopic.*";
string queueName = channel.QueueDeclare().QueueName;

channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
channel.QueueBind(queueName, exchangeName, routingKey, null);

channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(2), CancellationToken.None).Wait();

    var body = args.Body.ToArray();

    string message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Consumer 02 received: {args.RoutingKey} : \"{message}\"");

    channel.BasicAck(args.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
connection.Close();