﻿using Domain.Models.Entities.Core;
using Domain.Models.Entities.People;

namespace Domain.Models.Entities.Services;

public class Appointment
{
    public required Client Client { get; set; }
    public required Specialist Specialist { get; set; }
    public required Service Service { get; set; }
    
    public required DateOnly Date { get; set; }
    public required TimeOnlySegment TimeSegment { get; set; }
}