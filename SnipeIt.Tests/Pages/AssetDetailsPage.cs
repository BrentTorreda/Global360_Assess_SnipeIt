namespace SnipeIt.Tests.Pages;

public class AssetDetailsPage
{
    private readonly IPage _page;

    public AssetDetailsPage(IPage page) => _page = page;

    public async Task AssertBasicDetailsAsync(string expectedAssetTag, string expectedModel = "Macbook Pro 13")
    {
        await Assertions.Expect(_page.Locator("h1, .page-header")).ToContainTextAsync(expectedAssetTag);
        await Assertions.Expect(_page.GetByText(expectedModel)).ToBeVisibleAsync();
        await Assertions.Expect(_page.GetByText("Ready to Deploy")).ToBeVisibleAsync();
    }

    public async Task OpenHistoryTabAsync()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "History" }).ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task AssertHistoryContainsAsync(params string[] expectedTexts)
    {
        foreach (var text in expectedTexts)
        {
            await Assertions.Expect(_page.Locator("#history")).ToContainTextAsync(text);
        }
    }
}