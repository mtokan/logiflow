using LogiFlow.Application.Workflow;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.UnitTests.Workflow;

public sealed class WorkflowEngineTests
{
    [Fact]
    public async Task TransitionAsync_WhenCreatedToPlanned_GeneratesRoute()
    {
        var delivery = CreateDelivery();
        var context = CreateTestContext(delivery);

        var result = await context.WorkflowEngine.TransitionAsync(delivery.Id, DeliveryState.Planned);

        Assert.Equal(DeliveryState.Planned, result.Delivery.State);
        Assert.NotNull(result.Delivery.RouteId);
        Assert.Contains(result.Events, e => e.Type == DeliveryEventType.RouteGenerated);
    }

    [Fact]
    public async Task TransitionAsync_WhenPlannedToAssignedWithoutVehicle_Throws()
    {
        var delivery = CreateDelivery();
        delivery.UpdateState(DeliveryState.Planned);

        var context = CreateTestContext(delivery);

        var exception = await Assert.ThrowsAsync<InvalidWorkflowTransitionException>(() =>
            context.WorkflowEngine.TransitionAsync(delivery.Id, DeliveryState.Assigned));

        Assert.Equal(DeliveryState.Planned, exception.CurrentState);
        Assert.Equal(DeliveryState.Assigned, exception.TargetState);
    }

    [Fact]
    public async Task TransitionAsync_WhenPlannedToAssignedWithVehicle_UpdatesState()
    {
        var delivery = CreateDelivery();
        delivery.UpdateState(DeliveryState.Planned);
        delivery.AssignVehicle(Guid.NewGuid());

        var context = CreateTestContext(delivery);

        var result = await context.WorkflowEngine.TransitionAsync(delivery.Id, DeliveryState.Assigned);

        Assert.Equal(DeliveryState.Assigned, result.Delivery.State);
        Assert.Contains(result.Events, e => e.Type == DeliveryEventType.StateChanged);
    }

    [Fact]
    public async Task TransitionAsync_WhenInvalidTransition_Throws()
    {
        var delivery = CreateDelivery();

        var context = CreateTestContext(delivery);

        var exception = await Assert.ThrowsAsync<InvalidWorkflowTransitionException>(() =>
            context.WorkflowEngine.TransitionAsync(delivery.Id, DeliveryState.InTransit));

        Assert.Equal(DeliveryState.Created, exception.CurrentState);
        Assert.Equal(DeliveryState.InTransit, exception.TargetState);
    }

    private static Delivery CreateDelivery()
    {
        return new Delivery
        {
            Code = "DEL-001",
            Origin = new GeoPoint(52.520008, 13.404954),
            Destination = new GeoPoint(52.5170365, 13.3888599)
        };
    }

    private static TestContext CreateTestContext(Delivery delivery)
    {
        var deliveryRepository = new FakeDeliveryRepository(delivery);
        var routeRepository = new FakeDeliveryRouteRepository();
        var routingService = new FakeRoutingService();
        var eventRepository = new FakeDeliveryEventRepository();


        var workflowEngine = new WorkflowEngine(deliveryRepository, routingService, eventRepository, routeRepository);

        return new TestContext(workflowEngine);
    }

    private sealed record TestContext(
        WorkflowEngine WorkflowEngine
    );
}