using ASP_NET_20._TaskFlow.BLL.Common;
using ASP_NET_20._TaskFlow.BLL.DTOs;
using ASP_NET_20._TaskFlow.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_NET_20._TaskFlow.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IProjectService _projectService;
    private readonly ITaskItemService _taskItemService;
    private readonly IAuthorizationService _authorizationService;

    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    public AttachmentsController(IAttachmentService attachmentService, IProjectService projectService, ITaskItemService taskItemService, IAuthorizationService authorizationService)
    {
        _attachmentService = attachmentService;
        _projectService = projectService;
        _taskItemService = taskItemService;
        _authorizationService = authorizationService;
    }

    [HttpPost("~/api/tasks/{taskId}/attachments")]
    public async Task<ActionResult<ApiResponse<AttachmentResponseDto>>> Upload(int taskId, IFormFile file, CancellationToken cancellationToken)
    {
        var task = await _taskItemService.GetTaskEntityAsync(taskId);
        if (task is null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);
        if (project is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authResult.Succeeded) return Forbid();
        if (file is null || file.Length == 0) return BadRequest("File is required");
        AttachmentResponseDto? attachment;
        await using var stream = file.OpenReadStream();
        attachment = await _attachmentService.UploadAsync(taskId, stream, file.FileName, file.ContentType, file.Length, UserId!, cancellationToken);
        if (attachment is null) return NotFound();
        return Ok(ApiResponse<AttachmentResponseDto>.SuccessResponse(attachment, "File uploaded"));
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id, CancellationToken cancellationToken)
    {
        var info = await _attachmentService.GetAttachmentInfoAsync(id, cancellationToken);
        if (info is null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(info.ProjectId);
        if (project is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authResult.Succeeded) return Forbid();
        var result = await _attachmentService.GetDownloadAsync(id, cancellationToken);
        if (result is null) return NotFound();
        return File(result.Value.stream, result.Value.contentType, result.Value.fileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var info = await _attachmentService.GetAttachmentInfoAsync(id, cancellationToken);
        if (info is null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(info.ProjectId);
        if (project is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        var deleted = await _attachmentService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
