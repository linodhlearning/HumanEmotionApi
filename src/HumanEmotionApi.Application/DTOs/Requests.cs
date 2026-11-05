using System.ComponentModel.DataAnnotations;
using HumanEmotionApi.Domain.Enums;

namespace HumanEmotionApi.Application.DTOs;

public record CreateHumanRequest(
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Name
);

public record UpdateHumanRequest(
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Name
);

public record AddEmotionRequest(
    [Required]
    EmotionType Type,

    [Required]
    [Range(0.0, 100.0, ErrorMessage = "Percentage must be between 0.0 and 100.0.")]
    double Percentage
);

public record UpdateEmotionRequest(
    [Required]
    [Range(0.0, 100.0, ErrorMessage = "Percentage must be between 0.0 and 100.0.")]
    double Percentage
);

