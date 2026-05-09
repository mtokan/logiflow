using LogiFlow.Api.Contracts;

namespace LogiFlow.Api.Realtime.Contracts;

public sealed record VehicleSnapshotUpdated(
    VehicleResponse Vehicle,
    string Reason,
    DateTimeOffset Timestamp
);