using Microsoft.Playwright;

namespace AutomationFramework.Infrastructure.UI.Pages
{
    public class WikiPage
    {
        private readonly IPage _page;
        private static readonly string url = "https://en.wikipedia.org/wiki/Playwright_(software)";

        private WikiPage(IPage page)
        {
            _page = page;
        }

        public static async Task<WikiPage> GoTo(IPage page)
        {
            var wikiPage = new WikiPage(page);
            await page.GotoAsync(url);
            return wikiPage;
        }

        public async Task<string> GetDebuggingFeaturesTextAsync()
        {
            var p = await _page.Locator("xpath=//*[@id='Debugging_features']/../following-sibling::p[1]")
                .InnerTextAsync();
            var ul = await _page.Locator("xpath=//*[@id='Debugging_features']/../following-sibling::ul[1]")
                .InnerTextAsync();
            return p.Normalize() + ul.Normalize();
        }

        private string NormalizeText(string text)
        {
            var normalized = Regex.Replace(text.ToLower(), @"[^\w\s]", "");
            return normalized;
        }

    }
}
