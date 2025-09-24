using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;
using PersonalPortfolio.Library.Infrastructure.Repo;
using Xunit;

namespace EventInvitations.Tests.Infrastructure;

public class WebsiteRepoTests
{
    private static WebsiteDatabaseData BuildSampleDbData()
    {
        return new WebsiteDatabaseData
        {
            Configurations = new Configurations { WebsiteTheme = WebsiteTheme.Blue, EnableDarkMode = true },
            PersonalInformation = new PersonalInformation
            {
                Person = new Person { FirstName = "Alex", LastName = "Mercado", FullName = "Alex Mercado" },
                SocialMediaLinks = new SocialMediaLinks { Github = "https://github.com/test" }
            },
            WebsiteData = new WebsiteData
            {
                WeddingInvitation = new WeddingInvitation
                {
                    HeroBackgroundImage = "img.jpg",
                    CoupleNames = "Alex & Rocío",
                    DateText = "June 1, 2026",
                    Subtitle = "Join us",
                    ScrollIndicatorText = "Scroll",
                    FooterText = "Thank you",
                    Sections = new()
                    {
                        new WeddingSection { Title = "Our Story", Image = "story.jpg", Paragraphs = new() { "p1", "p2" } }
                    }
                }
            }
        };
    }

    [Fact]
    public async Task EnsureInitializedAsync_Should_Initialize_Once_And_Cache()
    {
        // Arrange
        var dbData = BuildSampleDbData();
        var dbMock = new Mock<IDatabaseService>();
        dbMock.Setup(x => x.GetWebsiteDatabaseDataAsync()).ReturnsAsync(dbData);

        var sut = new WebsiteRepo(dbMock.Object);

        // Act
        await sut.EnsureInitializedAsync();
        await sut.EnsureInitializedAsync();
        var websiteData = await sut.GetWebsiteData();
        var personal = await sut.GetPersonalInformation();
        var config = await sut.GetConfigurations();

        // Assert
        dbMock.Verify(x => x.GetWebsiteDatabaseDataAsync(), Times.Once);
        websiteData.Should().BeSameAs(dbData.WebsiteData);
        personal.Should().BeSameAs(dbData.PersonalInformation);
        config.Should().BeSameAs(dbData.Configurations);
    }

    [Fact]
    public async Task GetWebsiteData_Should_Trigger_Lazy_Init_When_Not_Initialized()
    {
        // Arrange
        var dbData = BuildSampleDbData();
        var dbMock = new Mock<IDatabaseService>();
        dbMock.Setup(x => x.GetWebsiteDatabaseDataAsync()).ReturnsAsync(dbData);

        var sut = new WebsiteRepo(dbMock.Object);

        // Act
        var websiteData = await sut.GetWebsiteData();

        // Assert
        dbMock.Verify(x => x.GetWebsiteDatabaseDataAsync(), Times.Once);
        websiteData.Should().BeSameAs(dbData.WebsiteData);
    }

    [Fact]
    public async Task Should_Propagate_Errors_From_DatabaseService()
    {
        // Arrange
        var dbMock = new Mock<IDatabaseService>();
        dbMock.Setup(x => x.GetWebsiteDatabaseDataAsync()).ThrowsAsync(new InvalidOperationException("boom"));
        var sut = new WebsiteRepo(dbMock.Object);

        // Act
        var act = async () => await sut.GetWebsiteData();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*boom*");
    }
}