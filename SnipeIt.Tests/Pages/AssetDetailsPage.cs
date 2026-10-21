using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace SnipeIt.Tests.Pages;

public class AssetDetailsPage
{
    private readonly IPage _page;

    public AssetDetailsPage(IPage page) => _page = page;

    public async Task AssertBasicDetailsAsync(string expectedAssetTag, string expectedModel = "Macbook Pro 13")
    {
        var body = _page.Locator("body");

        await Assertions.Expect(body).ToContainTextAsync(expectedAssetTag);
        await Assertions.Expect(body).ToContainTextAsync(expectedModel);
        await Assertions.Expect(body).ToContainTextAsync("Ready to Deploy");
    }

    public async Task OpenHistoryTabAsync()
    {
        await _page.Locator("a[href='#history']").ClickAsync();

        // Wait until the history table actually shows the "create new" action
        await _page.GetByText("create new").First
            .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 15000 });

        await _page.WaitForTimeoutAsync(500);
    }

    public async Task AssertHistoryContainsAsync(params string[] expectedTexts)
    {
        foreach (var text in expectedTexts)
        {
            try
            {
                await Assertions.Expect(_page.Locator("body")).ToContainTextAsync(text);
            }
            catch (Exception ex)
            {
                // Print a snippet of the page so we can see what is actually there
                var bodyText = await _page.Locator("body").InnerTextAsync();
                var snippet = bodyText.Length > 1200 ? bodyText.Substring(0, 1200) : bodyText;
                Assert.Fail($"History assertion failed for text: '{text}'\n\nPage snippet:\n{snippet}\n\nOriginal error: {ex.Message}");
            }
        }
    }
}