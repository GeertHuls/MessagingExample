using System.Threading.Tasks;
using MassTransit;
using WheelsOnFire.Messaging;

namespace WheelsOnFire.Notification.Console
{
    public class OrderRegisteredConsumer : IConsumer<IOrderRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<IOrderRegisteredEvent> context)
        {
            //Send notification to user
            await System.Console.Out.WriteLineAsync($"Customer notification sent: Order id {context.Message.CorrelationId}");
        }
    }
}
