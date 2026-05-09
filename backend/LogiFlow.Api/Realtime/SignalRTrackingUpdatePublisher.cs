using LogiFlow.Api.Hubs;
using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Tracking;
using Microsoft.AspNetCore.SignalR;

namespace LogiFlow.Api.Realtime;

public sealed class SignalRTrackingUpdatePublisher(
    IHubContext<TrackingHub> hubContext)
    : ITrackingUpdatePublisher
{
    public Task PublishDeliveryPositionUpdatedAsync(DeliveryPositionUpdated update,
        CancellationToken cancellationToken = default)
    {
        return hubContext.Clients.All.SendAsync(
            "DeliveryPositionUpdated",
            update,
            cancellationToken);
    }
}