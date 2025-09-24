using System.Text.Json;
using FluentAssertions;
using PersonalPortfolio.Library.Infrastructure.Extensions;
using PersonalPortfolio.Library.Domain;
using Xunit;

namespace EventInvitations.Tests.Infrastructure;

public class MiExtensionsTests
{
    [Fact]
    public void DeserializeWithCamelCase_Should_Handle_CamelCase_And_PascalCase()
    {
        // Arrange
        var person = new Person { FirstName = "Alex", LastName = "Mercado", FullName = "Alex Mercado" };
        var pascalJson = JsonSerializer.Serialize(person); // default: PascalCase
        var camelJson = "{\n  \"firstName\": \"Alex\",\n  \"lastName\": \"Mercado\",\n  \"fullName\": \"Alex Mercado\"\n}";

        // Act
        var fromPascal = pascalJson.DeserializeWithCamelCase<Person>();
        var fromCamel = camelJson.DeserializeWithCamelCase<Person>();

        // Assert
        fromPascal.Should().NotBeNull();
        fromPascal!.FirstName.Should().Be("Alex");
        fromPascal.LastName.Should().Be("Mercado");
        fromPascal.FullName.Should().Be("Alex Mercado");

        fromCamel.Should().NotBeNull();
        fromCamel!.FirstName.Should().Be("Alex");
        fromCamel.LastName.Should().Be("Mercado");
        fromCamel.FullName.Should().Be("Alex Mercado");
    }

    [Fact]
    public void Serialize_Should_Use_Default_SystemTextJson_Behavior_Preserving_Property_Names()
    {
        // Arrange
        var data = new Person { FirstName = "Rocío", LastName = "L.", FullName = "Rocío L." };

        // Act
        var json = data.Serialize();

        // Assert
        json.Should().Contain("\"FirstName\""); // default System.Text.Json keeps PascalCase unless options overridden
        json.Should().Contain("\"LastName\"");
        json.Should().Contain("\"FullName\"");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FirstCharToUpper_Should_Return_Input_When_NullOrWhitespace(string? input)
    {
        // Act
        var result = input!.FirstCharToUpper();

        // Assert
        result.Should().Be(input);
    }

    [Theory]
    [InlineData("hello", "Hello")]
    [InlineData("h", "H")]
    [InlineData("Hello", "Hello")]
    [InlineData("1abc", "1abc")]
    public void FirstCharToUpper_Should_Capitalize_First_Character_When_Possible(string input, string expected)
    {
        // Act
        var result = input.FirstCharToUpper();

        // Assert
        result.Should().Be(expected);
    }
}