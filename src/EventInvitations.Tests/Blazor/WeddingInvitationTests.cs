using System.Linq;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.Services;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;
using Xunit;

namespace EventInvitations.Tests.Blazor;

public class WeddingInvitationTests : BunitContext
{
    private static WebsiteData BuildWebsiteData(
        string? hero = "hero.jpg",
        string couple = "Alex & Rocío",
        string date = "June 1, 2026",
        string subtitle = "Join us",
        string scroll = "Scroll down",
        string footer = "Gracias")
    {
        return new WebsiteData
        {
            WeddingInvitation = new WeddingInvitation
            {
                HeroBackgroundImage = hero,
                CoupleNames = couple,
                DateText = date,
                Subtitle = subtitle,
                ScrollIndicatorText = scroll,
                FooterText = footer,
                Sections = new()
                {
                    new WeddingSection { Title = "Our Story", Image = "story.jpg", ImageAlt = "story", Paragraphs = new() { "p1", "p2" } },
                    new WeddingSection { Title = "Venue", Reverse = true, Image = "venue.jpg", ImageAlt = "venue", Paragraphs = null! }
                }
            }
        };
    }

    private void RegisterCommonServices(WebsiteData website)
    {
        Services.AddMudServices();

        var repo = new Mock<IWebsiteRepo>();
        repo.Setup(x => x.GetWebsiteData()).ReturnsAsync(website);
        Services.AddSingleton(repo.Object);

        // JSInterop expectations for OnAfterRenderAsync
        JSInterop.SetupVoid("weddingSnow.init");
        JSInterop.SetupVoid("scrollFade.init");
    }

    [Fact]
    public void Renders_Hero_And_Basic_Text_From_WebsiteRepo()
    {
        // Arrange
        var data = BuildWebsiteData();
        RegisterCommonServices(data);

        // Act
        var cut = Render<PersonalPortfolio.Blazor.Pages.WeddingInvitation>();

        // Assert
        cut.Find(".couple").TextContent.Should().Be("Alex & Rocío");
        cut.Find(".date").TextContent.Should().Be("June 1, 2026");
        cut.Find(".subtitle").TextContent.Should().Be("Join us");
        cut.Find(".scroll-indicator").TextContent.Should().Be("Scroll down");
        cut.Find(".footer").TextContent.Should().Be("Gracias");

        // Hero style should include background only when non-empty
        var hero = cut.Find(".hero");
        hero.GetAttribute("style").Should().Contain("background-image:url('hero.jpg')");
    }

    [Fact]
    public void Renders_Sections_With_Reverse_And_Paragraphs_Safely()
    {
        // Arrange
        var data = BuildWebsiteData();
        RegisterCommonServices(data);

        // Act
        var cut = Render<PersonalPortfolio.Blazor.Pages.WeddingInvitation>();

        // Assert
        var sections = cut.FindAll(".fade-section");
        sections.Should().HaveCount(2);

        // Section 1: not reversed => image then text
        var inner1 = sections[0].GetElementsByTagName("div").First(e => e.ClassList.Contains("section-inner"));
        var children1 = inner1.Children.ToArray();
        children1[0].ClassName.Should().Contain("section-image");
        children1[1].ClassName.Should().Contain("section-text");
        children1[1].TextContent.Should().Contain("Our Story");
        children1[1].TextContent.Should().Contain("p1");

        // Section 2: reversed => text then image, and paragraphs null handled gracefully
        var inner2 = sections[1].GetElementsByTagName("div").First(e => e.ClassList.Contains("section-inner"));
        inner2.ClassList.Contains("reverse").Should().BeTrue();
        var children2 = inner2.Children.ToArray();
        children2[0].ClassName.Should().Contain("section-text");
        children2[1].ClassName.Should().Contain("section-image");
    }

    [Fact]
    public void JSInterop_Is_Called_Only_On_First_Render()
    {
        // Arrange
        var data = BuildWebsiteData();
        RegisterCommonServices(data);

        // Act
        var cut = Render<PersonalPortfolio.Blazor.Pages.WeddingInvitation>();
        cut.Render(); // trigger a subsequent render; OnAfterRenderAsync(firstRender=false)

        // Assert - allow async OnAfterRenderAsync to complete
        cut.WaitForAssertion(() =>
        {
            JSInterop.Invocations.Count(i => i.Identifier == "weddingSnow.init").Should().Be(1);
        }, timeout: System.TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void When_No_Hero_Image_Then_No_Background_Style_Is_Applied()
    {
        // Arrange
        var data = BuildWebsiteData(hero: " \t\n");
        RegisterCommonServices(data);

        // Act
        var cut = Render<PersonalPortfolio.Blazor.Pages.WeddingInvitation>();

        // Assert
        var hero = cut.Find(".hero");
        hero.GetAttribute("style")?.Should().NotContain("background-image");
    }

    [Fact]
    public void Renders_Events_Section_With_Target_Blank_On_Links()
    {
        // Arrange
        var data = BuildWebsiteData();
        data.WeddingInvitation.Sections.Add(new WeddingSection
        {
            Type = WeddingSectionType.Events,
            Title = "Events",
            Events = new()
            {
                new WeddingEvent
                {
                    Title = "Ceremony",
                    LocationUrl = "https://maps.google.com"
                }
            }
        });
        RegisterCommonServices(data);

        // Act
        var cut = Render<PersonalPortfolio.Blazor.Pages.WeddingInvitation>();

        // Assert
        var link = cut.Find(".event-link");
        link.GetAttribute("target").Should().Be("_blank");
    }
}