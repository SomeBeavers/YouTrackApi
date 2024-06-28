using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CoreApp;

public class Program
{
    private static readonly HttpClient client = new();
    private static IConfiguration _configuration;


    public static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true);

        _configuration = builder.Build();

        client.BaseAddress = new Uri("https://youtrack.jetbrains.com/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // TODO: move token to config
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            "");

        try
        {
            var issues = await GetIssuesAsync("ReSharper", 10);
            foreach (var issue in issues) Console.WriteLine($"Issue {issue.Id}: {issue.Summary}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static async Task<List<YouTrackIssue>> GetIssuesAsync(string project, int limit)
    {
        var query = $"api/issues?fields=id,summary,description&query=project:{project}&$top={limit}";
        var response = await client.GetAsync(query);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<YouTrackIssue>>(data);
    }
}