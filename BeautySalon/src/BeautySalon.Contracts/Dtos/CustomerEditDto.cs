using BeautySalon.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс клиента
/// </summary>
public sealed class CustomerEditDto
{
    /// <summary>
    /// Фамилия
    /// </summary>
    [Required]
    [StringLength(80, MinimumLength = 1)]
    public string? LastName { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    [Required]
    [StringLength(80, MinimumLength = 1)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Отчество
    /// </summary>
    [StringLength(80)]
    public string? Patronymic { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    [Required]
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// Пол
    /// </summary>
    [Required]
    [EnumDataType(typeof(Gender))]
    public Gender? Gender { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
    [Required]
    [Phone]
    [StringLength(32)]
    public string? Phone { get; set; }
}