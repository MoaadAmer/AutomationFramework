using AutomationFramework.Infrastructure.API;
using AutomationFramework.Infrastructure.UI.Pages;
using Microsoft.Playwright;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace AutomationFramework.Tests
{
    [TestClass]
    public class Tests : PageTest
    {
        [TestMethod]
        public async Task UniqueWordsForDebuggingSectionShouldBeEqualTest()
        {


            var wikiPage = await WikiPage.GoTo(Page);
            string uiText = await wikiPage.GetDebuggingFeaturesTextAsync();
            IAPIRequestContext request = await Playwright.APIRequest.NewContextAsync(new()
            {
                BaseURL = "https://en.wikipedia.org/w/api.php",
                IgnoreHTTPSErrors = true,
            });

            var wikiApiService = new WikiApiService(request);
            string apiText = await wikiApiService.GetDebuggingFeaturesTextAsync();

            var set1 = new HashSet<string>();
            foreach (string word in uiText.Split(" "))
            {
                set1.Add(word);
            }
            var set2 = new HashSet<string>();
            foreach (string word in apiText.Split(" "))
            {
                set2.Add(word);
            }

            Assert.AreEqual(set1, set2);
        }




        [TestMethod]
        public async Task AllMicrosoftDevlopmentToolsTechnologyNamesAreLinksTest()
        {
            string NavboxTitle = "Microsoft development tools";

            WikiPage WikiPage = await WikiPage.GoTo(Page);
            IReadOnlyList<ILocator> items = await WikiPage.GetNavboxListItemsByTitleAsync(NavboxTitle);



            // 2) Validate each list item is a clickable text link (has <a>, non-empty text, has href)
            var failures = new List<string>();

            foreach (var li in items)
            {
                var (href, text) = await WikiPage.GetLiAnchorAsync(li);

                var hasAnchor = href is not null;
                var hasText = !string.IsNullOrWhiteSpace(text);

                if (!(hasAnchor && hasText))
                {
                    var liPreview = (await li.InnerTextAsync()).Trim();
                    failures.Add($"Item not a valid text link: '{liPreview}' (href: {href ?? "null"})");
                }
            }
        }



        [TestMethod]
        public async Task ChangeColorToDarkTest()
        {
            var wikiPage = await WikiPage.GoTo(Page);

            // Act: open Appearance panel and choose "Dark"
            await wikiPage.SwitchToDarkViaAppearanceAsync();

            Assert.IsTrue(await wikiPage.IsDarkThemeActiveAsync());

        }


    }
}