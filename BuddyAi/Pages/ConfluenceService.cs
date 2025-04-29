using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class ConfluenceService
{
    private readonly HttpClient _httpClient;

    public ConfluenceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetPageContent()
    {
        var username = "veneret@gmail.com";
        var apiToken = "ATATT3xFfGF06zhEp2OoP2..."; // your token

        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{apiToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");

        var url = "https://veneret.atlassian.net/wiki/rest/api/content/229597?expand=body.storage";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return $"Error: {response.StatusCode}";

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var content = document.RootElement
            .GetProperty("body")
            .GetProperty("storage")
            .GetProperty("value")
            .GetString();

        return content ?? "No content found.";
    }
}
