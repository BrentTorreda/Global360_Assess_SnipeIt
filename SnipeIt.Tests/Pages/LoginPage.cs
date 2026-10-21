namespace SnipeIt.Tests.Pages;

public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page) => _page = page;

    private ILocator Username => _page.Locator("input[name='username']");
    private ILocator Password => _page.Locator("input[name='password']");
    private ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });

    public async Task GotoAsync()
    {
        await _page.GotoAsync("https://demo.snipeitapp.com/login");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task LoginAsAdminAsync()
    {
        await Username.FillAsync("admin");
        await Password.FillAsync("password");
        await LoginButton.ClickAsync();
        await _page.WaitForURLAsync("**/");
        await Assertions.Expect(_page.GetByText("You have successfully logged in.")).ToBeVisibleAsync();
    }
}