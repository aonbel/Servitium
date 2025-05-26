using Application.Features.Appointments.Handlers;
using Application.Features.Appointments.Queries;
using Application.Tests.Utility;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Core;
using Domain.Entities.People;
using Domain.Entities.Services;
using Domain.Interfaces;
using JetBrains.Annotations;
using Moq;

namespace Application.Tests.Features.Appointments.Handlers;

[TestSubject(typeof(GetAppointmentBySpecialistIdAndDateTimeQueryHandler))]
public class GetAppointmentBySpecialistIdAndDateTimeQueryHandlerTest
{
    private readonly int _specialistId = 1;
    private readonly DateOnly _appointmentDate = new DateOnly(2020, 1, 1);

    private readonly TimeOnlySegment _appointmentTimeOnlySegment = new TimeOnlySegment(
        new TimeOnly(4, 0),
        new TimeOnly(4, 30)
    );

    private readonly DateTime _dateTimeInAppointment = new DateTime(2020, 1, 1, 4, 20, 00);
    private readonly DateTime _dateTimeNotInAppointment = new DateTime(2020, 1, 1, 4, 35, 00);

    private readonly Specialist _specialist;
    private readonly Appointment _appointment;
    private readonly Appointment _wrongAppointment;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly Mock<IApplicationDbContext> _mock = new();
    private readonly GetAppointmentBySpecialistIdAndDateTimeQueryHandler _handler;

    public GetAppointmentBySpecialistIdAndDateTimeQueryHandlerTest()
    {
        _specialist = new Specialist
        {
            Id = _specialistId,
            Contacts = [],
            Location = "Test Location",
            PersonId = 1,
            PricePerHour = 0,
            ServiceIds = [],
            ServiceProviderId = 1,
            WorkDays = [],
            WorkTime = new TimeOnlySegment()
        };

        _appointment = new Appointment
        {
            Id = 1,
            SpecialistId = _specialistId,
            ServiceId = 1,
            ClientId = 1,
            Date = _appointmentDate,
            TimeSegment = _appointmentTimeOnlySegment,
        };

        _wrongAppointment = new Appointment
        {
            Id = 2,
            SpecialistId = _specialistId,
            ServiceId = 2,
            ClientId = 2,
            Date = _appointmentDate.AddDays(1),
            TimeSegment = _appointmentTimeOnlySegment,
        };

        _handler = new GetAppointmentBySpecialistIdAndDateTimeQueryHandler(_mock.Object);
    }

    [Fact]
    public async Task Test_Handle_ReturnsAppointment_WhenAppointmentExistsForSpecialistAndDateTime()
    {
        // Arrange

        _mock.Setup(db => db.Specialists.FindAsync(new object[] { _specialistId }, _cancellationToken))
            .ReturnsAsync(_specialist);

        _mock.Setup(db => db.Appointments)
            .Returns(MockDbSetUtility.MockDbSet([_appointment, _wrongAppointment]));

        var query = new GetAppointmentBySpecialistIdAndDateTimeQuery(_specialistId, _dateTimeInAppointment);

        // Act
        
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        
        Assert.NotNull(result.Value.Appointment);
        var appointment = result.Value.Appointment;
        
        Assert.Equal(_appointment.Id, appointment.Id);
        Assert.Equal(_appointment.Date, appointment.Date);
        Assert.Equal(_appointment.TimeSegment, appointment.TimeSegment);
        Assert.Equal(_appointment.ClientId, appointment.ClientId);
        Assert.Equal(_appointment.ServiceId, appointment.ServiceId);
        Assert.Equal(_appointment.SpecialistId, appointment.SpecialistId);
    }
    
    [Fact]
    public async Task Test_Handle_ReturnsNullAppointment_WhenAppointmentDoNotExistForSpecialistAndDateTime()
    {
        // Arrange

        _mock.Setup(db => db.Specialists.FindAsync(new object[] { _specialistId }, _cancellationToken))
            .ReturnsAsync(_specialist);

        _mock.Setup(db => db.Appointments)
            .Returns(MockDbSetUtility.MockDbSet([_appointment, _wrongAppointment]));

        var query = new GetAppointmentBySpecialistIdAndDateTimeQuery(_specialistId, _dateTimeNotInAppointment);

        // Act
        
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        
        Assert.Null(result.Value.Appointment);
    }
    
    [Fact]
    public async Task Test_Handle_ReturnsSpecialistError_WhenSpecialistDoesNotExist()
    {
        // Arrange

        _mock.Setup(db => db.Specialists.FindAsync(new object[] { _specialistId }, _cancellationToken))
            .ReturnsAsync((Specialist?)null);
        
        var query = new GetAppointmentBySpecialistIdAndDateTimeQuery(_specialistId, _dateTimeInAppointment);

        // Act
        
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        
        Assert.True(result.IsError);
        
        Assert.Equal(SpecialistErrors.NotFoundById(_specialistId), result.Error);
    }
}