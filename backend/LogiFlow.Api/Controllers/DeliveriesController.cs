using LogiFlow.Api.Contracts;
using LogiFlow.Api.Realtime;
using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Workflow;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;
using LogiFlow.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.Api.Controllers;

[ApiController]
[Route("api/deliveries")]
public sealed class DeliveriesController(
    IDeliveryRepository deliveryRepository,
    IDeliveryEventRepository eventRepository,
    IDeliveryRouteRepository routeRepository,
    IVehicleRepository vehicleRepository,
    IWorkflowEngine workflowEngine,
    ITrackingSimulationService trackingSimulationService,
    IRealtimeUpdatePublisher realtimeUpdatePublisher
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<DeliveryResponse>> Create(CreateDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var delivery = new Delivery
        {
            Code = request.Code,
            Origin = new GeoPoint(request.OriginLatitude, request.OriginLongitude),
            Destination = new GeoPoint(request.DestinationLatitude, request.DestinationLongitude)
        };

        await deliveryRepository.AddAsync(delivery, cancellationToken);

        var deliveryEvent = new DeliveryEvent
        {
            DeliveryId = delivery.Id,
            Type = DeliveryEventType.DeliveryCreated,
            Message = $"Delivery {delivery.Code} was created."
        };

        await eventRepository.AddAsync(deliveryEvent, cancellationToken);

        await realtimeUpdatePublisher.PublishDeliveryLogEventAsync(deliveryEvent, cancellationToken);

        await realtimeUpdatePublisher.PublishDeliverySnapshotAsync(
            delivery,
            "Created",
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = delivery.Id },
            delivery.ToResponse()
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DeliveryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var deliveries = await deliveryRepository.GetAllAsync(cancellationToken);
        return Ok(deliveries.Select(delivery => delivery.ToResponse()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DeliveryResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var delivery = await deliveryRepository.GetByIdAsync(id, cancellationToken);
        if (delivery is null) return NotFound();

        return Ok(delivery.ToResponse());
    }

    [HttpPost("{id:guid}/plan")]
    public async Task<ActionResult<DeliveryResponse>> Plan(Guid id, CancellationToken cancellationToken)
    {
        var result = await workflowEngine.TransitionAsync(id, DeliveryState.Planned, cancellationToken);

        await PublishWorkflowResultAsync(result, "Planned", cancellationToken);

        return Ok(result.Delivery.ToResponse());
    }

    [HttpPost("{id:guid}/start")]
    public async Task<ActionResult<DeliveryResponse>> Start(Guid id, CancellationToken cancellationToken)
    {
        var result = await workflowEngine.TransitionAsync(id, DeliveryState.InTransit, cancellationToken);

        if (result.Delivery.AssignedVehicleId is not { } vehicleId)
            return BadRequest(new { error = "Delivery does not have an assigned vehicle." });

        var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle is null) return NotFound(new { error = "Assigned vehicle was not found." });

        vehicle.MarkInTransit();

        await vehicleRepository.UpdateAsync(vehicle, cancellationToken);

        await PublishWorkflowResultAsync(result, "Started", cancellationToken);

        await realtimeUpdatePublisher.PublishVehicleSnapshotAsync(vehicle, "Started", cancellationToken);

        await trackingSimulationService.StartTrackingAsync(result.Delivery.Id, cancellationToken);

        return Ok(result.Delivery.ToResponse());
    }

    [HttpPost("{id:guid}/close")]
    public async Task<ActionResult<DeliveryResponse>> Close(Guid id, CancellationToken cancellationToken)
    {
        var result = await workflowEngine.TransitionAsync(id, DeliveryState.Closed, cancellationToken);
        await PublishWorkflowResultAsync(result, "Closed", cancellationToken);
        if (result.Delivery.AssignedVehicleId is { } vehicleId)
        {
            var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
            if (vehicle is not null)
            {
                vehicle.Release();
                await vehicleRepository.UpdateAsync(vehicle, cancellationToken);
                await realtimeUpdatePublisher.PublishVehicleSnapshotAsync(vehicle, "Released", cancellationToken);
            }
        }

        await trackingSimulationService.StopTrackingAsync(result.Delivery.Id, cancellationToken);
        return Ok(result.Delivery.ToResponse());
    }

    [HttpGet("{id:guid}/events")]
    public async Task<ActionResult<IReadOnlyList<DeliveryEventResponse>>> GetEvents(Guid id,
        CancellationToken cancellationToken)
    {
        var delivery = await deliveryRepository.GetByIdAsync(id, cancellationToken);
        if (delivery is null) return NotFound();

        var events = await eventRepository.GetByDeliveryIdAsync(id, cancellationToken);
        return Ok(events.Select(deliveryEvent => deliveryEvent.ToResponse()).ToList());
    }

    [HttpGet("{id:guid}/route")]
    public async Task<ActionResult<DeliveryRouteResponse>> GetRoute(Guid id, CancellationToken cancellationToken)
    {
        var delivery = await deliveryRepository.GetByIdAsync(id, cancellationToken);
        if (delivery is null) return NotFound();

        var route = await routeRepository.GetByDeliveryIdAsync(id, cancellationToken);
        if (route is null) return NotFound(new { error = "Route has not been generated yet." });

        return Ok(route.ToResponse());
    }

    [HttpPost("{id:guid}/assign-vehicle")]
    public async Task<ActionResult<DeliveryResponse>> AssignVehicle(Guid id, AssignVehicleRequest request,
        CancellationToken cancellationToken)
    {
        if (request.VehicleId == Guid.Empty) return BadRequest(new { error = "VehicleId is required." });

        var delivery = await deliveryRepository.GetByIdAsync(id, cancellationToken);
        if (delivery is null) return NotFound(new { error = "Delivery was not found." });
        if (delivery.State != DeliveryState.Planned)
            return BadRequest(new { error = "Delivery must be planned before assigning a vehicle." });

        var vehicle = await vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);
        if (vehicle is null) return NotFound(new { error = "Vehicle was not found." });
        if (vehicle.Status != VehicleStatus.Available)
            return BadRequest(new { error = "Vehicle is not available." });

        vehicle.AssignToDelivery(delivery.Id);
        delivery.AssignVehicle(vehicle.Id);

        await vehicleRepository.UpdateAsync(vehicle, cancellationToken);
        await deliveryRepository.UpdateAsync(delivery, cancellationToken);

        var result = await workflowEngine.TransitionAsync(delivery.Id, DeliveryState.Assigned, cancellationToken);

        await PublishWorkflowResultAsync(result, "Assigned", cancellationToken);

        await realtimeUpdatePublisher.PublishVehicleSnapshotAsync(vehicle, "Assigned", cancellationToken);

        return Ok(result.Delivery.ToResponse());
    }

    private async Task PublishWorkflowResultAsync(
        WorkflowTransitionResult result,
        string snapshotReason,
        CancellationToken cancellationToken)
    {
        foreach (var deliveryEvent in result.Events)
            await realtimeUpdatePublisher.PublishDeliveryLogEventAsync(deliveryEvent, cancellationToken);

        await realtimeUpdatePublisher.PublishDeliverySnapshotAsync(result.Delivery, snapshotReason, cancellationToken);
    }
}