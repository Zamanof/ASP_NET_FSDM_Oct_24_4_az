using ASP_NET_21._TaskFlow_CQRS.Application.Contracts.Storage;

namespace ASP_NET_21._TaskFlow_CQRS_Integration_tests;

public class TestFileStorage : IFileStorage
{
    private readonly Dictionary<string, byte[]> _storage = new();
    public Task DeleteAsync(string storageKey, CancellationToken cancellation = default)
    {
        _storage.Remove(storageKey);
        return Task.CompletedTask;
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellation = default)
    {
        if (_storage.TryGetValue(storageKey, out var bytes))
        {
            return Task.FromResult<Stream>(new MemoryStream(bytes));
        }
        throw new FileNotFoundException($"File with storage key '{storageKey}' not found.");
    }

    public Task<StoredFileInfo> UploadAsync(
        Stream stream,
        string originalFileName,
        string contentType,
        string folderKey,
        CancellationToken cancellation = default)
    {
        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrEmpty(ext))
        {
            ext = ".bin";
        }
        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var storageKey = $"{folderKey}/{storedFileName}".Replace('\\', '/');

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var bytes = ms.ToArray();
        _storage[storageKey] = bytes;
        return Task.FromResult(new StoredFileInfo
        {
            StorageKey = storageKey,
            Size = bytes.Length,
            StoredFileName = storedFileName
        });

    }
}