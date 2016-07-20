using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using WheelsOnFire.Messaging;
using WheelsOnFire.Registration.Console.Messages;

namespace WheelsOnFire.Registration.Console
{
    public class RegisteredOrderCommandConsumer: DefaultBasicConsumer
    {
        private readonly RabbitMqManager _rabbitMqManager;

        public RegisteredOrderCommandConsumer(
            RabbitMqManager rabbitMqManager)
        {
            this._rabbitMqManager = rabbitMqManager;
        }

        public override void HandleBasicDeliver(
            string consumerTag, ulong deliveryTag, 
            bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, byte[] body)
        {
            if (properties.ContentType != RabbitMqConstants.JsonMimeType)
                throw new ArgumentException(
                    $"Can't handle content type {properties.ContentType}");

            System.Console.WriteLine($"Correclation id = {properties.CorrelationId}");

            var message = Encoding.UTF8.GetString(body);
            var commandObj = 
                JsonConvert.DeserializeObject<RegisterOrderCommand>(
                    message);
            Consume(commandObj);
            //The delivery tag is unique.
            _rabbitMqManager.SendAck(deliveryTag);
        }

        private void Consume(IRegisterOrderCommand command)
        {
            //Store order registration and get Id
            var id = 12;

            System.Console.WriteLine($"Order with id {id} registered");
            System.Console.WriteLine("Publishing order registered event");

            //notify subscribers that a order is registered
            var orderRegisteredEvent = new OrderRegisteredEvent(command, id);
            //publish event
            _rabbitMqManager.SendOrderRegisteredEvent(orderRegisteredEvent);
        }
    }
}
