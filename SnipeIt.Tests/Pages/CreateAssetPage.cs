namespace SnipeIt.Tests.Pages;

public class CreateAssetPage
{
    private readonly IPage _page;

    public CreateAssetPage(IPage page) => _page = page;

    private ILocator AssetTagInput => _page.Locator("#asset_tag");
    private ILocator ModelSelect => _page.Locator("#model_select_id + .select2-container");
    private ILocator StatusSelect => _page.Locator("#status_select_id + .select2-container");
    private ILocator AssignedUserSelect => _page.Locator("#assigned_user_select + .select2-container");
    private ILocator SaveButton => _page.GetByRole(AriaRole.Button, new() { Name = "Save" });

    public async Task GotoAsync()
    {
        await _page.GotoAsync("https://demo.snipeitapp.com/hardware/create");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task CreateMacbookPro13Async(string assetTag, string userSearchTerm = "a")
    {
        // Asset Tag
        await AssetTagInput.FillAsync(assetTag);

        // Model – Select2 AJAX
        await ModelSelect.ClickAsync();
        await _page.Locator(".select2-search__field").FillAsync("Macbook Pro 13");
        await _page.Locator(".select2-results__option").Filter(new() { HasText = "Macbook Pro 13" }).First.ClickAsync();

        // Status – Ready to Deploy
        await StatusSelect.ClickAsync();
        await _page.Locator(".select2-results__option").Filter(new() { HasText = "Ready to Deploy" }).ClickAsync();

        // Checkout to a random user (type a letter and pick the first result)
        await AssignedUserSelect.ClickAsync();
        await _page.Locator(".select2-search__field").FillAsync(userSearchTerm);
        await _page.WaitForTimeoutAsync(800); // allow AJAX
        var firstUser = _page.Locator(".select2-results__option").First;
        await firstUser.ClickAsync();

        await SaveButton.ClickAsync();
        await _page.WaitForURLAsync("**/hardware/**");
    }
}