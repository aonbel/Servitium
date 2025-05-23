using Application.Features.HealthСertificatates.Handlers;
using Application.Features.HealthСertificatates.Queries;
using Application.Tests.Utility;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Entities.Services;
using Domain.Interfaces;
using JetBrains.Annotations;
using Moq;

namespace Application.Tests.Features.HealthCertificates.Handlers;

[TestSubject(typeof(GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler))]
public class GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandlerTests
{
    private const int ClientId = 1;
    private const int NonExistingClientId = 2;
    private const int HealthCertificateTemplateId = 10;
    private const int NonExistingHealthCertificateTemplateId = 20;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Mock<IApplicationDbContext> _mockDbContext = new();

    private readonly Client _client = new()
    {
        Id = ClientId,
        Birthday = new DateOnly(1990, 1, 1),
        Gender = "Male",
        PersonId = 100
    };
    private readonly HealthCertificateTemplate _template = new()
    {
        Id = HealthCertificateTemplateId,
        Name = "Test Template",
    };
    private readonly HealthCertificate _healthCertificate = new()
    {
        Id = 1,
        ClientId = ClientId,
        TemplateId = HealthCertificateTemplateId,
        ReceivingTime = new DateOnly(2024, 6, 1),
        Description = "Test Description"
    };
    private readonly HealthCertificate _olderHealthCertificate = new()
    {
        Id = 2,
        ClientId = ClientId,
        TemplateId = HealthCertificateTemplateId,
        ReceivingTime = new DateOnly(2024, 1, 1),
        Description = "Test Description"
    };

    [Fact]
    public async Task Test_Handle_ReturnsLatestCertificate_WhenClientAndTemplateExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.HealthCertificateTemplates.FindAsync(new object[] { HealthCertificateTemplateId }, _cancellationToken))
            .ReturnsAsync(_template);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate> { _healthCertificate }));

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            ClientId,
            HealthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.HealthCertificate);
        
        var healthCertificate = result.Value.HealthCertificate;
        
        Assert.Equal(_healthCertificate.Id, healthCertificate.Id);
        Assert.Equal(ClientId, healthCertificate.ClientId);
        Assert.Equal(HealthCertificateTemplateId, healthCertificate.TemplateId);
        Assert.Equal(_healthCertificate.ReceivingTime, healthCertificate.ReceivingTime);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { NonExistingClientId }, _cancellationToken))
            .ReturnsAsync((Client?)null);

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            NonExistingClientId,
            HealthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ClientErrors.NotFoundById(NonExistingClientId), result.Error);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenTemplateDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.HealthCertificateTemplates.FindAsync(new object[] { NonExistingHealthCertificateTemplateId }, _cancellationToken))
            .ReturnsAsync((HealthCertificateTemplate?)null);

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            ClientId,
            NonExistingHealthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HealthCertificateTemplateErrors.NotFoundById(NonExistingHealthCertificateTemplateId), result.Error);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNull_WhenCertificateDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.HealthCertificateTemplates.FindAsync(new object[] { HealthCertificateTemplateId }, _cancellationToken))
            .ReturnsAsync(_template);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate>()));

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            ClientId,
            HealthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.HealthCertificate);
    }

    [Fact]
    public async Task Test_Handle_ReturnsLatestCertificate_WhenMultipleCertificatesExist()
    {
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { ClientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.HealthCertificateTemplates.FindAsync(new object[] { HealthCertificateTemplateId }, _cancellationToken))
            .ReturnsAsync(_template);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate> { _healthCertificate, _olderHealthCertificate }));

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            ClientId,
            HealthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.HealthCertificate);
        
        var healthCertificate = result.Value.HealthCertificate;
        
        Assert.Equal(_healthCertificate.Id, healthCertificate.Id);
        Assert.Equal(_healthCertificate.ReceivingTime, healthCertificate.ReceivingTime);
    }
}