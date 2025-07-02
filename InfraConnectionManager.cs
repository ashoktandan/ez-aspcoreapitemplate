using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

public interface IInfraConnectionManager : IDisposable
{
    object GetConnection();
}

#if mysql
public class MySqlConnectionManager : IInfraConnectionManager
{
    private readonly ConcurrentBag<MySql.Data.MySqlClient.MySqlConnection> _pool = new();
    private readonly string _connectionString;
    private bool _disposed;

    public MySqlConnectionManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public object GetConnection()
    {
        if (_pool.TryTake(out var conn))
        {
            if (conn.State == System.Data.ConnectionState.Open)
                return conn;
            conn.Dispose();
        }
        var newConn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
        newConn.Open();
        return newConn;
    }

    public void ReturnConnection(MySql.Data.MySqlClient.MySqlConnection conn)
    {
        if (!_disposed && conn.State == System.Data.ConnectionState.Open)
            _pool.Add(conn);
        else
            conn.Dispose();
    }

    public void Dispose()
    {
        _disposed = true;
        while (_pool.TryTake(out var conn))
            conn.Dispose();
    }
}
#endif

#if postgres
public class PostgresConnectionManager : IInfraConnectionManager
{
    private readonly ConcurrentBag<Npgsql.NpgsqlConnection> _pool = new();
    private readonly string _connectionString;
    private bool _disposed;

    public PostgresConnectionManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public object GetConnection()
    {
        if (_pool.TryTake(out var conn))
        {
            if (conn.State == System.Data.ConnectionState.Open)
                return conn;
            conn.Dispose();
        }
        var newConn = new Npgsql.NpgsqlConnection(_connectionString);
        newConn.Open();
        return newConn;
    }

    public void ReturnConnection(Npgsql.NpgsqlConnection conn)
    {
        if (!_disposed && conn.State == System.Data.ConnectionState.Open)
            _pool.Add(conn);
        else
            conn.Dispose();
    }

    public void Dispose()
    {
        _disposed = true;
        while (_pool.TryTake(out var conn))
            conn.Dispose();
    }
}
#endif

#if rabbit
public class RabbitMqConnectionManager : IInfraConnectionManager
{
    private readonly ConcurrentBag<RabbitMQ.Client.IConnection> _pool = new();
    private readonly RabbitMQ.Client.ConnectionFactory _factory;
    private bool _disposed;

    public RabbitMqConnectionManager(string hostName)
    {
        _factory = new RabbitMQ.Client.ConnectionFactory() { HostName = hostName };
    }

    public object GetConnection()
    {
        if (_pool.TryTake(out var conn) && conn.IsOpen)
            return conn;
        var newConn = _factory.CreateConnection();
        return newConn;
    }

    public void ReturnConnection(RabbitMQ.Client.IConnection conn)
    {
        if (!_disposed && conn.IsOpen)
            _pool.Add(conn);
        else
            conn.Dispose();
    }

    public void Dispose()
    {
        _disposed = true;
        while (_pool.TryTake(out var conn))
            conn.Dispose();
    }
}
#endif

#if kafka
public class KafkaConnectionManager : IInfraConnectionManager
{
    private readonly ConcurrentBag<Confluent.Kafka.IProducer<Null, string>> _pool = new();
    private readonly Confluent.Kafka.ProducerConfig _config;
    private bool _disposed;

    public KafkaConnectionManager(string bootstrapServers)
    {
        _config = new Confluent.Kafka.ProducerConfig { BootstrapServers = bootstrapServers };
    }

    public object GetConnection()
    {
        if (_pool.TryTake(out var producer))
            return producer;
        var newProducer = new Confluent.Kafka.ProducerBuilder<Null, string>(_config).Build();
        return newProducer;
    }

    public void ReturnConnection(Confluent.Kafka.IProducer<Null, string> producer)
    {
        if (!_disposed)
            _pool.Add(producer);
        else
            producer.Dispose();
    }

    public void Dispose()
    {
        _disposed = true;
        while (_pool.TryTake(out var producer))
            producer.Dispose();
    }
}
#endif
