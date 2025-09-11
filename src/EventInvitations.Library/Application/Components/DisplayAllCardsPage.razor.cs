using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;

namespace PersonalPortfolio.Library.Application.Components;

public partial class DisplayAllCardsPage
{
    private OtherPages _currentPage;

    private bool _hasLoaded;
    private WebsiteData _websiteDatabaseData;
    [Parameter] public string ClientRouteName { get; set; } = default!;
    [Inject] private IWebsiteRepo WebsiteRepo { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (_hasLoaded && ClientRouteName == _currentPage?.Endpoint) return;

        _websiteDatabaseData = await WebsiteRepo.GetWebsiteData();
        _currentPage = _websiteDatabaseData.OtherPages.FirstOrDefault(x => x.Endpoint == ClientRouteName);

        _hasLoaded = _currentPage is not null;
        StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_websiteDatabaseData is null)
        {
            _websiteDatabaseData = await WebsiteRepo.GetWebsiteData();
            _currentPage = _websiteDatabaseData.OtherPages.FirstOrDefault(x => x.Endpoint == ClientRouteName);
        }
        else if (_currentPage is not null && ClientRouteName != _currentPage.Endpoint)
        {
            _currentPage = _websiteDatabaseData.OtherPages.FirstOrDefault(x => x.Endpoint == ClientRouteName);
        }
        else
        {
            _currentPage ??= _websiteDatabaseData.OtherPages.FirstOrDefault(x => x.Endpoint == ClientRouteName);
        }

        _hasLoaded = _currentPage is not null;
        StateHasChanged();
    }

    private IEnumerable<IGrouping<string, Card>> GetCardsGroupedByCardName()
    {
        return _currentPage?.Cards.GroupBy(x => x.Name) ?? [];
    }
}