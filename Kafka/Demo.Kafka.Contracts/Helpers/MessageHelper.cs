using System;
using Demo.Kafka.Contracts.Messages;

namespace Demo.Kafka.Contracts.Helpers;

public static class MessageHelper
{
    public static string GetStringSourceEntityId(string sourceEntityId) => sourceEntityId;

    public static Guid GetGuidSourceEntityId(string sourceEntityId) => Guid.TryParse(sourceEntityId, out var id)
        ? id
        : throw new ArgumentException($"Invalid SourceEntityId format. Current source {sourceEntityId}",
            nameof(BaseMessage<Guid>.SourceEntityId));

    public static int GetIntSourceEntityId(string sourceEntityId) => int.TryParse(sourceEntityId, out var id)
        ? id
        : throw new ArgumentException($"Invalid SourceEntityId format. Current source {sourceEntityId}",
            nameof(BaseMessage<Guid>.SourceEntityId));

    public static long GetLongSourceEntityId(string sourceEntityId) => long.TryParse(sourceEntityId, out var id)
        ? id
        : throw new ArgumentException($"Invalid SourceEntityId format. Current source {sourceEntityId}",
            nameof(BaseMessage<Guid>.SourceEntityId));
}