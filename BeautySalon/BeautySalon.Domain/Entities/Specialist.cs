namespace BeautySalon.Domain.Entities;

/// <summary>
/// Класс мастера
/// </summary>
public class Specialist : Person
{
    /// <summary>
    /// Номер паспорта
    /// </summary>
    public string? PassportNumber { get; set; }

    /// <summary>
    /// Специализация
    /// </summary>
    public required string Specialization { get; set; }

    /// <summary>
    /// Стаж работы
    /// </summary>
    public required int ExperienceYears { get; set; }
}
