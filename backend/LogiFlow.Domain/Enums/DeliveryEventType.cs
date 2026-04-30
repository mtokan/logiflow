namespace LogiFlow.Domain.Enums;

public enum DeliveryEventType
{
    DeliveryCreated = 1,
    StateChanged = 2,
    RouteGenerated = 3,
    TrackingStarted = 4,
    PositionUpdated = 5,
    EtaUpdated = 6,
    TrafficChanged = 7,
    TrackingStopped = 8,
    DeliveryClosed = 9
}