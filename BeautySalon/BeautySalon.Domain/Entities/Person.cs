using BeautySalon.Domain.Enums;

namespace BeautySalon.Domain.Entities;

/// <summary>
/// Базовый класс человека
/// </summary>
public abstract class Person
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public required DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Пол
    /// </summary>
    public required Gender Gender { get; set; }

    /// <summary>
    /// Номер телефона
    /// </summary>
    public required string Phone { get; set; }
}
