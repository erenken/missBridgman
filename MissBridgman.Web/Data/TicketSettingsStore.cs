using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure;
using System.Text.Json;

namespace MissBridgman.Web.Data;

public sealed class TicketSettingsStore(IConfiguration configuration)
{
    public const decimal DefaultPrice = 20m;
    private readonly BlobServiceClient? _service = CreateService(configuration);

    public async Task<decimal> GetPriceAsync(CancellationToken token = default)
    {
        if (_service is null) return DefaultPrice;
        try
        {
            var content = await _service.GetBlobContainerClient("site-settings").GetBlobClient("ticket-price.json").DownloadContentAsync(token);
            var saved = content.Value.Content.ToObjectFromJson<TicketPriceSetting>();
            return saved is { Price: > 0 and <= 1000 } ? saved.Price : DefaultPrice;
        }
        catch (RequestFailedException error) when (error.Status == 404) { return DefaultPrice; }
    }

    public async Task SavePriceAsync(decimal price, CancellationToken token = default)
    {
        if (price <= 0 || price > 1000) throw new ArgumentOutOfRangeException(nameof(price));
        if (_service is null) throw new InvalidOperationException("Azure storage is not configured.");
        var container = _service.GetBlobContainerClient("site-settings");
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: token);
        await container.GetBlobClient("ticket-price.json").UploadAsync(
            BinaryData.FromObjectAsJson(new TicketPriceSetting(price)), overwrite: true, cancellationToken: token);
    }

    private static BlobServiceClient? CreateService(IConfiguration config)
    {
        var connection = config["AzureWebJobsStorage"] ?? config.GetConnectionString("Storage");
        return string.IsNullOrWhiteSpace(connection) ? null : new BlobServiceClient(connection);
    }

    private sealed record TicketPriceSetting(decimal Price);
}
