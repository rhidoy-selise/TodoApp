using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace TodoApp.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(ILogger<RabbitMqService> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "root",
            Password = "root"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void SendMessage<T>(string exchangeName, string routingKey, T message)
    {
        try
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.QueueDeclare(queue: routingKey, durable: false, exclusive: false, autoDelete: false,
                arguments: null);
            _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
        }
    }

    public EventingBasicConsumer? AddConsumer(string exchangeName, string routingKey)
    {
        try
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        var consumer = new EventingBasicConsumer(_channel);
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        return consumer;
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
        }

        return null;
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }

}
