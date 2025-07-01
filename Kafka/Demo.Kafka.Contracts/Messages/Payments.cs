using System;
using Demo.Kafka.Contracts.Helpers;

namespace Demo.Kafka.Contracts.Messages;

public record PaymentSucceeded : BaseMessage<Guid>
{
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }

    public PaymentSucceeded() : base(nameof(PaymentSucceeded)) {}

    public override Guid GetSourceEntityId() => MessageHelper.GetGuidSourceEntityId(SourceEntityId);
}

public record PaymentFailure : PaymentSucceeded
{
    public string Reason { get; set; } = string.Empty;
}