using BeautySalon.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс мастера
/// </summary>
public sealed class SpecialistEditDto
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

    /// <summary>
    /// Номер паспорта
    /// </summary>
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Номер паспорта должен содержать 10 цифр")]
    public string? PassportNumber { get; init; }

    /// <summary>
    /// Специализация
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? Specialization { get; init; }

    /// <summary>
    /// Стаж работы
    /// </summary>
    [Required]
    [Range(0, 80)]
    public int? ExperienceYears { get; init; }
}