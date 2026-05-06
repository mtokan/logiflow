using System.ComponentModel.DataAnnotations;

namespace LogiFlow.Api.Contracts;

public sealed record CreateVehicleRequest(
    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Name
);