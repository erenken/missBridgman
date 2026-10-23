using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenTelemetry;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MissBridgman.Web.Components;
using MissBridgman.Web.Data;

var builder = WebApplication.CreateBuilder(args);
builder.UseMicrosoftOpenTelemetry(options =>
{
    options.Exporters = ExportTarget.AzureMonitor;
});
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication("AdminCookie").AddCookie("AdminCookie", options =>
{
    options.Cookie.Name = "MissBridgman.Admin";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<PhotoStore>();
builder.Services.AddSingleton<TicketSettingsStore>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" })).AllowAnonymous();

app.MapPost("/api/admin/login", async (HttpContext context, IConfiguration config) =>
{
    var form = context.Request.HasFormContentType ? await context.Request.ReadFormAsync() : null;
    var password = form?["password"].ToString() ?? (await context.Request.ReadFromJsonAsync<LoginRequest>())?.Password;
    var expected = config["Admin:Password"];
    if (string.IsNullOrWhiteSpace(expected) || password is null || !FixedTimeEquals(expected, password))
        return form is null ? Results.Unauthorized() : Results.Redirect("/admin?error=1");

    await context.SignInAsync("AdminCookie", new System.Security.Claims.ClaimsPrincipal(
        new System.Security.Claims.ClaimsIdentity([new System.Security.Claims.Claim("Name", "Site administrator"), new System.Security.Claims.Claim("Role", "Admin")], "AdminCookie")));
    return form is null ? Results.Ok() : Results.Redirect("/admin?login=1");
}).AllowAnonymous();

app.MapPost("/api/admin/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("AdminCookie");
    return Results.NoContent();
}).RequireAuthorization();

app.MapGet("/api/photos", async (PhotoStore store, string? year, CancellationToken token) =>
    Results.Ok(await store.ListAsync(year, token))).AllowAnonymous();

app.MapGet("/api/forms/ticket-price", async (TicketSettingsStore store, CancellationToken token) =>
    Results.Ok((await store.GetSettingsAsync(token)).TicketPrice)).AllowAnonymous();

app.MapPost("/api/admin/pricing", async (HttpRequest request, TicketSettingsStore store, CancellationToken token) =>
{
    var form = request.HasFormContentType ? await request.ReadFormAsync(token) : null;
    if (form is null) return Results.BadRequest("Expected form data.");
    static bool ReadPrice(IFormCollection form, string key, out decimal value) => decimal.TryParse(
        form[key], System.Globalization.NumberStyles.Number,
        System.Globalization.CultureInfo.InvariantCulture, out value) && value is >= 0 and <= 100000;
    if (!ReadPrice(form, "ticketPrice", out var ticketPrice) ||
        !ReadPrice(form, "goodLuckPrice", out var goodLuckPrice) ||
        !ReadPrice(form, "archivedGoodLuckPrice", out var archivedGoodLuckPrice) ||
        !ReadPrice(form, "fullPageAdPrice", out var fullPageAdPrice) ||
        !ReadPrice(form, "halfPageAdPrice", out var halfPageAdPrice) ||
        !ReadPrice(form, "quarterPageAdPrice", out var quarterPageAdPrice) ||
        !ReadPrice(form, "businessCardAdPrice", out var businessCardAdPrice) ||
        !System.Net.Mail.MailAddress.TryCreate(form["contactEmail"].ToString().Trim(), out var contactAddress) ||
        !contactAddress.Address.Equals(form["contactEmail"].ToString().Trim(), StringComparison.OrdinalIgnoreCase))
        return Results.Redirect("/admin?pricingError=1");
    var settings = new PricingSettings(ticketPrice, goodLuckPrice, archivedGoodLuckPrice,
        fullPageAdPrice, halfPageAdPrice, quarterPageAdPrice, businessCardAdPrice,
        form["contactEmail"].ToString().Trim());
    try { await store.SaveSettingsAsync(settings, token); }
    catch (InvalidOperationException) { return Results.Redirect("/admin?ticketPriceError=1"); }
    return Results.Redirect("/admin?pricingUpdated=1");
}).RequireAuthorization();

app.MapPost("/api/admin/photos", async (HttpRequest request, PhotoStore store, CancellationToken token) =>
{
    if (!request.HasFormContentType) return Results.BadRequest("Expected multipart form data.");
    var form = await request.ReadFormAsync(token);
    var image = form.Files.GetFile("image");
    if (image is null || image.Length == 0) return Results.BadRequest("Choose an image to upload.");
    if (image.Length > 12 * 1024 * 1024) return Results.BadRequest("Images must be 12 MB or smaller.");
    if (image.ContentType is not ("image/jpeg" or "image/png" or "image/webp")) return Results.BadRequest("Use a JPG, PNG, or WebP image.");
    var photo = new PhotoMetadata(
        form["name"].ToString().Trim(), form["title"].ToString().Trim(), form["year"].ToString().Trim(),
        form["description"].ToString().Trim(), form["category"].ToString().Trim());
    if (string.IsNullOrWhiteSpace(photo.Name) || photo.Year.Length != 4 || !int.TryParse(photo.Year, out _))
        return Results.BadRequest("Name and a four-digit year are required.");
    await store.SaveAsync(image, photo, token);
    return Results.Redirect("/admin?uploaded=1");
}).RequireAuthorization();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();

static bool FixedTimeEquals(string expected, string actual)
{
    var left = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
    var right = SHA256.HashData(Encoding.UTF8.GetBytes(actual ?? string.Empty));
    return CryptographicOperations.FixedTimeEquals(left, right);
}

record LoginRequest(string Password);
