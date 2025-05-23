using JetBrains.Annotations;
using SerializationPlugin.Serializers;
using SerializationPlugin.Tests.TestEntities;

namespace SerializationPlugin.Tests.Serializers;

[TestSubject(typeof(JsonSerializer))]
public class JsonSerializerTest
{
    private readonly JsonSerializer _serializer = new();
    private readonly Person _person = new()
    {
        Name = "John",
        Age = 25,
        IsStudent = true
    };
    private readonly ComplexPerson _complexPerson = new()
    {
        Name = "Alice",
        Address = new Address
        {
            City = "New York",
            Street = "Main St"
        }
    };

    [Fact]
    public void Serialize_SimpleObject_ReturnsCorrectJson()
    {
        // Act
        var result = _serializer.Serialize(_person);

        // Assert
        Assert.Equal("{\"Name\":\"John\",\"Age\":25,\"IsStudent\":true}", result);
    }

    [Fact]
    public void Serialize_ComplexObject_ReturnsCorrectJson()
    {
        // Act
        var result = _serializer.Serialize(_complexPerson);

        // Assert
        Assert.Equal("{\"Name\":\"Alice\",\"Address\":{\"City\":\"New York\",\"Street\":\"Main St\"}}", result);
    }

    [Fact]
    public void Serialize_NullObject_ReturnsEmptyJson()
    {
        // Arrange
        Person? person = null;

        // Act
        var result = _serializer.Serialize(person);

        // Assert
        Assert.Equal("{}", result);
    }

    [Fact]
    public void Deserialize_SimpleObject_ReturnsCorrectObject()
    {
        // Arrange
        const string json = "{\"Name\":\"John\",\"Age\":25,\"IsStudent\":true}";

        // Act
        var result = _serializer.Deserialize<Person>(json);

        // Assert
        Assert.Equal("John", result.Name);
        Assert.Equal(25, result.Age);
        Assert.True(result.IsStudent);
    }

    [Fact]
    public void Deserialize_ComplexObject_ReturnsCorrectObject()
    {
        // Arrange
        const string json = "{\"Name\":\"Alice\",\"Address\":{\"City\":\"New York\",\"Street\":\"Main St\"}}";

        // Act
        var result = _serializer.Deserialize<ComplexPerson>(json);

        // Assert
        Assert.Equal("Alice", result.Name);
        Assert.NotNull(result.Address);
        Assert.Equal("New York", result.Address.City);
        Assert.Equal("Main St", result.Address.Street);
    }

    [Fact]
    public void Deserialize_EmptyJson_ReturnsDefaultObject()
    {
        // Arrange
        const string json = "{}";

        // Act
        var result = _serializer.Deserialize<Person>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Name);
        Assert.Equal(0, result.Age);
        Assert.False(result.IsStudent);
    }

    [Fact]
    public void Deserialize_NullOrWhitespace_ReturnsDefaultObject()
    {
        // Arrange
        string? json = null;

        // Act
        var result = _serializer.Deserialize<Person>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Name);
        Assert.Equal(0, result.Age);
        Assert.False(result.IsStudent);
    }

    [Fact]
    public void Deserialize_PrimitiveString_ReturnsCorrectValue()
    {
        // Arrange
        const string json = "\"Hello\"";

        // Act
        var result = _serializer.Deserialize<string>(json);

        // Assert
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Deserialize_PrimitiveInt_ReturnsCorrectValue()
    {
        // Arrange
        const string json = "42";

        // Act
        var result = _serializer.Deserialize<int>(json);

        // Assert
        Assert.Equal(42, result);
    }
}