using WheelsOnFire.Messaging;

namespace WheelsOnFire.Registration.Console.Messages
{
    public class OrderRegisteredEvent: IOrderRegisteredEvent
    {
        private readonly IRegisterOrderCommand _command;

        public OrderRegisteredEvent(IRegisterOrderCommand command, int orderId)
        {
            this._command = command;
            this.OrderId = orderId;
        }
        public int OrderId { get; }

        public string PickupName => _command.PickupName;
        public string PickupAddress => _command.PickupAddress;
        public string PickupCity => _command.PickupCity;

        public string DeliverName => _command.DeliverName;
        public string DeliverAddress => _command.DeliverAddress;
        public string DeliverCity => _command.DeliverCity;

        public int Weight => _command.Weight;
        public bool Fragile => _command.Fragile;
        public bool Oversized => _command.Oversized;
    }
}
