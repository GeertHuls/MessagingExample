using System.Threading.Tasks;
using MassTransit;
using WheelsOnFire.Messaging;

namespace WheelsOnFire.Finance.Console
{
    public class OrderRegisteredConsumer : IConsumer<IOrderRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<IOrderRegisteredEvent> context)
        {
            //Save to db
            await System.Console.Out.WriteLineAsync($"New order received: Order id {context.Message.OrderId}");
        }
    }
}
