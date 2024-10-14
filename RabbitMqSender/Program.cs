using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://admin:admin123@localhost:5672");
factory.ClientProvidedName = "RabbitMQ Sender Console App";

IConnection connection = factory.CreateConnection();

IModel channel = connection.CreateModel();

string exchangeName = "MyExchange";
string routingKey = "my-routing-key";
string queueName = "MyQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);//Fanout, Direct, Topic, Headers
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

for (int i = 1; i <= 20; i++)
{
    Console.WriteLine($"Sending Message #{i}");

    byte[] messageBody = Encoding.UTF8.GetBytes($"Message #{i}");
    channel.BasicPublish(exchangeName, routingKey, null, messageBody);

    Task.Delay(TimeSpan.FromMilliseconds(500), CancellationToken.None).Wait();
}

channel.Close();
connection.Close();
