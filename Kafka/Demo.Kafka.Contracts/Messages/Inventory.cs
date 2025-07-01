using System;
using System.Collections.Generic;
using Demo.Kafka.Contracts.Helpers;

namespace Demo.Kafka.Contracts.Messages;

public record InventoryDeductionSucceeded : BaseMessage<Guid>
{
    public Dictionary<int, decimal> GoodsDeducted { get; set; } = [];

    public InventoryDeductionSucceeded() : base(nameof(InventoryDeductionSucceeded)) {}

    public override Guid GetSourceEntityId() => MessageHelper.GetGuidSourceEntityId(SourceEntityId);
}

public record InventoryDeductionFailure : BaseMessage<Guid>
{
    public string Reason { get; set; } = string.Empty;

    public InventoryDeductionFailure() : base(nameof(InventoryDeductionFailure)) { }
    
    public override Guid GetSourceEntityId() => MessageHelper.GetGuidSourceEntityId(SourceEntityId);
}