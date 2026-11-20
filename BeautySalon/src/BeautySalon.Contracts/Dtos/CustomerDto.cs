using BeautySalon.Domain.Enums;

namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс клиента
/// </summary>
public sealed class CustomerDto : IEntityDto
{
    /// <summary>
    /// Уникальный идентификатор клиента
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
    /// Телефон
    /// </summary>
    public required string Phone { get; set; }
}