using JetBrains.Annotations;
using SerializationPlugin.Serializers;
using SerializationPlugin.Tests.TestEntities;

namespace SerializationPlugin.Tests.Serializers
{
    [TestSubject(typeof(TomlSerializer))]
    public class TomlSerializerTest
    {
        private readonly TomlSerializer _serializer = new();
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
        public void Serialize_SimpleObject_ReturnsCorrectToml()
        {
            // Act
            var result = _serializer.Serialize(_person);

            // Assert
            const string expected = "Name = \"John\"\nAge = 25\nIsStudent = true";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Serialize_ComplexObject_ReturnsCorrectToml()
        {
            // Act
            var result = _serializer.Serialize(_complexPerson);

            // Assert
            const string expected = "Name = \"Alice\"\nAddress = [City = \"New York\"\nStreet = \"Main St\"]";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Serialize_NullObject_ReturnsEmptyString()
        {
            // Arrange
            Person? person = null;

            // Act
            var result = _serializer.Serialize(person);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Deserialize_SimpleObject_ReturnsCorrectObject()
        {
            // Arrange
            const string toml = "Name = \"John\"\nAge = 25\nIsStudent = true\n";

            // Act
            var result = _serializer.Deserialize<Person>(toml);

            // Assert
            Assert.Equal("John", result.Name);
            Assert.Equal(25, result.Age);
            Assert.True(result.IsStudent);
        }

        [Fact]
        public void Deserialize_ComplexObject_ReturnsCorrectObject()
        {
            // Arrange
            const string toml = "Name = \"Alice\"\nAddress = [City = \"New York\"\nStreet = \"Main St\"]\n";

            // Act
            var result = _serializer.Deserialize<ComplexPerson>(toml);

            // Assert
            Assert.Equal("Alice", result.Name);
            Assert.NotNull(result.Address);
            Assert.Equal("New York", result.Address.City);
            Assert.Equal("Main St", result.Address.Street);
        }

        [Fact]
        public void Deserialize_EmptyToml_ReturnsDefaultObject()
        {
            // Arrange
            const string toml = "";

            // Act
            var result = _serializer.Deserialize<Person>(toml);

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
            string? toml = null;

            // Act
            var result = _serializer.Deserialize<Person>(toml);

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
            const string toml = "\"Hello\"";

            // Act
            var result = _serializer.Deserialize<string>(toml);

            // Assert
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void Deserialize_PrimitiveInt_ReturnsCorrectValue()
        {
            // Arrange
            const string toml = "42";

            // Act
            var result = _serializer.Deserialize<int>(toml);

            // Assert
            Assert.Equal(42, result);
        }
    }
}