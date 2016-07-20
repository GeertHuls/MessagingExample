namespace WheelsOnFire.Registration.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Title = "Registration service";
            using (var rabbitMqManager = new RabbitMqManager())
            {
                rabbitMqManager.ListenForRegisterOrderCommand();
                System.Console.WriteLine("Listening for RegisterOrderCommand..");
                System.Console.ReadKey();
            }
}
    }
}
