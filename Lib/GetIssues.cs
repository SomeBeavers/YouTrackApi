using CoreApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CoreApp;

public class GetIssues(HttpClient client)
{
	public string ProjectQuery = "project:ReSharper";
	public string CycleDatesQuery = "created: 2024-03-21 .. 2024-07-11";
	public string CreatedByJetBrainsQuery = "created by: jetbrains-team";

	public async Task<List<YouTrackIssue>> GetIssuesAsync()
	{
        var combinedQuery = $"{ProjectQuery} and {CycleDatesQuery} and {CreatedByJetBrainsQuery}";

        var query = $"/api/issues?fields=id,summary,customFields(name,value(name))&query={Uri.EscapeDataString(combinedQuery)}";
        var response = await client.GetAsync(query);
        response.EnsureSuccessStatusCode();


        string data = await response.Content.ReadAsStringAsync();
        var youTrackIssues = JsonConvert.DeserializeObject<List<YouTrackIssue>>(data);

        for (var index = 0; index < youTrackIssues.Count; index++)
        {
            var issue = youTrackIssues[index];
            var customFields = issue.CustomFields;
            foreach (var field in customFields)
            {
                if (field.Name == "Type")
                {
                    if (field.Value == null)
                    {
                        break;
                    }

                    var typeName = JsonConvert.DeserializeObject<TypeField>(field.Value.ToString()).Name;

                    if (typeName != null)
                    {
						youTrackIssues[index].Type = typeName;
                    }
                }
            }
        }

        return youTrackIssues;
    }

	// Get number of issues of every type created by jetbrains-team from 21 March 2024 to 11 July 2024
	public async Task<Dictionary<string, int>> GetIssuesByTypeAsync()
	{
		var combinedQuery = $"{ProjectQuery} and {CycleDatesQuery} and {CreatedByJetBrainsQuery}";

		var query = $"/api/issues?fields=id,summary,customFields(name,value(name))&query={Uri.EscapeDataString(combinedQuery)}";
		var response = await client.GetAsync(query);
		response.EnsureSuccessStatusCode();


		string data = await response.Content.ReadAsStringAsync();
		var youTrackIssues = JsonConvert.DeserializeObject<List<YouTrackIssue>>(data);

		var issueTypeCounts = new Dictionary<string, int>();

		foreach (var issue in youTrackIssues)
		{
			var customFields = issue.CustomFields;
			foreach (var field in customFields)
			{
				if (field.Name == "Type")
				{
					if (field.Value == null)
					{
						break;
					}
					var typeName = JsonConvert.DeserializeObject<TypeField>(field.Value.ToString()).Name;

					if (typeName != null)
					{
						//if (typeName == "Plan")
						//{
						//	Console.WriteLine($"Id: {issue.Id}, Summary: {issue.Summary}");
						//}
						
						if (issueTypeCounts.ContainsKey(typeName))
						{
							issueTypeCounts[typeName]++;
						}
						else
						{
							issueTypeCounts[typeName] = 1;
						}
					}
				}
			}
		}

		Console.WriteLine("__");

		foreach (var kvp in issueTypeCounts)
		{
			Console.WriteLine($"{kvp.Key}: {kvp.Value}");
		}


		return issueTypeCounts;
	}
}