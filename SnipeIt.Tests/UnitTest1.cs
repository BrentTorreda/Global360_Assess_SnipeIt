using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SnipeIt.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CreateMacbookAssetTests : PageTest
{
    [Test]
    public async Task Create_Macbook_Pro_13_Checked_Out_To_Random_User()
    {
        var assetTag = $"PW-MBP-{DateTime.UtcNow:yyyyMMddHHmmss}";

        // 1. Login
        await Page.GotoAsync("https://demo.snipeitapp.com/login");
        await Page.FillAsync("input[name='username']", "admin");
        await Page.FillAsync("input[name='password']", "password");
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForURLAsync("**/");

        // 2. Go to Create Asset
        await Page.GotoAsync("https://demo.snipeitapp.com/hardware/create");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Asset Tag
        await Page.FillAsync("#asset_tag", assetTag);

        // Model - Select2
        await Page.ClickAsync("#model_select_id + .select2-container");
        await Page.FillAsync(".select2-search__field", "Macbook Pro 13");
        await Page.ClickAsync(".select2-results__option >> text=Macbook Pro 13");

        // Status - Ready to Deploy
        await Page.ClickAsync("#status_select_id + .select2-container");
        await Page.ClickAsync(".select2-results__option >> text=Ready to Deploy");

        // Checkout to a user (type a letter and pick first)
        await Page.ClickAsync("#assigned_user_select + .select2-container");
        await Page.FillAsync(".select2-search__field", "a");
        await Page.WaitForTimeoutAsync(1000);
        await Page.ClickAsync(".select2-results__option >> nth=0");

        // Save
        await Page.ClickAsync("button:has-text('Save')");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // 3. Go to Assets list and verify
        await Page.GotoAsync("https://demo.snipeitapp.com/hardware");
        await Page.FillAsync("input[type='search']", assetTag);
        await Page.WaitForTimeoutAsync(1500);

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = assetTag })).ToBeVisibleAsync();

        // 4. Open the asset
        await Page.GetByRole(AriaRole.Link, new() { Name = assetTag }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Validate details
        await Expect(Page.Locator("body")).ToContainTextAsync(assetTag);
        await Expect(Page.Locator("body")).ToContainTextAsync("Macbook Pro 13");
        await Expect(Page.Locator("body")).ToContainTextAsync("Ready to Deploy");

        // 5. History tab
        await Page.ClickAsync("a:has-text('History')");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page.Locator("#history, .history, body")).ToContainTextAsync("created");
    }
}