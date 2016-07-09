using System;
using System.Text;
using WheelsOnFire.Messaging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace WheelsOnFireWeb
{
    public class RabbitMqManager : IDisposable
    {
        private readonly IModel channel;
        public RabbitMqManager()
        {
            var connectionFactory =
                new ConnectionFactory { Uri = RabbitMqConstants.RabbitMqUri };
            var connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            connection.AutoClose = true;
        }
        public void SendRegisterOrderCommand(IRegisterOrderCommand command)
        {
            EnsureMessageBrokerExits();

            var serializedCommand = JsonConvert.SerializeObject(command);

            var messageProperties = channel.CreateBasicProperties();
            messageProperties.ContentType =
                RabbitMqConstants.JsonMimeType;

            channel.BasicPublish(
                exchange: RabbitMqConstants.RegisterOrderExchange,
                routingKey: "",
                basicProperties: messageProperties,
                body: Encoding.UTF8.GetBytes(serializedCommand));
        }

        private void EnsureMessageBrokerExits()
        {
            channel.ExchangeDeclare(
                exchange: RabbitMqConstants.RegisterOrderExchange,
                type: ExchangeType.Direct);
            channel.QueueDeclare(
                queue: RabbitMqConstants.RegisterOrderQueue,
                durable: false, /*When set to false, data will exits in memory
                                and will never perist to disk. This is ok for demo
                                purposes but recommended to set to true for production
                                environment.*/
                exclusive: false, /*Parameter to set wheter this is 
                               exclusive for this connection or not.*/
                autoDelete: false, /*Will let the queue to delete itself
                    when the last consumer of this queue closes its channel.*/
                arguments: null);
            channel.QueueBind( /* Config binding between exchange and queue.*/
                queue: RabbitMqConstants.RegisterOrderQueue,
                exchange: RabbitMqConstants.RegisterOrderExchange,
                routingKey: string.Empty); //Empty string when routing is not used
        }

        public void Dispose()
        {
            if (!channel.IsClosed)
                channel.Close();
        }
    }
}
