using BeautySalon.Contracts.Dtos;
using BeautySalon.Domain.Entities;

namespace BeautySalon.Application.Mappers;

/// <summary>
/// Маппер специалистов
/// </summary>
public sealed class SpecialistMapper : IEntityMapper<Specialist, SpecialistDto, SpecialistEditDto>
{
    /// <summary>
    /// Преобразует специалиста в DTO
    /// </summary>
    public SpecialistDto ToDto(Specialist entity)
    {
        return new()
        {
            Id = entity.Id,
            LastName = entity.LastName,
            FirstName = entity.FirstName,
            Patronymic = entity.Patronymic,
            DateOfBirth = entity.DateOfBirth,
            Gender = entity.Gender,
            Phone = entity.Phone,
            PassportNumber = entity.PassportNumber,
            Specialization = entity.Specialization,
            ExperienceYears = entity.ExperienceYears
        };
    }

    /// <summary>
    /// Создаёт доменного специалиста из входного DTO
    /// </summary>
    public Specialist ToEntity(SpecialistEditDto dto, int id)
    {
        return new()
        {
            Id = id,
            LastName = MappingValidation.Required(dto.LastName, nameof(dto.LastName)),
            FirstName = MappingValidation.Required(dto.FirstName, nameof(dto.FirstName)),
            Patronymic = dto.Patronymic,
            DateOfBirth = MappingValidation.Required(dto.DateOfBirth, nameof(dto.DateOfBirth)),
            Gender = MappingValidation.Required(dto.Gender, nameof(dto.Gender)),
            Phone = MappingValidation.Required(dto.Phone, nameof(dto.Phone)),
            PassportNumber = dto.PassportNumber,
            Specialization = MappingValidation.Required(dto.Specialization, nameof(dto.Specialization)),
            ExperienceYears = MappingValidation.Required(dto.ExperienceYears, nameof(dto.ExperienceYears))
        };
    }
}