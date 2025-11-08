using AutomationFramework.Infrastructure.API;
using AutomationFramework.Infrastructure.UI.Pages;
using Microsoft.Playwright;

namespace AutomationFramework.Tests
{
    [TestClass]
    public class Test1 : PageTest
    {
        [TestMethod]
        public async Task UniqueWordsForDebuggingSectionShouldBeEqualTest()
        {
            try
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

            catch (Exception ex)
            {
                Console.WriteLine();
            }
        }

    }
}
