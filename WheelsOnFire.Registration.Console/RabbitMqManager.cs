using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using WheelsOnFire.Messaging;

namespace WheelsOnFire.Registration.Console
{
    public class RabbitMqManager: IDisposable
    {
        private readonly IModel _channel;

        public RabbitMqManager()
        {
            var connectionFactory = 
                new ConnectionFactory {Uri = RabbitMqConstants.RabbitMqUri};
            var connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            connection.AutoClose = true;
        }

        public void ListenForRegisterOrderCommand()
        {
            EnsureMessageQueueExits();

            //Bind order command to the queue:
            var consumer = new RegisteredOrderCommandConsumer(this);
            _channel.BasicConsume(
                queue: RabbitMqConstants.RegisterOrderQueue,
                noAck: false,
                consumer: consumer);
        }

        private void EnsureMessageQueueExits()
        {
            _channel.QueueDeclare(
                queue: RabbitMqConstants.RegisterOrderQueue,
                durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            //Prefetch gets multiple messages at once and process them in-memory.
            //Prefetch size set to 0 means no limit.
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1,
                global: false);
        }

        public void SendOrderRegisteredEvent(IOrderRegisteredEvent command)
        {
            SetupExchangeAndQueue();

            var serializedCommand = JsonConvert.SerializeObject(command);

            var messageProperties = _channel.CreateBasicProperties();
            messageProperties.ContentType = RabbitMqConstants.JsonMimeType;

            _channel.BasicPublish(
                exchange: RabbitMqConstants.OrderRegisteredExchange, 
                routingKey: "",
                basicProperties: messageProperties, 
                body: Encoding.UTF8.GetBytes(serializedCommand));
        }

        private void SetupExchangeAndQueue()
        {
            _channel.ExchangeDeclare(
                exchange: RabbitMqConstants.OrderRegisteredExchange,
                type: ExchangeType.Fanout); //Using exchange type fanout instead of direct.
            //Fanout because this exchange is meant to send out messages
            //to muliple queues.
            _channel.QueueDeclare(
                queue: RabbitMqConstants.OrderRegisteredNotificationQueue,
                durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            _channel.QueueBind(
                queue: RabbitMqConstants.OrderRegisteredNotificationQueue,
                exchange: RabbitMqConstants.OrderRegisteredExchange,
                routingKey: "");
        }

        public void SendAck(ulong deliveryTag)
        {
            //When rabbit mq receives this acknowledgement, it will be deleted from the queue.
            _channel.BasicAck(deliveryTag: deliveryTag, multiple: false);
        }

        public void Dispose()
        {
            if (!_channel.IsClosed)
                _channel.Close();
        }
    }
}
