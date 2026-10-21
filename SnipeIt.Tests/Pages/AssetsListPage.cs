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

    public async Task SearchForAssetAsync(string assetTag)
    {
        var search = _page.Locator("input[type='search']").First;
        await search.FillAsync(assetTag);
        await _page.WaitForTimeoutAsync(1000); // table filters client-side
    }

    public async Task OpenAssetByTagAsync(string assetTag)
    {
        await SearchForAssetAsync(assetTag);
        await _page.GetByRole(AriaRole.Link, new() { Name = assetTag }).ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task AssertAssetExistsInListAsync(string assetTag)
    {
        await SearchForAssetAsync(assetTag);
        await Assertions.Expect(_page.GetByRole(AriaRole.Link, new() { Name = assetTag })).ToBeVisibleAsync();
    }
}