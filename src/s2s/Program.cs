using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("y").AddHeaderPropagation();
builder.Services.AddHttpClient("authority", x => { x.BaseAddress = new Uri("https://raw.githubusercontent.com/istio/istio/release-1.20/security/tools/jwt/samples/demo.jwt"); });
builder.Services.AddHeaderPropagation(x => x.Headers.Add("Authorization"));
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
app.UseHeaderPropagation();
app.MapGet("/x", async ([FromServices] IOptions<Cleverbit> options, [FromServices] IHttpClientFactory httpClientFactory) =>
{
    if (!options.Value.Enabled || string.IsNullOrEmpty(options.Value.Url))
        return "Service is disabled";

    var http = httpClientFactory.CreateClient("y");
    http.BaseAddress = new Uri(options.Value.Url);
    http.DefaultRequestHeaders.Add("User-Agent", "servicex");
    return await http.GetStringAsync("y");
});

app.MapGet("/y", async ([FromServices] IOptions<Cleverbit> options, [FromServices] IHttpClientFactory httpClientFactory, HttpContext context) =>
{
    if (!options.Value.Enabled)
        return "Service is disabled";

    var http = httpClientFactory.CreateClient("authority");
    var token = await http.GetStringAsync(string.Empty);
    Console.WriteLine(context.Request.Headers["X-JWT-PAYLOAD"].ToString());
    return token.Contains(context.Request.Headers["X-JWT-PAYLOAD"].ToString()) && context.Request.Headers["User-Agent"][0]!.Equals("servicex")
        ? "Hello World!"
        : "Unauthorized";
});

app.Run();

class Cleverbit
{
    public bool Enabled { get; set; }
    public string? Url { get; set; }
}