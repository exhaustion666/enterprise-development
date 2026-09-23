using System;
using System.Collections.Generic;
using System.Text;

namespace BeautySalon.Domain.Entities;

public class Booking
{
    public required int Id { get; set; }
    public required DateTimeOffset StartAt { get; set; }
    public required int SpecialistId { get; set; }
    public Specialist? Specialist { get; set; }
    public required int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public required int BeautyServiceId { get; set; }
    public BeautyService? BeautyService { get; set; }
    public required bool IsRegularCustomer { get; set; }
}
