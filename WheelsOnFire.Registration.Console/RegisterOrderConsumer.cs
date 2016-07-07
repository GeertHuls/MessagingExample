using WheelsOnFire.Messaging;
using WheelsOnFire.Registration.Console.Messages;

namespace WheelsOnFire.Registration.Console
{
    public class RegisterOrderConsumer
    {
        public void Consume(IRegisterOrderCommand command)
        {
            //Store order registration and get Id
            var id = 12;

            System.Console.WriteLine($"Order with id {id} registered");

            //notify subscribers that a order is registered
            var orderRegisteredEvent =
                new OrderRegisteredEvent(command, id);
            //publish event
        }
    }
}
