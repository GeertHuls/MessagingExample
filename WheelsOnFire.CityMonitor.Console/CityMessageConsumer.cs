using System.Threading.Tasks;
using MassTransit;
using WheelsOnFire.Messaging;

namespace WheelsOnFire.CityMonitor.Console
{
    public class CityMessageConsumer: IConsumer<ICityMessage>
    {
        public async Task Consume(ConsumeContext<ICityMessage> context)
        {
            await System.Console.Out.WriteLineAsync("City message captured");
        }
    }
}
