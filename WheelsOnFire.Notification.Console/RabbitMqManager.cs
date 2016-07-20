using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WheelsOnFire.Messaging;
using WheelsOnFire.Notification.Console.Messages;

namespace WheelsOnFire.Notification.Console
{
    public class RabbitMqManager : IDisposable
    {
        private readonly IModel _channel;

        public RabbitMqManager()
        {
            var connectionFactory = new ConnectionFactory { Uri = RabbitMqConstants.RabbitMqUri };
            var connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            connection.AutoClose = true;
        }

        public void ListenForOrderRegisteredEvent()
        {
            EnsureMessageQueueExists();

            var eventingConsumer = new EventingBasicConsumer(_channel);
            eventingConsumer.Received += (chan, eventArgs) =>
            {
                var contentType = eventArgs.BasicProperties.ContentType;
                if (contentType != RabbitMqConstants.JsonMimeType)
                    throw new ArgumentException(
                        $"Can't handle content type {contentType}");

                var message = Encoding.UTF8.GetString(eventArgs.Body);
                var orderConsumer = new OrderRegisteredConsumer();
                var commandObj =
                    JsonConvert.DeserializeObject<OrderRegisteredEvent>(message);
                orderConsumer.Consume(commandObj);
                DequeuMessage(eventArgs);
            };
            _channel.BasicConsume(
                queue: RabbitMqConstants.OrderRegisteredNotificationQueue,
                noAck: false,
                consumer: eventingConsumer);
        }

        private void DequeuMessage(BasicDeliverEventArgs eventArgs)
        {
            _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag,
                multiple: false);
        }

        private void EnsureMessageQueueExists()
        {
            _channel.QueueDeclare(
                queue: RabbitMqConstants.OrderRegisteredNotificationQueue,
                durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        public void Dispose()
        {
            if (!_channel.IsClosed)
                _channel.Close();
        }
    }
}