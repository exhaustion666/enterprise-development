namespace BeautySalon.Domain.Exceptions;

/// <summary>
/// Исключение при попытке выполнить операцию, нарушающую связи сущностей
/// </summary>
public sealed class EntityConflictException(string message) : Exception(message);