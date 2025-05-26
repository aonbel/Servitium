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

[TestSubject(typeof(CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandler))]
public class CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandlerTest
{
    private const int ClientId = 1;
    private const int NonExistingClientId = 2;
    private const int ServiceId = 10;
    private const int NonExistingServiceId = 20;
    private const int RequiredHealthCertificateTemplateId = 100;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Mock<IApplicationDbContext> _mockDbContext = new();

    private readonly Client _client = new()
    {
        Id = ClientId,
        Birthday = new DateOnly(1990, 1, 1),
        Gender = "Male",
        PersonId = 100
    };

    private readonly Service _service = new()
    {
        Id = ServiceId,
        Name = "Test Service",
        ShortName = "Test Service",
        Description = "Test Service",
        PricePerHourForEquipment = 0,
        PricePerHourForMaterials = 0,
        Duration = new TimeSpan(0,0,0),
        RequiredHealthCertificateTemplateIds = new List<int> { RequiredHealthCertificateTemplateId },
        ResultHealthCertificateTemplateIds = new List<int>()
    };

    private readonly HealthCertificate _healthCertificate = new()
    {
        Id = 1,
        ClientId = ClientId,
        TemplateId = RequiredHealthCertificateTemplateId,
        ReceivingTime = new DateOnly(2024, 6, 1),
        Description = "Test Certificate"
    };

    private readonly Appointment _appointmentInFuture = new()
    {
        Id = 1,
        ClientId = ClientId,
        ServiceId = 11,
        SpecialistId = 200,
        Date = DateOnly.FromDateTime(DateTime.Now).AddDays(10),
        TimeSegment = new TimeOnlySegment(new TimeOnly(10, 0), new TimeOnly(11, 0))
    };

    private readonly Service _serviceFromAppointment = new()
    {
        Id = 11,
        Name = "Previous Service",
        ShortName = "Test Service",
        Description = "Test Service",
        PricePerHourForEquipment = 0,
        PricePerHourForMaterials = 0,
        Duration = new TimeSpan(0,0,0),
        RequiredHealthCertificateTemplateIds = new List<int>(),
        ResultHealthCertificateTemplateIds = new List<int> { RequiredHealthCertificateTemplateId }
    };

    [Fact]
    public async Task Test_Handle_ReturnsCanCreateTrue_WhenAllCertificatesExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.Services.FindAsync(new object[] { ServiceId }, _cancellationToken))
            .ReturnsAsync(_service);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet([ _healthCertificate ]));

        _mockDbContext.Setup(db => db.Appointments)
            .Returns(MockDbSetUtility.MockDbSet(new List<Appointment>()));

        var handler =
            new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandler(_mockDbContext
                .Object);

        var request = new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(
            ClientId,
            ServiceId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.CanCreate);
        Assert.NotNull(result.Value.MinDateTime);
        Assert.True(result.Value.MinDateTime >= DateTime.Now.AddMinutes(-1)); 
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { NonExistingClientId }, _cancellationToken))
            .ReturnsAsync((Client?)null);

        var handler =
            new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandler(_mockDbContext
                .Object);

        var request = new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(
            NonExistingClientId,
            ServiceId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ClientErrors.NotFoundById(NonExistingClientId), result.Error);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenServiceDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.Services.FindAsync(new object[] { NonExistingServiceId }, _cancellationToken))
            .ReturnsAsync((Service?)null);

        var handler =
            new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandler(_mockDbContext
                .Object);

        var request = new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(
            ClientId,
            NonExistingServiceId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrors.NotFoundById(NonExistingServiceId), result.Error);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenServiceFromAppointmentDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.Services.FindAsync(new object[] { ServiceId }, _cancellationToken))
            .ReturnsAsync(_service);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate>()));

        _mockDbContext.Setup(db => db.Appointments)
            .Returns(MockDbSetUtility.MockDbSet(new List<Appointment> { _appointmentInFuture }));

        _mockDbContext.Setup(db => db.Services.FindAsync(new object[] { _appointmentInFuture.ServiceId }, _cancellationToken))
            .ReturnsAsync((Service?)null);

        var handler =
            new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandler(_mockDbContext
                .Object);

        var request = new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(
            ClientId,
            ServiceId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrors.NotFoundById(_appointmentInFuture.ServiceId), result.Error);
    }

    [Fact]
    public async Task Test_Handle_ReturnsCanCreateTrueWithMinDateTime_WhenCertificatesMissingButAppointmentsExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.Services.FindAsync(new object[] { ServiceId }, _cancellationToken))
            .ReturnsAsync(_service);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate>()));

        _mockDbContext.Setup(db => db.Appointments)
            .Returns(MockDbSetUtility.MockDbSet(new List<Appointment> { _appointmentInFuture }));

        _mockDbContext.Setup(db => db.Services.FindAsync(new object[] { _appointmentInFuture.ServiceId }, _cancellationToken))
            .ReturnsAsync(_serviceFromAppointment);

        var handler =
            new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandler(_mockDbContext
                .Object);

        var request = new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(
            ClientId,
            ServiceId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.CanCreate);
        Assert.NotNull(result.Value.MinDateTime);
        var expectedMinDateTime = _appointmentInFuture.Date.ToDateTime(_appointmentInFuture.TimeSegment.End);
        Assert.Equal(expectedMinDateTime, result.Value.MinDateTime);
    }

    [Fact]
    public async Task Test_Handle_ReturnsCanCreateFalse_WhenCertificatesMissingAndNoSuitableAppointments()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.Services.FindAsync(new object[] { ServiceId }, _cancellationToken))
            .ReturnsAsync(_service);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate>()));

        _mockDbContext.Setup(db => db.Appointments)
            .Returns(MockDbSetUtility.MockDbSet(new List<Appointment>()));

        var handler =
            new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandler(_mockDbContext
                .Object);

        var request = new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(
            ClientId,
            ServiceId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(result.Value.CanCreate);
        Assert.Null(result.Value.MinDateTime);
    }
}