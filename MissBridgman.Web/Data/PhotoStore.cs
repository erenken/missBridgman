using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MissBridgman.Web.Data;

public sealed record PhotoMetadata(string Name, string Title, string Year, string Description, string Category);
public sealed record PhotoRecord(string Id, string Name, string Title, string Year, string Description, string Category, string Url);

public sealed class PhotoStore(IConfiguration configuration)
{
    private readonly BlobContainerClient? _container = CreateContainer(configuration);

    public async Task<IReadOnlyList<PhotoRecord>> ListAsync(string? year, CancellationToken token)
    {
        if (_container is null) return [];
        var photos = new List<PhotoRecord>();
        await foreach (var item in _container.GetBlobsAsync(BlobTraits.Metadata, cancellationToken: token))
        {
            var itemYear = item.Metadata.TryGetValue("year", out var storedYear) ? storedYear : LegacyYear(item.Name);
            if (!string.IsNullOrWhiteSpace(year) && itemYear != year) continue;
            var properties = item.Metadata;
            photos.Add(new(item.Name,
                Read(properties, "name") ?? Path.GetFileNameWithoutExtension(item.Name),
                Read(properties, "title") ?? "",
                Read(properties, "year") ?? itemYear ?? "",
                Read(properties, "description") ?? "",
                Read(properties, "category") ?? "",
                _container.GetBlobClient(item.Name).Uri.ToString()));
        }
        return photos.OrderByDescending(p => p.Year).ThenBy(p => p.Name).ToArray();
    }

    public async Task<PhotoRecord> SaveAsync(IFormFile image, PhotoMetadata photo, CancellationToken token)
    {
        if (_container is null) throw new InvalidOperationException("AzureWebJobsStorage is not configured.");
        var id = $"{photo.Year}/{Guid.NewGuid():N}{Path.GetExtension(image.FileName).ToLowerInvariant()}";
        var blob = _container.GetBlobClient(id);
        await using var stream = image.OpenReadStream();
        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = image.ContentType },
            Metadata = new Dictionary<string, string>
            {
                ["name"] = Encode(Clean(photo.Name)), ["title"] = Encode(Clean(photo.Title)), ["year"] = photo.Year,
                ["description"] = Encode(Clean(photo.Description)), ["category"] = Encode(Clean(photo.Category))
            }
        }, token);
        return new(id, photo.Name, photo.Title, photo.Year, photo.Description, photo.Category, blob.Uri.ToString());
    }

    private static string Clean(string value) => value.Length > 500 ? value[..500] : value;
    private static string Encode(string value) => "b64:" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));

    private static BlobContainerClient? CreateContainer(IConfiguration configuration)
    {
        var connection = configuration["AzureWebJobsStorage"] ?? configuration.GetConnectionString("Storage");
        if (string.IsNullOrWhiteSpace(connection)) return null;
        var container = new BlobServiceClient(connection).GetBlobContainerClient("images");
        return container;
    }

    private static string? Read(IDictionary<string, string> metadata, string key)
    {
        if (!metadata.TryGetValue(key, out var value)) return null;
        if (!value.StartsWith("b64:", StringComparison.Ordinal)) return value;
        try { return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value[4..])); }
        catch (FormatException) { return value; }
    }

    private static string? LegacyYear(string blobName)
    {
        var firstSegment = blobName.Split('/', 2)[0];
        return firstSegment.Length == 4 && int.TryParse(firstSegment, out _) ? firstSegment : null;
    }
}
