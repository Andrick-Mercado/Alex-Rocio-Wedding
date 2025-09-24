using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure.Repo;
using Xunit;

namespace EventInvitations.Tests.Infrastructure;

public class WebDatabaseServiceTests
{
    private sealed class StubMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public StubMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }

    private static HttpClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        var handler = new StubMessageHandler(responder);
        return new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
    }

    [Fact]
    public async Task GetWebsiteDatabaseDataAsync_Should_Return_Data_On_Success()
    {
        // Arrange
        var data = new WebsiteDatabaseData
        {
            Configurations = new Configurations { WebsiteTheme = WebsiteTheme.Green, EnableDarkMode = false },
            PersonalInformation = new PersonalInformation
            {
                Person = new Person { FirstName = "A", LastName = "B", FullName = "A B" },
                SocialMediaLinks = new SocialMediaLinks { Github = "gh" }
            },
            WebsiteData = new WebsiteData { WeddingInvitation = new WeddingInvitation { CoupleNames = "X & Y" } }
        };
        var json = JsonSerializer.Serialize(data);
        var client = CreateClient(req =>
        {
            req.RequestUri!.ToString().Should().EndWith("database/websiteData.json");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        });
        var sut = new WebDatabaseService(client);

        // Act
        var result = await sut.GetWebsiteDatabaseDataAsync();

        // Assert
        result.Should().NotBeNull();
        result.WebsiteData.WeddingInvitation.CoupleNames.Should().Be("X & Y");
    }

    [Fact]
    public async Task GetWebsiteDatabaseDataAsync_Should_Throw_On_404()
    {
        // Arrange
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var sut = new WebDatabaseService(client);

        // Act
        var act = async () => await sut.GetWebsiteDatabaseDataAsync();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetWebsiteDatabaseDataAsync_Should_Throw_InvalidOperation_When_Body_Null()
    {
        // Arrange: return explicit JSON null which deserializes to null
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        });
        var sut = new WebDatabaseService(client);

        // Act
        var act = async () => await sut.GetWebsiteDatabaseDataAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetWebsiteDatabaseDataAsync_Should_Throw_On_Invalid_Json()
    {
        // Arrange
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ invalid json ]", Encoding.UTF8, "application/json")
        });
        var sut = new WebDatabaseService(client);

        // Act
        var act = async () => await sut.GetWebsiteDatabaseDataAsync();

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }
}