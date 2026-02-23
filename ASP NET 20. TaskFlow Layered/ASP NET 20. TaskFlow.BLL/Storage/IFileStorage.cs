namespace ASP_NET_20._TaskFlow.BLL.Storage;

public interface IFileStorage
{
    Task<StoredFileInfo> UploadAsync(Stream stream, string originalFileName, string contentType, string folderKey, CancellationToken cancellation = default);
    Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellation = default);
    Task DeleteAsync(string storageKey, CancellationToken cancellation = default);
}
