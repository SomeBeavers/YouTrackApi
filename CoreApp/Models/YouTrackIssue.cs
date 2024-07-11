namespace CoreApp.Models;

public class YouTrackIssue
{
	public string Id { get; set; }
	public string Summary { get; set; }
	public string Description { get; set; }
	public CustomField[] CustomFields { get; set; }
}

public class CustomField
{
	public string Id { get; set; }
	public string Name { get; set; }

	public object Value { get; set; }
}

public class TypeField
{
	public string Name { get; set; }
}
