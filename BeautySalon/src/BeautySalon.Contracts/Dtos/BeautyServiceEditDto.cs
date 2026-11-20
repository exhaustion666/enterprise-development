using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс услуги
/// </summary>
public sealed class BeautyServiceEditDto
{
    /// <summary>
    /// Наименование
    /// </summary>
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string? Name { get; set; }

    /// <summary>
    /// Категория
    /// </summary>
    [Required]
    [StringLength(80, MinimumLength = 1)]
    public string? Category { get; set; }

    /// <summary>
    /// Цена
    /// </summary>
    [Required]
    [Range(typeof(decimal), "1", "1000000")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Длительность
    /// </summary>
    [Required]
    [Range(1, 1440)]
    public int? DurationMinutes { get; set; }
}