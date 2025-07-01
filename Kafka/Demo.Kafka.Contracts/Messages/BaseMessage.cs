using System;

namespace Demo.Kafka.Contracts.Messages;

public record BaseMessage<TId> where TId : notnull
{
    public string SourceEvent { get; set; } = string.Empty;
    public string SourceEntityId { get; set; } = string.Empty;

    public int Version { get; set; }
    public string Type { get; set; }

    public BaseMessage(string type)
    {
        Type = type;
    }
    
    public virtual TId GetSourceEntityId() => throw new NotImplementedException(nameof(GetSourceEntityId));
}