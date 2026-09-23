namespace BeautySalon.Domain.Entities;

public class BeautyService
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required decimal Price { get; set; }
    public required TimeSpan Duration { get; set; }
}
