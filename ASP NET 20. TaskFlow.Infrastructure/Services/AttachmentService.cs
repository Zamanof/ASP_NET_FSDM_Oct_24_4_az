
using ASP_NET_20._TaskFlow.Application.DTOs;
using ASP_NET_20._TaskFlow.Application.Services;
using ASP_NET_20._TaskFlow.Application.Storage;
using ASP_NET_20._TaskFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_20._TaskFlow.Infrastructure.Services;

public class AttachmentService : IAttachmentService
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;// 5MB
    public static readonly string[] AllowedExtensions = {
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf",
        ".txt",
        ".zip"
    };
    public static readonly string[] AllowedContentTypes = {
        "image/jpeg",
        "image/png",
        "application/pdf",
        "text/plain",
        "application/zip",
        "application/x-zip-compressed"
    };

    private readonly TaskFlowDbContext _context;
    private readonly IFileStorage _storage;

    public AttachmentService(TaskFlowDbContext context, IFileStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<AttachmentResponseDto?> UploadAsync(int taskId, Stream fileStream, string originalFileName, string contentType, long length, string userId, CancellationToken cancellationToken = default)
    {
        if (length > MaxFileSizeBytes)
            throw new ArgumentException($"File size must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB");

        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
            throw new ArgumentException($"Allowed types: {string.Join(", ", AllowedExtensions)}");

        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Allowed content types: {string.Join(", ", AllowedContentTypes)}");

        var task = await _context.TaskItems.FindAsync([taskId], cancellationToken);

        if (task is null)
            return null;

        var folderKey = $"tasks/{taskId}";

        var info = await _storage.UploadAsync(fileStream, originalFileName, contentType, folderKey, cancellationToken);

        var attachment = new TaskAttachment
        {
            TaskItemId = taskId,
            OriginalFileName = originalFileName,
            StoredFileName = info.StoredFileName,
            ContentType = contentType,
            Size = info.Size,
            UploadedUserId = userId,
            UploadedAt = DateTimeOffset.UtcNow
        };

        _context.Attachments.Add(attachment);

        await _context.SaveChangesAsync(cancellationToken);

        return new AttachmentResponseDto
        {
            Id = attachment.Id,
            TaskItemId = attachment.TaskItemId,
            OriginalFileName = attachment.OriginalFileName,
            ContentType = attachment.ContentType,
            Size = attachment.Size,
            UploadedUserId = attachment.UploadedUserId,
            UploadedAt = attachment.UploadedAt
        };


    }
    public async Task<(Stream stream, string fileName, string contentType)?> GetDownloadAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken);

        if (att is null)
            return null;

        var key = $"tasks/{att.TaskItemId}/{att.StoredFileName}";

        var stream = await _storage.OpenReadAsync(key, cancellationToken);

        return (stream, att.OriginalFileName, att.ContentType);
    }
    public async Task<TaskAttachmentInfo?> GetAttachmentInfoAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _context.Attachments
                                      .Include(a => a.TaskItem)
                                      .FirstOrDefaultAsync(a => a.Id == attachmentId);

        if (att is null)
            return null;

        return new TaskAttachmentInfo
        {
            Id = att.Id,
            TaskItemId = att.TaskItemId,
            ProjectId = att.TaskItem.ProjectId,
            StoredFileName = att.StoredFileName,
            StorageKey = $"tasks/{att.TaskItemId}/{att.StoredFileName}",
            UploadedUserId = att.UploadedUserId
        };
    }
    public async Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId);

        if (att is null)
            return false;

        var key = $"tasks/{att.TaskItemId}/{att.StoredFileName}";

        _context.Attachments.Remove(att);

        await _context.SaveChangesAsync(cancellationToken);
        await _storage.DeleteAsync(key, cancellationToken);

        return true;
    }


}
