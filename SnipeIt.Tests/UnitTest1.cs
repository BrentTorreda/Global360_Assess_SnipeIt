using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SnipeIt.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CreateMacbookAssetTests : PageTest
{
    [Test]
    [Ignore("Using Page Object version instead")]
    public async Task Create_Macbook_Pro_13_Checked_Out_To_Random_User()
    {
        var assetTag = $"PW-MBP-{DateTime.UtcNow:yyyyMMddHHmmss}";
        Console.WriteLine($"=== Creating asset with tag: {assetTag} ===");

        // ---------- 1. Login ----------
        await Page.GotoAsync("https://demo.snipeitapp.com/login");
        await Page.FillAsync("input[name='username']", "admin");
        await Page.FillAsync("input[name='password']", "password");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForURLAsync("**/");
        await Expect(Page.GetByText("You have successfully logged in.")).ToBeVisibleAsync();

        // ---------- 2. Create Asset ----------
        await Page.GotoAsync("https://demo.snipeitapp.com/hardware/create");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Asset Tag
        await Page.Locator("#asset_tag").FillAsync(assetTag);

        // ----- Model (Select2 AJAX) -----
        await SelectSelect2OptionAsync("#model_select_id", "Macbook Pro 13");

        // ----- Status (Select2) -----
        await SelectSelect2OptionAsync("#status_select_id", "Ready to Deploy");

        // ----- Checkout to a random user (Select2 AJAX) -----
        await SelectSelect2OptionAsync("#assigned_user_select", "a", pickFirst: true);

        // Save
        await Page.Locator("#submit_button").ClickAsync();

        // Wait for either success message or redirect to the asset page
        try
        {
            await Page.WaitForURLAsync("**/hardware/**", new() { Timeout = 10000 });
        }
        catch
        {
            // Still on create page → form validation failed
            var bodyText = await Page.Locator("body").InnerTextAsync();
            Assert.Fail($"Save did not redirect. Page still shows create form.\n\nPage content snippet:\n{bodyText.Substring(0, Math.Min(800, bodyText.Length))}");
        }

        Console.WriteLine($"After save URL: {Page.Url}");

        // ---------- 3. Go to Assets list and search ----------
        await Page.GotoAsync("https://demo.snipeitapp.com/hardware");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Search box in the table
        var searchBox = Page.Locator("input[type='search']").First;
        await searchBox.FillAsync(assetTag);
        await Page.WaitForTimeoutAsync(2000); // allow table to filter

        // The asset tag should appear as a link
        var assetLink = Page.GetByRole(AriaRole.Link, new() { Name = assetTag });
        await Expect(assetLink).ToBeVisibleAsync(new() { Timeout = 10000 });

        // ---------- 4. Open the asset and validate ----------
        await assetLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("body")).ToContainTextAsync(assetTag);
        await Expect(Page.Locator("body")).ToContainTextAsync("Macbook Pro 13");
        await Expect(Page.Locator("body")).ToContainTextAsync("Ready to Deploy");

        // ---------- 5. History tab ----------
        await Page.GetByRole(AriaRole.Link, new() { Name = "History" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page.Locator("body")).ToContainTextAsync("created");

        Console.WriteLine("=== Test completed successfully ===");
    }

    /// <summary>
    /// Helper that reliably works with Snipe-IT's Select2 dropdowns
    /// </summary>
    private async Task SelectSelect2OptionAsync(string selectId, string searchText, bool pickFirst = false)
    {
        // Click the visible Select2 container next to the hidden <select>
        var container = Page.Locator($"{selectId} + .select2-container");
        await container.ClickAsync();

        // Type into the search field that appears
        var searchField = Page.Locator(".select2-search__field");
        await searchField.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await searchField.FillAsync(searchText);

        // Give AJAX time to return results
        await Page.WaitForTimeoutAsync(1200);

        if (pickFirst)
        {
            // Pick the first result (for random user)
            await Page.Locator(".select2-results__option").First.ClickAsync();
        }
        else
        {
            // Click the option that contains the text
            await Page.Locator(".select2-results__option")
                      .Filter(new() { HasTextString = searchText })
                      .First
                      .ClickAsync();
        }

        // Small pause so the value is committed
        await Page.WaitForTimeoutAsync(300);
    }
}