using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace CoreApp;

public class Program
{
	private static readonly HttpClient client = new();
	private static IConfiguration _configuration;


	public static async Task Main(string[] args)
	{
		var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", false, true);

		_configuration = builder.Build();

		client.BaseAddress = new Uri("https://youtrack.jetbrains.com/");
		client.DefaultRequestHeaders.Accept.Clear();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
			_configuration["Authentication:BearerToken"]);

		try
		{
			var queries = new GetIssues(client);
			await queries.GetIssuesByTypeAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}
}