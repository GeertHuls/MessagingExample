using System;

namespace WheelsOnFire.Messaging
{
    public interface IOrderRegisteredEvent
    {
        Guid CorrelationId { get; }
    }
}
