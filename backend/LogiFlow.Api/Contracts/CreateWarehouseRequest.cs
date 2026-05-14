using System.ComponentModel.DataAnnotations;

namespace LogiFlow.Api.Contracts;

public sealed record CreateWarehouseRequest(
    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Name,
    [Range(-90, 90)] double Latitude,
    [Range(-180, 180)] double Longitude
);