using Application.Features.HealthСertificatates.Handlers;
using Application.Features.HealthСertificatates.Queries;
using Application.Tests.Utility;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Entities.Services;
using Domain.Interfaces;
using JetBrains.Annotations;
using Moq;

namespace Application.Tests.Features.HealthCertificates;

[TestSubject(typeof(GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler))]
public class GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandlerTests
{
    private readonly int _clientId = 1;
    private readonly int _nonExistingClientId = 2;
    private readonly int _healthCertificateTemplateId = 10;
    private readonly int _nonExistingHealthCertificateTemplateId = 20;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Mock<IApplicationDbContext> _mockDbContext = new();

    private readonly Client _client;
    private readonly HealthCertificateTemplate _template;
    private readonly HealthCertificate _healthCertificate;
    private readonly HealthCertificate _olderHealthCertificate;

    public GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandlerTests()
    {
        _client = new Client
        {
            Id = _clientId,
            Birthday = new DateOnly(1990, 1, 1),
            Gender = "Male",
            PersonId = 100
        };

        _template = new HealthCertificateTemplate
        {
            Id = _healthCertificateTemplateId,
            Name = "Test Template",
        };

        _healthCertificate = new HealthCertificate
        {
            Id = 1,
            ClientId = _clientId,
            TemplateId = _healthCertificateTemplateId,
            ReceivingTime = new DateOnly(2024, 6, 1),
            Description = "Test Description"
        };

        _olderHealthCertificate = new HealthCertificate
        {
            Id = 2,
            ClientId = _clientId,
            TemplateId = _healthCertificateTemplateId,
            ReceivingTime = new DateOnly(2024, 1, 1),
            Description = "Test Description"
        };
    }

    [Fact]
    public async Task Test_Handle_ReturnsLatestCertificate_WhenClientAndTemplateExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { _clientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.HealthCertificateTemplates.FindAsync(new object[] { _healthCertificateTemplateId }, _cancellationToken))
            .ReturnsAsync(_template);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate> { _healthCertificate }));

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            _clientId,
            _healthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.HealthCertificate);
        
        var healthCertificate = result.Value.HealthCertificate;
        
        Assert.Equal(_healthCertificate.Id, healthCertificate.Id);
        Assert.Equal(_clientId, healthCertificate.ClientId);
        Assert.Equal(_healthCertificateTemplateId, healthCertificate.TemplateId);
        Assert.Equal(_healthCertificate.ReceivingTime, healthCertificate.ReceivingTime);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { _nonExistingClientId }, _cancellationToken))
            .ReturnsAsync((Client?)null);

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            _nonExistingClientId,
            _healthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ClientErrors.NotFoundById(_nonExistingClientId), result.Error);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenTemplateDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { _clientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.HealthCertificateTemplates.FindAsync(new object[] { _nonExistingHealthCertificateTemplateId }, _cancellationToken))
            .ReturnsAsync((HealthCertificateTemplate?)null);

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            _clientId,
            _nonExistingHealthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HealthCertificateTemplateErrors.NotFoundById(_nonExistingHealthCertificateTemplateId), result.Error);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNull_WhenCertificateDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { _clientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.HealthCertificateTemplates.FindAsync(new object[] { _healthCertificateTemplateId }, _cancellationToken))
            .ReturnsAsync(_template);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate>()));

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            _clientId,
            _healthCertificateTemplateId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.HealthCertificate);
    }

    [Fact]
    public async Task Test_Handle_ReturnsLatestCertificate_WhenMultipleCertificatesExist()
    {
        _mockDbContext.Setup(db => db.Clients.FindAsync(new object[] { _clientId }, _cancellationToken))
            .ReturnsAsync(_client);

        _mockDbContext.Setup(db => db.HealthCertificateTemplates.FindAsync(new object[] { _healthCertificateTemplateId }, _cancellationToken))
            .ReturnsAsync(_template);

        _mockDbContext.Setup(db => db.HealthCertificates)
            .Returns(MockDbSetUtility.MockDbSet(new List<HealthCertificate> { _healthCertificate, _olderHealthCertificate }));

        var handler = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQueryHandler(_mockDbContext.Object);

        var request = new GetLatestHealthCertificateByClientIdAndHealthCertificateTemplateIdQuery(
            _clientId,
            _healthCertificateTemplateId);

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