using WheelsOnFire.Messaging;

namespace WheelsOnFire.Notification.Console
{
    public class OrderRegisteredConsumer
    {
        public void Consume(IOrderRegisteredEvent registeredEvent)
        {
            //Send notification to user
            System.Console.WriteLine("Customer notification sent: Order id " +
                              $"{registeredEvent.OrderId} registered");
        }
    }
}
