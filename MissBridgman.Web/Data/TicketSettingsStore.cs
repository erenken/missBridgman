using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure;

namespace MissBridgman.Web.Data;

public sealed class TicketSettingsStore(IConfiguration configuration)
{
    public static readonly PricingSettings Defaults = new(
        TicketPrice: 20m,
        GoodLuckPrice: 15m,
        ArchivedGoodLuckPrice: 10m,
        FullPageAdPrice: 250m,
        HalfPageAdPrice: 125m,
        QuarterPageAdPrice: 50m,
        BusinessCardAdPrice: 25m,
        ContactEmail: "missbridgmanqc@gmail.com");

    private readonly BlobServiceClient? _service = CreateService(configuration);

    public async Task<PricingSettings> GetSettingsAsync(CancellationToken token = default)
    {
        if (_service is null) return Defaults;
        try
        {
            var blob = _service.GetBlobContainerClient("site-settings").GetBlobClient("pricing-settings.json");
            var saved = (await blob.DownloadContentAsync(token)).Value.Content.ToObjectFromJson<PricingSettings>();
            return saved is not null && IsValid(saved) ? saved : Defaults;
        }
        catch (RequestFailedException error) when (error.Status == 404)
        {
            // Preserve an existing ticket price when upgrading from the original single-setting file.
            try
            {
                var oldBlob = _service.GetBlobContainerClient("site-settings").GetBlobClient("ticket-price.json");
                var old = (await oldBlob.DownloadContentAsync(token)).Value.Content.ToObjectFromJson<LegacyTicketPrice>();
                return old is { Price: > 0 and <= 1000 } ? Defaults with { TicketPrice = old.Price } : Defaults;
            }
            catch (RequestFailedException missing) when (missing.Status == 404) { return Defaults; }
        }
    }

    public async Task SaveSettingsAsync(PricingSettings settings, CancellationToken token = default)
    {
        if (!IsValid(settings)) throw new ArgumentOutOfRangeException(nameof(settings), "All configured prices must be between $0 and $100,000.");
        if (_service is null) throw new InvalidOperationException("Azure storage is not configured.");
        var container = _service.GetBlobContainerClient("site-settings");
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: token);
        await container.GetBlobClient("pricing-settings.json").UploadAsync(
            BinaryData.FromObjectAsJson(settings), overwrite: true, cancellationToken: token);
    }

    private static bool IsValid(PricingSettings value) =>
        value.TicketPrice is >= 0 and <= 100000 &&
        value.GoodLuckPrice is >= 0 and <= 100000 &&
        value.ArchivedGoodLuckPrice is >= 0 and <= 100000 &&
        value.FullPageAdPrice is >= 0 and <= 100000 &&
        value.HalfPageAdPrice is >= 0 and <= 100000 &&
        value.QuarterPageAdPrice is >= 0 and <= 100000 &&
        value.BusinessCardAdPrice is >= 0 and <= 100000 &&
        !string.IsNullOrWhiteSpace(value.ContactEmail) && value.ContactEmail.Length <= 254 &&
        System.Net.Mail.MailAddress.TryCreate(value.ContactEmail, out var address) &&
        address.Address.Equals(value.ContactEmail, StringComparison.OrdinalIgnoreCase);

    private static BlobServiceClient? CreateService(IConfiguration config)
    {
        var connection = config["AzureWebJobsStorage"] ?? config.GetConnectionString("Storage");
        return string.IsNullOrWhiteSpace(connection) ? null : new BlobServiceClient(connection);
    }

    private sealed record LegacyTicketPrice(decimal Price);
}

public sealed record PricingSettings(
    decimal TicketPrice,
    decimal GoodLuckPrice,
    decimal ArchivedGoodLuckPrice,
    decimal FullPageAdPrice,
    decimal HalfPageAdPrice,
    decimal QuarterPageAdPrice,
    decimal BusinessCardAdPrice,
    string ContactEmail);
