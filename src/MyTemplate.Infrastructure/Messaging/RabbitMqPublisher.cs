#if rabbit
using RabbitMQ.Client;
using System;
using MyTemplate.Application.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate.Infrastructure.Messaging;

public class RabbitMqMessagePublisher : IMessagePublisher
{
    private readonly IRabbitMqConnection _connection;

    public RabbitMqMessagePublisher(IRabbitMqConnection connection)
    {
        _connection = connection;
    }

    public Task PublishAsync(string topic, string message)
    {
        using var channel = _connection.CreateChannel();
        channel.QueueDeclare(queue: topic, durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: topic, basicProperties: null, body: body);

        return Task.CompletedTask;
    }
}


public interface IRabbitMqConnection : IDisposable
{
    IModel CreateChannel();
}

public class RabbitMqConnection : IRabbitMqConnection
{
    private readonly IConnection _connection;
    private readonly ConnectionFactory _factory;

    public RabbitMqConnection()
    {
        _factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
            DispatchConsumersAsync = true
        };

        // Create single long-lived connection (Connection Pooling pattern)
        _connection = _factory.CreateConnection();
    }

    public IModel CreateChannel()
    {
        return _connection.CreateModel();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
#endif

