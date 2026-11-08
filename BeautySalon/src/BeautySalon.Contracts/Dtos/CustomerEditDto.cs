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
    public string? LastName { get; init; }

    /// <summary>
    /// Имя
    /// </summary>
    [Required]
    [StringLength(80, MinimumLength = 1)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Отчество
    /// </summary>
    [StringLength(80)]
    public string? Patronymic { get; init; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    [Required]
    public DateOnly? DateOfBirth { get; init; }

    /// <summary>
    /// Пол
    /// </summary>
    [Required]
    [EnumDataType(typeof(Gender))]
    public Gender? Gender { get; init; }

    /// <summary>
    /// Телефон
    /// </summary>
    [Required]
    [Phone]
    [StringLength(32)]
    public string? Phone { get; init; }
}