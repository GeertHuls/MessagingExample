using Automatonymous;
using MassTransit.Saga;
using WheelsOnFire.Messaging;

namespace WheelsOnFire.Saga
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Title = "Saga";
            var saga = new OrderSaga();
            var repo = new InMemorySagaRepository<OrderSagaState>();

            var bus = BusConfigurator.ConfigureBus((cfg, host) =>
            {
                cfg.ReceiveEndpoint(host, RabbitMqConstants.SagaQueue, e =>
                {
                    e.StateMachineSaga(saga, repo);
                });
            });
            bus.Start();
            System.Console.WriteLine("Saga active.. Press enter to exit");
            System.Console.ReadLine();
            bus.Stop();
        }
    }
}
