using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://admin:admin123@localhost:5672");
factory.ClientProvidedName = "RabbitMQ Sender Console App";

IConnection connection = factory.CreateConnection();

IModel channel = connection.CreateModel();

string exchangeName = "MyTopic";
string routingKey = "all.mytopic.subscriber";

channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

for (int i = 1; i <= 10; i++)
{
    Console.WriteLine($"Sending Message #{i}");

    byte[] messageBody = Encoding.UTF8.GetBytes($"Topic message #{i}");
    channel.BasicPublish(exchangeName, routingKey, null, messageBody);

    Task.Delay(TimeSpan.FromMilliseconds(500), CancellationToken.None).Wait();
}

channel.Close();
connection.Close();
