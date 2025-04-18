﻿using Domain.Models.Entities.Services;

namespace Domain.Models.Entities.People;

public class Client : Person
{
    public required DateOnly Birthday { get; set; }
    public required string Gender { get; set; }
    
    public ICollection<HealthCertificate> ServiceResults { get; set; } = [];
}