using System;
using Automatonymous;
using WheelsOnFire.Messaging;

namespace WheelsOnFire.Saga
{
    public class OrderSaga : MassTransitStateMachine<OrderSagaState>
    {
        public State Received { get; private set; }
        public State Registered { get; private set; }

        public Event<IRegisterOrderCommand> RegisterOrder { get; private set; }
        public Event<IOrderRegisteredEvent> OrderRegistered { get; private set; }

        public OrderSaga()
        {
            //Define workflow:
            InstanceState(s => s.CurrentState);

            Event(() => RegisterOrder,
                cc =>       //for demo puposes use pickupname as identifier
                    cc.CorrelateBy(state => state.PickupName, context =>
                        context.Message.PickupName)
                        .SelectId(context => Guid.NewGuid()));

            Event(() => OrderRegistered, x => x.CorrelateById(context =>
                context.Message.CorrelationId));

            //Define what triggers a new workflow:
            Initially(
                When(RegisterOrder)
                    .Then(context =>
                    {
                        context.Instance.ReceivedDateTime = DateTime.Now;
                        context.Instance.PickupName = context.Data.PickupName;
                        context.Instance.PickupAddress = context.Data.PickupAddress;
                        context.Instance.PickupCity = context.Data.PickupCity;
                        //etc
                    })
                    .ThenAsync(
                        context => Console.Out.WriteLineAsync($"Order for customer" +
                                                              $" {context.Data.PickupName} received"))
                    .TransitionTo(Received)
                    //the registration service listens to the OrderReceivedEvent message.
                    .Publish(context => new OrderReceivedEvent(context.Instance))
                );

            //Define when sage is in the receive state:
            During(Received,
                When(OrderRegistered)
                    .Then(context => context.Instance.RegisteredDateTime =
                        DateTime.Now)
                    .ThenAsync(
                        context => Console.Out.WriteLineAsync(
                            $"Order for customer {context.Instance.PickupName} " +
                            $"registered"))
                    .Finalize()
                );

            //SagaStageObject will be deleted here:
            SetCompletedWhenFinalized();
        }
    }
}