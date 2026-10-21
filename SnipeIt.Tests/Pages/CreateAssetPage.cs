using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace SnipeIt.Tests.Pages;

public class CreateAssetPage
{
    private readonly IPage _page;

    public CreateAssetPage(IPage page) => _page = page;

    public async Task GotoAsync()
    {
        await _page.GotoAsync("https://demo.snipeitapp.com/hardware/create");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task CreateMacbookPro13Async(string assetTag, string userSearchTerm = "a")
    {
        // Asset Tag
        await _page.Locator("#asset_tag").FillAsync(assetTag);

        // Model (Select2)
        await SelectSelect2OptionAsync("#model_select_id", "Macbook Pro 13");

        // Status (Select2)
        await SelectSelect2OptionAsync("#status_select_id", "Ready to Deploy");

        // Checkout to a random user (Select2)
        await SelectSelect2OptionAsync("#assigned_user_select", userSearchTerm, pickFirst: true);

        // Click the correct Save button
        await _page.Locator("#submit_button").ClickAsync();

        // Wait for the page to settle after save
        // Snipe-IT usually redirects to the asset detail page or shows a success banner
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Give it a moment and check what happened
        await _page.WaitForTimeoutAsync(1500);

        var currentUrl = _page.Url;
        Console.WriteLine($"After Save URL: {currentUrl}");

        // If we are still on the create page, the form probably had validation errors
        if (currentUrl.Contains("/hardware/create"))
        {
            var bodyText = await _page.Locator("body").InnerTextAsync();
            var snippet = bodyText.Length > 600 ? bodyText.Substring(0, 600) : bodyText;
            Assert.Fail($"Save did not leave the create page.\nCurrent URL: {currentUrl}\n\nPage snippet:\n{snippet}");
        }

        // Optional: wait a bit more if it redirected to the new asset
        if (currentUrl.Contains("/hardware/"))
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    /// <summary>
    /// Helper for Snipe-IT Select2 dropdowns
    /// </summary>
    private async Task SelectSelect2OptionAsync(string selectId, string searchText, bool pickFirst = false)
    {
        var container = _page.Locator($"{selectId} + .select2-container");
        await container.ClickAsync();

        var searchField = _page.Locator(".select2-search__field");
        await searchField.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
        await searchField.FillAsync(searchText);

        // Allow AJAX results to appear
        await _page.WaitForTimeoutAsync(1200);

        if (pickFirst)
        {
            await _page.Locator(".select2-results__option").First.ClickAsync();
        }
        else
        {
            await _page.Locator(".select2-results__option")
                      .Filter(new() { HasTextString = searchText })
                      .First
                      .ClickAsync();
        }

        await _page.WaitForTimeoutAsync(400);
    }
}