using Application.Features.Services.Handlers;
using Application.Features.Services.Queries;
using Application.Tests.Utility;
using Domain.Entities.Services;
using Domain.Interfaces;
using JetBrains.Annotations;
using Moq;

namespace Application.Tests.Features.Services.Handlers;

[TestSubject(typeof(GetServicesByResultTemplateIdQueryHandler))]
public class GetServicesByResultTemplateIdQueryHandlerTest
{
    private const int NeededHealthCertificateTemplateId = 4;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Service _firstServiceWithNeededHealthCertificateTemplateId = new()
    {
        ShortName = "Test Name",
        Description = "Test Name",
        RequiredHealthCertificateTemplateIds = [1, 2, 3],
        ResultHealthCertificateTemplateIds = [NeededHealthCertificateTemplateId, 5, 6],
        PricePerHourForMaterials = 0,
        PricePerHourForEquipment = 0,
        Duration = TimeSpan.Zero,
        Name = "Test Name",
        Id = 1
    };

    private readonly Service _secondServiceWithNeededHealthCertificateTemplateId = new()
    {
        ShortName = "Test Name",
        Description = "Test Name",
        RequiredHealthCertificateTemplateIds = [1, 2, 3],
        ResultHealthCertificateTemplateIds = [NeededHealthCertificateTemplateId, 5, 6],
        PricePerHourForMaterials = 0,
        PricePerHourForEquipment = 0,
        Duration = TimeSpan.Zero,
        Name = "Test Name",
        Id = 2
    };

    private readonly Service _serviceWithoutNeededHealthCertificateTemplateId = new()
    {
        ShortName = "Test Name",
        Description = "Test Name",
        RequiredHealthCertificateTemplateIds = [2, 3],
        ResultHealthCertificateTemplateIds = [5, 6],
        PricePerHourForMaterials = 0,
        PricePerHourForEquipment = 0,
        Duration = TimeSpan.Zero,
        Name = "Test Name",
        Id = 3
    };

    private static readonly Mock<IApplicationDbContext> MockDbContext = new();

    private readonly GetServicesByResultTemplateIdQueryHandler _handler =
        new GetServicesByResultTemplateIdQueryHandler(MockDbContext.Object);

    [Fact]
    public async Task Test_Handle_ReturnsListOfServices_WhenNeededServicesExistAndThereIsNotNeededService()
    {
        // Arrange

        MockDbContext.Setup(db => db.Services)
            .Returns(MockDbSetUtility.MockDbSet([
                _firstServiceWithNeededHealthCertificateTemplateId,
                _secondServiceWithNeededHealthCertificateTemplateId,
                _serviceWithoutNeededHealthCertificateTemplateId
            ]));

        var getServicesByResultTemplateIdQuery =
            new GetServicesByResultTemplateIdQuery(NeededHealthCertificateTemplateId);

        // Act

        var result = await _handler.Handle(getServicesByResultTemplateIdQuery, _cancellationToken);

        // Assert

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var returnedServices = result.Value;
        
        Assert.Equal(2, returnedServices.Count);
        Assert.Contains(_firstServiceWithNeededHealthCertificateTemplateId, returnedServices);
        Assert.Contains(_secondServiceWithNeededHealthCertificateTemplateId, returnedServices);
        Assert.DoesNotContain(_serviceWithoutNeededHealthCertificateTemplateId, returnedServices);
    }

    [Fact]
    public async Task Test_Handle_ReturnsEmptyList_WhenNeededServicesDoNotExistAndThereIsNotNeededService()
    {
        // Arrange

        MockDbContext.Setup(db => db.Services)
            .Returns(MockDbSetUtility.MockDbSet([
                _serviceWithoutNeededHealthCertificateTemplateId
            ]));

        var getServicesByResultTemplateIdQuery =
            new GetServicesByResultTemplateIdQuery(NeededHealthCertificateTemplateId);

        // Act

        var result = await _handler.Handle(getServicesByResultTemplateIdQuery, _cancellationToken);

        // Assert

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var returnedServices = result.Value;

        Assert.Empty(returnedServices);
    }

    [Fact]
    public async Task Test_Handle_ReturnsEmptyList_WhenNeededServicesDoNotExistAndThereIsNoNotNeededService()
    {
        // Arrange

        MockDbContext.Setup(db => db.Services)
            .Returns(MockDbSetUtility.MockDbSet(new List<Service>()));

        var getServicesByResultTemplateIdQuery =
            new GetServicesByResultTemplateIdQuery(NeededHealthCertificateTemplateId);

        // Act

        var result = await _handler.Handle(getServicesByResultTemplateIdQuery, _cancellationToken);

        // Assert

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var returnedServices = result.Value;

        Assert.Empty(returnedServices);
    }
}