namespace BeautySalon.Domain.Models;

/// <summary>
/// Класс для "окошек" мастера
/// </summary>
public class TimeSlot
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
    public TimeSpan Duration => EndAt - StartAt;
}
