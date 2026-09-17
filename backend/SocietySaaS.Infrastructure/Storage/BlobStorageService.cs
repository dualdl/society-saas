using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace SocietySaaS.Infrastructure.Storage;

public interface IBlobStorageService
{
    Task<string> UploadAsync(string containerName, string blobName, Stream stream, string contentType);
    Task<Stream?> DownloadAsync(string containerName, string blobName);
    Task DeleteAsync(string containerName, string blobName);
}

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureStorage:ConnectionString"] ?? configuration["AZURE_STORAGE_CONNECTIONSTRING"];
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadAsync(string containerName, string blobName, Stream stream, string contentType)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync();
        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(stream, overwrite: true);
        return blob.Uri.ToString();
    }

    public async Task<Stream?> DownloadAsync(string containerName, string blobName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        if (!await blob.ExistsAsync()) return null;
        var response = await blob.DownloadAsync();
        return response.Value.Content;
    }

    public async Task DeleteAsync(string containerName, string blobName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync();
    }
}
