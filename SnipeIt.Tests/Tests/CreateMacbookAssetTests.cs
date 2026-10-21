using SnipeIt.Tests.Helpers;
using SnipeIt.Tests.Pages;

namespace SnipeIt.Tests.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CreateMacbookAssetTests : PageTest
{
    [Test]
    public async Task Create_Macbook_Pro_13_Checked_Out_To_Random_User_And_Validate()
    {
        var assetTag = TestData.GenerateUniqueAssetTag();

        // 1. Login
        var loginPage = new LoginPage(Page);
        await loginPage.GotoAsync();
        await loginPage.LoginAsAdminAsync();

        // 2. Create asset
        var createPage = new CreateAssetPage(Page);
        await createPage.GotoAsync();
        await createPage.CreateMacbookPro13Async(assetTag);

        // Capture the assigned user name from the success page / details for later assertion
        var detailsPage = new AssetDetailsPage(Page);
        // After save we usually land on the asset details page

        // 3. Go to Assets list and verify it appears
        var listPage = new AssetsListPage(Page);
        await listPage.GotoAsync();
        await listPage.AssertAssetExistsInListAsync(assetTag);

        // 4. Open the asset and validate details
        await listPage.OpenAssetByTagAsync(assetTag);
        await detailsPage.AssertBasicDetailsAsync(assetTag, "Macbook Pro 13");

        // 5. History tab
        await detailsPage.OpenHistoryTabAsync();
        await detailsPage.AssertHistoryContainsAsync(
            "create new",
            "checkout"
        );
    }
}