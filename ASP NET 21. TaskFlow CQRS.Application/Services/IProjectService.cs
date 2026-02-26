using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_21._TaskFlow_CQRS.Domain;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Services;

public interface IProjectService
{
    Task<Project?> GetProjectEntityAsync(int id);
    Task<IEnumerable<ProjectMemberResponseDto>> GetMembersAsync(int projectId);
    Task<IEnumerable<AvailableUserDto>> GetAvailableUsersToAddAsync(int projectId);
    Task<bool> AddMemberAsync(int projectId, string userIdOrEmail);
    Task<bool> RemoveMemberAsync(int projectId, string userId);
    Task<bool> IsMemberAsync(int projectId, string userId);
}
