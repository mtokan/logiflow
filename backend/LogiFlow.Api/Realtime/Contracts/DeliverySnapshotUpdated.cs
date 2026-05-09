using LogiFlow.Api.Contracts;

namespace LogiFlow.Api.Realtime.Contracts;

public sealed record DeliverySnapshotUpdated(
    DeliveryResponse Delivery,
    string Reason,
    DateTimeOffset Timestamp
);