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

    /// <summary>
    /// Номер паспорта
    /// </summary>
    [Required(ErrorMessage = "Номер паспорта обязателен")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Номер паспорта должен содержать 10 цифр")]
    public string? PassportNumber { get; set; }

    /// <summary>
    /// Специализация
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? Specialization { get; set; }

    /// <summary>
    /// Стаж работы
    /// </summary>
    [Required]
    [Range(0, 80)]
    public int? ExperienceYears { get; set; }
}