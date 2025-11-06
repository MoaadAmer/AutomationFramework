using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[TestClass]
public sealed class ExampleTest : PageTest
{
    [TestInitialize]
    public async Task TestInitialize()
    {
        await Context.Tracing.StartAsync(new()
        {
            Title = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = Path.Combine(
                Environment.CurrentDirectory,
                "playwright-traces",
                $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}.zip"
            )
        });
    }

    [TestMethod]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://playwright.dev");
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
    }

    [TestMethod]
    public async Task GetStartedLink()
    {
        await Page.GotoAsync("https://playwright.dev");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Get started" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" })).ToBeVisibleAsync();
    }



}
