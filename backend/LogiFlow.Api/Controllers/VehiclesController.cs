using LogiFlow.Api.Contracts;
using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
public sealed class VehiclesController(IVehicleRepository vehicleRepository) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> Create(CreateVehicleRequest request,
        CancellationToken cancellationToken)
    {
        var vehicle = new Vehicle { Name = request.Name };
        await vehicleRepository.AddAsync(vehicle, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle.ToResponse());
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VehicleResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var vehicles = await vehicleRepository.GetAllAsync(cancellationToken);
        return Ok(vehicles.Select(vehicle => vehicle.ToResponse()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VehicleResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(id, cancellationToken);
        if (vehicle is null) return NotFound();

        return Ok(vehicle.ToResponse());
    }
}