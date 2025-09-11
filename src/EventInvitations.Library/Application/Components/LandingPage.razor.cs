using Microsoft.AspNetCore.Components;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;

namespace PersonalPortfolio.Library.Application.Components;

public partial class LandingPage
{
    private bool _hasLoaded;
    private MainPage _mainPage;
    private WebsiteData _websiteDatabaseData;
    [Inject] private IWebsiteRepo WebsiteRepo { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (_hasLoaded) return;

        _websiteDatabaseData = await WebsiteRepo.GetWebsiteData();
        _hasLoaded = _websiteDatabaseData is not null;
        _mainPage = _websiteDatabaseData?.MainPage;
        StateHasChanged();
    }
}