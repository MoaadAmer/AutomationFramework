using Microsoft.Playwright;
using System.Text.Json;

namespace AutomationFramework.Infrastructure.API
{
    public class WikiApiService
    {

        private readonly IAPIRequestContext _apiContext;

        public WikiApiService(IAPIRequestContext apiContext)
        {
            _apiContext = apiContext;
        }


        public async Task<string> GetDebuggingFeaturesTextAsync()
        {
            string section = "Debugging features";

            IAPIResponse listResp = await _apiContext.GetAsync("?action=parse&format=json&page=Playwright_(software)&prop=sections");

            using var listDoc = JsonDocument.Parse(await listResp.BodyAsync());
            var sections = listDoc.RootElement.GetProperty("parse").GetProperty("sections");


            int? index = null;
            foreach (var s in sections.EnumerateArray())
            {
                var line = s.GetProperty("line").GetString()?.Trim();
                if (string.Equals(line, section, StringComparison.OrdinalIgnoreCase))
                {
                    index = int.Parse(s.GetProperty("index").GetString()!);
                    break;
                }
            }
            const string url = "https://en.wikipedia.org/w/api.php?action=parse&format=json&page=Playwright_(software)&section=5&prop=wikitext";

            var resp = await _apiContext.GetAsync(url);
            var json = await resp.BodyAsync();

            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var wiki = doc.RootElement.GetProperty("parse").GetProperty("wikitext").GetProperty("*").GetString() ?? "";

            return CleanWikitext(wiki).Normalize();
        }

        private string CleanWikitext(string wiki)
        {
            if (string.IsNullOrWhiteSpace(wiki)) return string.Empty;

            var lines = wiki.Split('\n');
            var result = new List<string>();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                if (trimmed.StartsWith("=")) continue;    // headings
                if (trimmed.StartsWith("{{")) continue;   // templates

                // Remove inline <ref> and anything after it
                var refIndex = trimmed.IndexOf("<ref", StringComparison.OrdinalIgnoreCase);
                if (refIndex >= 0)
                    trimmed = trimmed.Substring(0, refIndex).Trim();

                // Remove wiki markup
                trimmed = trimmed.Replace("[[", "").Replace("]]", "");
                trimmed = trimmed.Replace("'''", "").Replace("''", "");

                // Remove leading '*' from list items
                if (trimmed.StartsWith("*"))
                    trimmed = trimmed.Substring(1).Trim();

                if (!string.IsNullOrEmpty(trimmed))
                    result.Add(trimmed);
            }

            return string.Join(Environment.NewLine, result);
        }
    }
}
