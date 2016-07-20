using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using WheelsOnFire.Messaging;

namespace WheelsOnFire.Registration.Console
{
    public class RabbitMqManager: IDisposable
    {
        private readonly IModel channel;

        public RabbitMqManager()
        {
            var connectionFactory = 
                new ConnectionFactory {Uri = RabbitMqConstants.RabbitMqUri};
            var connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            connection.AutoClose = true;
        }

        public void ListenForRegisterOrderCommand()
        {
            channel.QueueDeclare(
                queue: RabbitMqConstants.RegisterOrderQueue, 
                durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            //Prefetch gets multiple messages at once and process them in-memory.
            //Prefetch size set to 0 means no limit.
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, 
                global: false);

            //Bind order command to the queue:
            var consumer = new RegisteredOrderCommandConsumer(this);
            channel.BasicConsume(
                queue: RabbitMqConstants.RegisterOrderQueue,
                noAck: false,
                consumer: consumer);
        }

        public void SendOrderRegisteredEvent(IOrderRegisteredEvent command)
        {
            channel.ExchangeDeclare(
                exchange: RabbitMqConstants.OrderRegisteredExchange, 
                type: ExchangeType.Fanout); //Using exchange type fanout instead of direct.
                                //Fanout because this exchange is meant to send out messages
                                //to muliple queues.
            channel.QueueDeclare(
                queue: RabbitMqConstants.OrderRegisteredNotificationQueue, 
                durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            channel.QueueBind(
                queue: RabbitMqConstants.OrderRegisteredNotificationQueue,
                exchange: RabbitMqConstants.OrderRegisteredExchange, 
                routingKey: "");

            var serializedCommand = JsonConvert.SerializeObject(command);

            var messageProperties = channel.CreateBasicProperties();
            messageProperties.ContentType = RabbitMqConstants.JsonMimeType;

            channel.BasicPublish(
                exchange: RabbitMqConstants.OrderRegisteredExchange, 
                routingKey: "",
                basicProperties: messageProperties, 
                body: Encoding.UTF8.GetBytes(serializedCommand));
        }

        public void SendAck(ulong deliveryTag)
        {
            //When rabbit mq receives this acknowledgement, it will be deleted from the queue.
            channel.BasicAck(deliveryTag: deliveryTag, multiple: false);
        }

        public void Dispose()
        {
            if (!channel.IsClosed)
                channel.Close();
        }
    }
}
