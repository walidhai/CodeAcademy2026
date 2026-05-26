using CodeAcademy.DotnetConsumer.Common.Config;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

Console.WriteLine("Producer starting...");
// Establish connection to RabbitMQ
using var connection = await ConnectionHelper.ConnectAsync();
Console.WriteLine("Connected to RabbitMQ");

// Implement a basic producer here.
// Start with:
// - Create a channel
// - Declare a queue
// - Publish a message to the queue (you can use a simple JSON string as the message body)


// Create a channel and declare the queue
await using var channel = await connection.CreateChannelAsync();
//await channel.QueueDeclareAsync(queue: "idem-events", durable: true, exclusive: false, autoDelete: false, arguments: null);
await channel.QueueDeclareAsync(queue: "chat_walid", durable: true, exclusive: false, autoDelete: false, arguments: null);
await channel.ExchangeDeclareAsync(exchange: "chat", type: ExchangeType.Fanout);
//await channel.ExchangeDeclareAsync("idem-direct", type: "direct", durable: true);


// Publish messages to the queue with for loop to simulate multiple events
for (int i = 0; i < 10; i++)
{
    var message = $" ;^)  {i + 1} at {DateTime.Now}";

    var messageBody = JsonSerializer.Serialize(message);
    var body = Encoding.UTF8.GetBytes(messageBody);

    await channel.BasicPublishAsync(exchange: "chat", routingKey: "idem-fanout", mandatory: true, basicProperties: new BasicProperties { Persistent = true }, body: body);
    Console.WriteLine($"Published event: {message}");

    await Task.Delay(2000);
}

Console.WriteLine("Producer finished.");