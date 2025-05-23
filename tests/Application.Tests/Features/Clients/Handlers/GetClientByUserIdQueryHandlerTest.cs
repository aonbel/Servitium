using Application.Features.Clients.Handlers;
using Application.Features.Clients.Queries;
using Application.Tests.Utility;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using JetBrains.Annotations;
using Moq;

namespace Application.Tests.Features.Clients.Handlers;

[TestSubject(typeof(GetClientByUserIdQueryHandler))]
public class GetClientByUserIdQueryHandlerTest
{
    private static readonly string UserId = Guid.NewGuid().ToString();
    private static readonly string NonExistingUserId = Guid.NewGuid().ToString();
    private const int PersonId = 10;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Mock<IApplicationDbContext> _mockDbContext = new();

    private readonly Person _person = new()
    {
        Id = PersonId,
        UserId = UserId,
        FirstName = "John",
        LastName = "Doe",
        MiddleName = "Doe",
        Email = "johndoe@gmail.com",
        Phone = "+123456789"
    };

    private readonly Client _client = new()
    {
        Id = 1,
        PersonId = PersonId,
        Birthday = new DateOnly(1990, 1, 1),
        Gender = "Male"
    };

    [Fact]
    public async Task Test_Handle_ReturnsClient_WhenPersonAndClientExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Persons)
            .Returns(MockDbSetUtility.MockDbSet(new List<Person> { _person }));

        _mockDbContext.Setup(db => db.Clients)
            .Returns(MockDbSetUtility.MockDbSet(new List<Client> { _client }));

        var handler = new GetClientByUserIdQueryHandler(_mockDbContext.Object);

        var request = new GetClientByUserIdQuery(UserId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_client.Id, result.Value.Id);
        Assert.Equal(_client.PersonId, result.Value.PersonId);
        Assert.Equal(_client.Birthday, result.Value.Birthday);
        Assert.Equal(_client.Gender, result.Value.Gender);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Persons)
            .Returns(MockDbSetUtility.MockDbSet(new List<Person>()));

        var handler = new GetClientByUserIdQueryHandler(_mockDbContext.Object);

        var request = new GetClientByUserIdQuery(NonExistingUserId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PersonErrors.NotFoundByUserId(NonExistingUserId), result.Error);
    }

    [Fact]
    public async Task Test_Handle_ReturnsNotFound_WhenClientDoesNotExist()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Persons)
            .Returns(MockDbSetUtility.MockDbSet(new List<Person> { _person }));

        _mockDbContext.Setup(db => db.Clients)
            .Returns(MockDbSetUtility.MockDbSet(new List<Client>()));

        var handler = new GetClientByUserIdQueryHandler(_mockDbContext.Object);

        var request = new GetClientByUserIdQuery(UserId);

        // Act
        var result = await handler.Handle(request, _cancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ClientErrors.NotFoundByPersonId(PersonId), result.Error);
    }
}