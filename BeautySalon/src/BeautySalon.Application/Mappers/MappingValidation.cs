using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Application.Mappers;

/// <summary>
/// Класс проверки обязательных значений
/// </summary>
internal static class MappingValidation
{
    /// <summary>
    /// Возвращает обязательную строку
    /// </summary>
    public static string Required(string? value, string fieldName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ValidationException($"Поле {fieldName} обязательно.")
            : value.Trim();

    /// <summary>
    /// Возвращает обязательное значение или сообщает об ошибке
    /// </summary>
    public static T Required<T>(T? value, string fieldName) where T : struct =>
        value ?? throw new ValidationException($"Поле {fieldName} обязательно.");
}