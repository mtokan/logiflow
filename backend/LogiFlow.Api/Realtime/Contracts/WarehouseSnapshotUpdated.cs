using LogiFlow.Api.Contracts;

namespace LogiFlow.Api.Realtime.Contracts;

public sealed record WarehouseSnapshotUpdated(
    WarehouseResponse Warehouse,
    string Reason,
    DateTimeOffset Timestamp
);