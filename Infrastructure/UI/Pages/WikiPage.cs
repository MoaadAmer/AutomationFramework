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


        public async Task<IReadOnlyList<ILocator>> GetNavboxListItemsByTitleAsync(string navboxTitle)
        {
            // Find a navbox table whose whole block contains the navboxTitle text.
            // We then collect all list items within its .navbox-list containers.
            // This is resilient to small structural changes within the navbox.
            var navbox = _page.Locator("//table[contains(@class,'navbox')]")
                              .Filter(new LocatorFilterOptions { HasTextString = navboxTitle });

            await navbox.First.ScrollIntoViewIfNeededAsync();
            await navbox.First.WaitForAsync();

            // Collect all LI elements within the navbox lists
            var items = navbox.Locator(".navbox-list li");
            var all = await items.AllAsync();

            return all;
        }


        public static async Task<(string? Href, string VisibleText)> GetLiAnchorAsync(ILocator li)
        {
            var a = li.Locator("a").First;
            if (!await a.IsVisibleAsync())
            {
                return (null, await li.InnerTextAsync());
            }
            var href = await a.GetAttributeAsync("href");
            var text = await a.InnerTextAsync();
            return (href, text.Trim());
        }


    }
}
