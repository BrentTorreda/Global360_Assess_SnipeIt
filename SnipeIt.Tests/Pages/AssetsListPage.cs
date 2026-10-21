using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace SnipeIt.Tests.Pages;

public class AssetsListPage
{
    private readonly IPage _page;

    public AssetsListPage(IPage page) => _page = page;

    public async Task GotoAsync()
    {
        await _page.GotoAsync("https://demo.snipeitapp.com/hardware");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task SearchByAssetTagAsync(string assetTag)
    {
        // Top navbar "Lookup by Asset Tag"
        var tagSearch = _page.Locator("#tagSearch");
        await tagSearch.FillAsync(assetTag);
        await tagSearch.PressAsync("Enter");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await _page.WaitForTimeoutAsync(2000);
    }

    public async Task AssertAssetExistsInListAsync(string assetTag)
    {
        await GotoAsync();
        await SearchByAssetTagAsync(assetTag);

        // Accept either a link or plain text containing the tag
        var matcher = _page.Locator($"text={assetTag}").First;
        await Assertions.Expect(matcher).ToBeVisibleAsync(new() { Timeout = 10000 });
    }

    public async Task OpenAssetByTagAsync(string assetTag)
    {
        await GotoAsync();
        await SearchByAssetTagAsync(assetTag);

        // Click the first link that contains the asset tag
        var link = _page.Locator($"a:has-text('{assetTag}')").First;
        await Assertions.Expect(link).ToBeVisibleAsync(new() { Timeout = 10000 });
        await link.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}