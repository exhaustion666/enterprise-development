using System;
using System.Collections.Generic;
using System.Text;

namespace BeautySalon.Domain.Models;

public class PopularService
{
    public required int BeautyServiceId { get; set; }
    public required int BookingCount { get; set; }
}
