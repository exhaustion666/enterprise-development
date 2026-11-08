using BeautySalon.Domain.Enums;

namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс мастера
/// </summary>
public sealed class SpecialistDto : IEntityDto
{
    /// <summary>
    /// Уникальный идентификатор специалиста
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Имя
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; init; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Пол
    /// </summary>
    public required Gender Gender { get; init; }

    /// <summary>
    /// Телефон
    /// </summary>
    public required string Phone { get; init; }

    /// <summary>
    /// Номер паспорта
    /// </summary>
    public string? PassportNumber { get; init; }

    /// <summary>
    /// Специализация
    /// </summary>
    public required string Specialization { get; init; }

    /// <summary>
    /// Стаж работы
    /// </summary>
    public required int ExperienceYears { get; init; }
}