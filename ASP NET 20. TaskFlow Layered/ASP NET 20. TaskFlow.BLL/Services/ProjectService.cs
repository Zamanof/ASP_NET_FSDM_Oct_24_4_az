using ASP_NET_20._TaskFlow.BLL.DTOs;
using ASP_NET_20._TaskFlow.DAL;
using ASP_NET_20._TaskFlow.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace ASP_NET_20._TaskFlow.BLL.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepo;
    private readonly IProjectMemberRepository _memberRepo;
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectService(IProjectRepository projectRepo, IProjectMemberRepository memberRepo, IUserRepository userRepo, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _projectRepo = projectRepo;
        _memberRepo = memberRepo;
        _userRepo = userRepo;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<ProjectResponseDto>> GetAllForUserAsync(string userId, IList<string> roles)
    {
        var projects = await _projectRepo.GetAllForUserAsync(userId, roles);
        return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
    }

    public async Task<Project?> GetProjectEntityAsync(int id) => await _projectRepo.GetByIdWithTasksAndMembersAsync(id);

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await _projectRepo.GetByIdWithTasksAsync(id);
        return project == null ? null : _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<ProjectResponseDto> CreateAsync(CreateProjectRequest request, string ownerId)
    {
        var project = _mapper.Map<Project>(request);
        project.OwnerId = ownerId;
        project = await _projectRepo.AddAsync(project);
        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<ProjectResponseDto?> UpdateAsync(int id, UpdateProjectRequest request)
    {
        var project = await _projectRepo.GetByIdWithTasksAsync(id);
        if (project == null) return null;
        _mapper.Map(request, project);
        await _projectRepo.UpdateAsync(project);
        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _projectRepo.FindAsync(id);
        if (project == null) return false;
        await _projectRepo.RemoveAsync(project);
        return true;
    }

    public async Task<IEnumerable<ProjectMemberResponseDto>> GetMembersAsync(int projectId)
    {
        var members = await _memberRepo.GetByProjectIdWithUserAsync(projectId);
        return members.Select(m => new ProjectMemberResponseDto
        {
            UserId = m.UserId,
            Email = m.User.Email ?? "",
            FirstName = m.User.FirstName,
            LastName = m.User.LastName,
            JoinedAt = m.CreatedAt
        });
    }

    public async Task<IEnumerable<AvailableUserDto>> GetAvailableUsersToAddAsync(int projectId)
    {
        var memberIds = await _memberRepo.GetMemberUserIdsAsync(projectId);
        var users = await _userRepo.GetOrderedByEmailExceptIdsAsync(memberIds);
        return users.Select(u => new AvailableUserDto { Id = u.Id, Email = u.Email ?? "", FirstName = u.FirstName, LastName = u.LastName });
    }

    public async Task<bool> AddMemberAsync(int projectId, string userIdOrEmail)
    {
        var project = await _projectRepo.FindAsync(projectId);
        if (project == null) return false;
        var user = userIdOrEmail.Contains("@") ? await _userManager.FindByEmailAsync(userIdOrEmail) : await _userManager.FindByIdAsync(userIdOrEmail);
        if (user == null) return false;
        if (await _memberRepo.ExistsAsync(projectId, user.Id)) return false;
        await _memberRepo.AddAsync(new ProjectMember { ProjectId = projectId, UserId = user.Id, CreatedAt = DateTimeOffset.UtcNow });
        return true;
    }

    public async Task<bool> RemoveMemberAsync(int projectId, string userId)
    {
        var member = await _memberRepo.FindAsync(projectId, userId);
        if (member == null) return false;
        await _memberRepo.RemoveAsync(member);
        return true;
    }

    public Task<bool> IsMemberAsync(int projectId, string userId) => _memberRepo.ExistsAsync(projectId, userId);
}
