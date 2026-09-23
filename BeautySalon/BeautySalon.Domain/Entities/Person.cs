using BeautySalon.Domain.Enums;

namespace BeautySalon.Domain.Entities;

public abstract class Person
{
    public required int Id { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? Patronymic { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required Gender Gender { get; set; }
    public required string Phone { get; set; }
}
