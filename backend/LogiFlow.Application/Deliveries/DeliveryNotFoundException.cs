namespace LogiFlow.Application.Deliveries;

public class DeliveryNotFoundException(Guid deliveryId) : Exception($"Delivery with id '{deliveryId}' was not found.")
{
    public Guid DeliveryId { get; } = deliveryId;
}