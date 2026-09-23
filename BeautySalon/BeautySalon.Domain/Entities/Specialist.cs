namespace BeautySalon.Domain.Entities;

public class Specialist : Person
{
    public string? PassportNumber { get; set; }
    public required string Specialization { get; set; }
    public required int ExperienceYears { get; set; }
}
