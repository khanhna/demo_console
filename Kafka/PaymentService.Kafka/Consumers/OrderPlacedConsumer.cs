using Demo.Kafka.Contracts.Messages;
using DotNetCore.CAP;

namespace PaymentService.Kafka.Consumers;

public class OrderPlacedConsumer : ICapSubscribe
{
    [CapSubscribe(OrderPlaced.MessageName)]
    public void Hanlde(OrderPlaced order)
    {
        Console.WriteLine(
            $"--> Received message {nameof(OrderPlaced)} -  {nameof(OrderPlaced.CustomerId)}: {order.CustomerId} | {nameof(OrderPlaced.OrderId)}: {order.OrderId} | {nameof(OrderPlaced.Amount)}: {order.Amount}");
    }
}