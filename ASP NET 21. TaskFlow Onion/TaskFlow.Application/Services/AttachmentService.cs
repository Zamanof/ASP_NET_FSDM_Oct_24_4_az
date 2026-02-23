using TaskFlow.Application.Contracts.Storage;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public class AttachmentService : IAttachmentService
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;
    public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".txt", ".zip" };
    public static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "application/pdf", "text/plain", "application/zip", "application/x-zip-compressed" };

    private readonly ITaskItemRepository _taskRepo;
    private readonly ITaskAttachmentRepository _attachmentRepo;
    private readonly IFileStorage _storage;

    public AttachmentService(ITaskItemRepository taskRepo, ITaskAttachmentRepository attachmentRepo, IFileStorage storage)
    {
        _taskRepo = taskRepo;
        _attachmentRepo = attachmentRepo;
        _storage = storage;
    }

    public async Task<AttachmentInfoDto?> GetAttachmentInfoAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _attachmentRepo.GetByIdAsync(attachmentId);
        if (att == null) return null;
        var task = await _taskRepo.FindAsync(att.TaskItemId);
        if (task == null) return null;
        return new AttachmentInfoDto { Id = att.Id, ProjectId = task.ProjectId };
    }

    public async Task<AttachmentResponseDto?> UploadAsync(int taskId, Stream stream, string originalFileName, string contentType, long length, string userId, CancellationToken cancellationToken = default)
    {
        if (length > MaxFileSizeBytes) throw new ArgumentException($"File size must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB");
        var ext = Path.GetExtension(originalFileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext)) throw new ArgumentException($"Allowed extensions: {string.Join(", ", AllowedExtensions)}");
        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase)) throw new ArgumentException($"Allowed content type: {string.Join(", ", AllowedContentTypes)}");
        var task = await _taskRepo.FindAsync(taskId);
        if (task == null) throw new ArgumentException($"Task with ID {taskId} not found.");
        var folderKey = $"tasks/{taskId}";
        var info = await _storage.UploadAsync(stream, originalFileName, contentType, folderKey, cancellationToken);
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
        await _attachmentRepo.AddAsync(attachment);
        return new AttachmentResponseDto { Id = attachment.Id, TaskItemId = attachment.TaskItemId, OriginalFileName = attachment.OriginalFileName, ContentType = attachment.ContentType, Size = attachment.Size, UploadedUserId = attachment.UploadedUserId, UploadedAt = attachment.UploadedAt };
    }

    public async Task<(Stream stream, string fileName, string contentType)?> GetDownloadAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _attachmentRepo.GetByIdAsync(attachmentId);
        if (att == null) return null;
        var key = $"tasks/{att.TaskItemId}/{att.StoredFileName}";
        var stream = await _storage.OpenReadAsync(key, cancellationToken);
        return (stream, att.OriginalFileName, att.ContentType);
    }

    public async Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _attachmentRepo.GetByIdAsync(attachmentId);
        if (att == null) return false;
        var key = $"tasks/{att.TaskItemId}/{att.StoredFileName}";
        await _attachmentRepo.RemoveAsync(att);
        await _storage.DeleteAsync(key, cancellationToken);
        return true;
    }
}
