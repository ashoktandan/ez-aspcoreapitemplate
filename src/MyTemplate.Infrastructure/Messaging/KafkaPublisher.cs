#if kafka
using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace MyTemplate.Infrastructure.Messaging;

public interface IKafkaProducer : IDisposable
{
    Task ProduceAsync(string topic, string message);
}

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            Acks = Acks.All,
            EnableIdempotence = true, // Best for avoiding duplicate messages
            LingerMs = 5,
            BatchSize = 32 * 1024,
            MessageSendMaxRetries = 5,
            RetryBackoffMs = 100
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(5));
        _producer?.Dispose();
    }
}
using Confluent.Kafka;
using MyTemplate.Application.Messaging;
using System.Threading.Tasks;

namespace MyTemplate.Infrastructure.Messaging;

public class KafkaMessagePublisher : IMessagePublisher
{
    private readonly IKafkaProducer _producer;

    public KafkaMessagePublisher(IKafkaProducer producer)
    {
        _producer = producer;
    }

    public async Task PublishAsync(string topic, string message)
    {
        await _producer.ProduceAsync(topic, message);
    }
}

#endif
