using WheelsOnFire.Messaging;
using MassTransit;

namespace WheelsOnFire.Registration.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Title = "Registration";

            var bus = BusConfigurator.ConfigureBus((cfg, host) =>
            {
                cfg.ReceiveEndpoint(host,
                    RabbitMqConstants.RegisterOrderServiceQueue, e =>
                    {
                        e.Consumer<RegisterOrderCommandConsumer>();
                    });
            });

            bus.Start();

            System.Console.WriteLine("Listening for Register order commands.. " +
                              "Press enter to exit");
            System.Console.ReadLine();

            bus.Stop();
        }
    }
}
