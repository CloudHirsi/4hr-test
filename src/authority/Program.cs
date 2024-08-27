using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("authority", x => { x.BaseAddress = new Uri("https://raw.githubusercontent.com/istio/istio/release-1.20/security/tools/jwt/samples/demo.jwt"); });
var pathKpf = Path.Combine(Directory.GetCurrentDirectory(), "kpf");
if (Directory.Exists(pathKpf))
{
    var kpfFiles = Directory.GetDirectories(pathKpf, "*", new EnumerationOptions { RecurseSubdirectories = true });
    foreach (var kpf in kpfFiles)
    {
        builder.Configuration.AddKeyPerFile(kpf, true, true);
    }
}
builder.Services.Configure<Cleverbit>(builder.Configuration.GetSection(nameof(Cleverbit)));
var app = builder.Build();

app.MapGet("/token", async ([FromServices] IOptions<Cleverbit> options, [FromServices] IHttpClientFactory httpClientFactory) =>
{
    if (!options.Value.Enabled)
        return "Service is disabled";

    var http = httpClientFactory.CreateClient("authority");
    return await http.GetStringAsync(string.Empty);
});

app.Run();

class Cleverbit
{
    public bool Enabled { get; set; }
}