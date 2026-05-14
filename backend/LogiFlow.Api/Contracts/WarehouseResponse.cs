namespace LogiFlow.Api.Contracts;

public sealed record WarehouseResponse(
    Guid Id,
    string Name,
    GeoPointResponse Location
);