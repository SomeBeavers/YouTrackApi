using BlazorYouTrackApp.Components;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
var builder1 = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", false, true);

var _configuration = builder1.Build();
builder.Services.AddScoped(sp => new HttpClient
{
	BaseAddress = new Uri("https://youtrack.jetbrains.com/"),
	DefaultRequestHeaders =
	{
		Accept = { new MediaTypeWithQualityHeaderValue("application/json") },
		Authorization = new AuthenticationHeaderValue("Bearer",
			_configuration["Authentication:BearerToken"])
	}
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
