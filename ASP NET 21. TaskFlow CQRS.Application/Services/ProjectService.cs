using ASP_NET_21._TaskFlow_CQRS.Application.Contracts.Auth;
using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_21._TaskFlow_CQRS.Application.Repositories;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using AutoMapper;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepo;
    private readonly IProjectMemberRepository _memberRepo;
    private readonly IUserRepository _userRepo;
    private readonly IAuthIdentityProvider _authIdentityProvider;

    public ProjectService(IProjectRepository projectRepo, IProjectMemberRepository memberRepo, IUserRepository userRepo, IAuthIdentityProvider authIdentityProvider)
    {
        _projectRepo = projectRepo;
        _memberRepo = memberRepo;
        _userRepo = userRepo;
        _authIdentityProvider = authIdentityProvider;
    }

    public async Task<Project?> GetProjectEntityAsync(int id) 
        => await _projectRepo.GetByIdWithTasksAndMembersAsync(id);   

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
        AuthUserInfo? user = userIdOrEmail.Contains("@")
            ? await _authIdentityProvider.FindByEmailAsync(userIdOrEmail)
            : await _authIdentityProvider.FindByIdAsync(userIdOrEmail);
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
