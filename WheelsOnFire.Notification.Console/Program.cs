namespace WheelsOnFire.Notification.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Title = "Notification service";
            using (var rabbitMqManager = new RabbitMqManager())
            {
                rabbitMqManager.ListenForOrderRegisteredEvent();
                System.Console.WriteLine("Listening for OrderRegisteredEvent..");
                System.Console.ReadKey();
            }
        }
    }
}
