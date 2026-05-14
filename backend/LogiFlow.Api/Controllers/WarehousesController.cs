using LogiFlow.Api.Contracts;
using LogiFlow.Api.Realtime;
using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.Api.Controllers;

[ApiController]
[Route("api/warehouses")]
public sealed class WarehousesController(
    IWarehouseRepository warehouseRepository,
    IRealtimeUpdatePublisher realtimeUpdatePublisher
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<WarehouseResponse>> Create(CreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var warehouse = new Warehouse
        {
            Name = request.Name,
            Location = new GeoPoint(request.Latitude, request.Longitude)
        };

        await warehouseRepository.AddAsync(warehouse, cancellationToken);

        await realtimeUpdatePublisher.PublishWarehouseSnapshotAsync(warehouse, "Created", cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse.ToResponse());
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WarehouseResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var warehouses = await warehouseRepository.GetAllAsync(cancellationToken);

        return Ok(warehouses.Select(warehouse => warehouse.ToResponse()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WarehouseResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var warehouse = await warehouseRepository.GetByIdAsync(id, cancellationToken);
        if (warehouse is null) return NotFound();
        return Ok(warehouse.ToResponse());
    }
}