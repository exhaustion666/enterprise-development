namespace BeautySalon.Contracts.Dtos;

/// <summary>
/// Класс для "окошек" мастера
/// </summary>
public sealed class TimeSlotDto
{
    /// <summary>
    /// Начало "окошка"
    /// </summary>
    public required DateTimeOffset StartAt { get; set; }

    /// <summary>
    /// Конец "окошка"
    /// </summary>
    public required DateTimeOffset EndAt { get; set; }

    /// <summary>
    /// Длительность "окошка"
    /// </summary>
    public required TimeSpan Duration { get; set; }
}