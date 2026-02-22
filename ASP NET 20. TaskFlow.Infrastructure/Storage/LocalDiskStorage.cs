namespace ASP_NET_20._TaskFlow.Infrastructure.Storage;

public class LocalDiskStorage : IFileStorage
{

    private readonly string _basePath;
    private readonly ILogger<LocalDiskStorage> _logger;

    public LocalDiskStorage(IWebHostEnvironment env, ILogger<LocalDiskStorage> logger)
    {
        _basePath = Path.Combine(env.ContentRootPath, "Storage");
        _logger = logger;
    }

    public async Task<StoredFileInfo> UploadAsync(Stream stream, string originalFileName, string contentType, string folderKey, CancellationToken cancellation = default)
    {
        var ext = Path.GetExtension(originalFileName);

        if (string.IsNullOrEmpty(ext))
            ext = ".bin";

        var storedFileName = $"{Guid.NewGuid():N}{ext}";

        var relaitivePath = Path.Combine(folderKey, storedFileName);

        var fullPath = Path.Combine(_basePath, relaitivePath);

        var dir = Path.GetDirectoryName(fullPath);

        Directory.CreateDirectory(dir!);

        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await stream.CopyToAsync(fs, cancellation);
        }

        var size = new FileInfo(fullPath).Length;

        _logger.LogInformation("Uploaded file to {Path}, size {Size}", fullPath, size);

        return new StoredFileInfo
        {
            StorageKey = relaitivePath.Replace('\\', '/'),
            StoredFileName = storedFileName,
            Size =size
        };
    }
    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellation = default)
    {
        var fullPath = Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File not found in storage", storageKey);

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return Task.FromResult<Stream>(stream);
    }
    public Task DeleteAsync(string storageKey, CancellationToken cancellation = default)
    {
        var fullPath = Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted file {Path}", fullPath);
        }

        return Task.CompletedTask;
    }

}
