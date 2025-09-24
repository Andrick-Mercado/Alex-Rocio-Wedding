using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;
using Moq;
using Xunit;

namespace EventInvitations.Tests.Blazor;

public class AppRoutingTests : TestContext
{
    private void RegisterDependencies(WebsiteData website)
    {
        Services.AddMudServices();

        var repo = new Mock<IWebsiteRepo>();
        repo.Setup(x => x.GetWebsiteData()).ReturnsAsync(website);
        Services.AddSingleton(repo.Object);

        JSInterop.SetupVoid("weddingSnow.init");
        JSInterop.SetupVoid("scrollFade.init");
    }

    [Fact]
    public void Router_Should_Render_Index_At_Root()
    {
        // Arrange
        var website = new WebsiteData { WeddingInvitation = new WeddingInvitation { CoupleNames = "Alex & Rocío" } };
        RegisterDependencies(website);

        // Act
        var cut = RenderComponent<PersonalPortfolio.Blazor.App>();

        // Assert: Index page renders WeddingInvitation content
        cut.Find(".couple").TextContent.Should().Be("Alex & Rocío");
    }

    [Fact]
    public void Router_Should_Render_NotFound_For_Unknown_Route()
    {
        // Arrange
        var website = new WebsiteData { WeddingInvitation = new WeddingInvitation { CoupleNames = "Alex & Rocío" } };
        RegisterDependencies(website);
        var cut = RenderComponent<PersonalPortfolio.Blazor.App>();

        // Act
        var nav = Services.GetRequiredService<NavigationManager>() as Bunit.TestDoubles.FakeNavigationManager;
        nav!.NavigateTo("http://localhost/this-does-not-exist");
        cut.Render();

        // Assert
        cut.Markup.Should().Contain("Not Found");
    }
}