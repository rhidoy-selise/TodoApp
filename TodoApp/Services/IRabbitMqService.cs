using RabbitMQ.Client.Events;

namespace TodoApp.Services;

public interface IRabbitMqService : IDisposable
{
    void SendMessage<T>(string exchangeName, string routingKey, T message);
    EventingBasicConsumer? AddConsumer(string exchangeName, string routingKey);
}
