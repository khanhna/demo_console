using System;

namespace Demo.Kafka.Contracts.Messages;

public record OrderPlaced : BaseMessage<Guid>
{
    public const string MessageName = "Demo.Kafka.Contracts.Messages.OrderPlaced";

    public OrderPlaced() : base(nameof(OrderPlaced)) {}
    
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }

    public override Guid GetSourceEntityId() => OrderId;
}