using System;
using System.Collections.Generic;
using System.Text;

namespace BeautySalon.Domain.Models;

public class TimeSlot
{
    public required DateTimeOffset StartAt { get; set; }
    public required DateTimeOffset EndAt { get; set; }
    public TimeSpan Duration => EndAt - StartAt;
}
