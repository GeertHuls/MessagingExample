using MassTransit;
using WheelsOnFire.Messaging;

namespace WheelsOnFire.CityMonitor.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Title = "City monitor";

            var bus = BusConfigurator.ConfigureBus((cfg, host) =>
            {
                cfg.EnablePerformanceCounters();
                cfg.ReceiveEndpoint(host, RabbitMqConstants.CityMonitorServiceQueue, e =>
                {
                    e.Consumer<CityMessageConsumer>();
                });
            });

            bus.Start();

            System.Console.WriteLine("Listening for city messages.. Press enter to exit");
            System.Console.ReadLine();

            bus.Stop();
        }
    }
}
