using LogiFlow.Api.Contracts;
using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Deliveries;
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
    IWorkflowEngine workflowEngine
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Delivery>> Create(CreateDeliveryRequest request, CancellationToken cancellationToken)
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

        return CreatedAtAction(
            nameof(GetById),
            new { id = delivery.Id },
            delivery
        );
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Delivery>>> GetAll(CancellationToken cancellationToken)
    {
        var deliveries = await deliveryRepository.GetAllAsync(cancellationToken);
        return Ok(deliveries);
    }

    [HttpGet("id:guid")]
    public async Task<ActionResult<Delivery>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var delivery = await deliveryRepository.GetByIdAsync(id, cancellationToken);
        if (delivery is null) return NotFound();

        return Ok(delivery);
    }

    [HttpPost("{id:guid}/transition")]
    public async Task<ActionResult<Delivery>> Transition(Guid id, TransitionDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await workflowEngine.TransitionAsync(id, request.TargetState, cancellationToken);
            return Ok(result.Delivery);
        }
        catch (DeliveryNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidWorkflowTransitionException exception)
        {
            return BadRequest(new
            {
                error = exception.Message,
                exception.CurrentState,
                exception.TargetState
            });
        }
    }

    [HttpGet("{id:guid}/events")]
    public async Task<ActionResult<IReadOnlyList<DeliveryEvent>>> GetEvents(Guid id,
        CancellationToken cancellationToken)
    {
        var delivery = await deliveryRepository.GetByIdAsync(id, cancellationToken);
        if (delivery is null) return NotFound();

        var events = await eventRepository.GetByDeliveryIdAsync(id, cancellationToken);
        return Ok(events);
    }
}