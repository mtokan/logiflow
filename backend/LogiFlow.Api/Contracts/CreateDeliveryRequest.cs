using System.ComponentModel.DataAnnotations;

namespace LogiFlow.Api.Contracts;

public sealed record CreateDeliveryRequest(
    [Required]
    [StringLength(50, MinimumLength = 3)]
    string Code,
    [Required] Guid WarehouseId,
    [Range(-90, 90)] double DestinationLatitude,
    [Range(-180, 180)] double DestinationLongitude
);