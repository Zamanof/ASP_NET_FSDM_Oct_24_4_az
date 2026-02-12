using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Data;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Project_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Services;

public class ProjectService : IProjectService
{

    private readonly TaskFlowDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectService(TaskFlowDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    public Task<bool> AddMemberAsync(int projectId, string userIdOrEmail)
    {
        throw new NotImplementedException();
    }

    public async Task<ProjectResponseDto> CreateAsync(
        CreateProjectRequest createProjectRequest, 
        string ownerId)
    {
        var project = _mapper.Map<Project>(createProjectRequest);

        project.OwnerId = ownerId;

        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        await _context.Entry(project).Collection(p => p.Tasks).LoadAsync();

        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project is null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ProjectResponseDto>> GetAllAsync()
    {
        var projects = await _context
                            .Projects
                            .Include(p => p.Tasks)
                            .ToListAsync();

        return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);

    }

    public async Task<IEnumerable<ProjectResponseDto>> GetAllForUserAsync(
        string userId, 
        IList<string> roles)
    {
        IQueryable<Project> query = _context.Projects
                                    .Include(p => p.Tasks);

        if (roles.Contains("Admin")) { }
        else if(roles.Contains("Manager"))
        {
            query = query.Where(p => p.OwnerId == userId);
        }
        else
        {
            query = query.Where(p => p.Members.Any(m => m.UserId == userId));
        }
        var projects = await query.ToListAsync();

        return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
    }

    public async Task<IEnumerable<AvailableUserDto>> GetAvailableUsersForAddAsync(int projectId)
    {
        var membersIds = await _context.ProjectMembers
                            .Where(m => m.ProjectId == projectId)
                            .Select(m => m.UserId)
                            .ToListAsync();

        var users = await _context.Users
                        .Where(u => !membersIds.Contains(u.Id))
                        .OrderBy(u => u.Email)
                        .Select(u => new AvailableUserDto
                        {
                            Id = u.Id,
                            Email = u.Email!,
                            FirstName = u.FirstName,
                            LastName = u.LastName
                        })
                        .ToListAsync();

        return users;
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await _context
                            .Projects
                            .Include(p => p.Tasks)
                            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (project is null) return null;

        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<IEnumerable<ProjectMemberResponseDto>> GetMembersAsync(int projectId)
    {
        var members = await _context
                             .ProjectMembers
                             .Include(m => m.User)
                             .Where(m => m.ProjectId == projectId)
                             .OrderBy(m => m.CreatedAt)
                             .ToListAsync();

        return members.Select(m => new ProjectMemberResponseDto
        {
            UserId = m.UserId,
            Email = m.User.Email!,
            FirstName = m.User.FirstName,
            LastName = m.User.LastName,
            JoinedAt = m.CreatedAt
        });
    }

    public async Task<Project?> GetProjectEntityAsync(int id)
    {
        return await _context.Projects
                    .Include(p => p.Tasks)
                    .Include(p => p.Members)
                    .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> RemoveMemberAsync(int projectId, string userId)
    {
        throw new NotImplementedException();
       
    }

    public async Task<ProjectResponseDto?> UpdateAsync(
        int id, 
        UpdateProjectRequest updateProjectRequest)
    {
        var updatedProject = await _context
                                     .Projects
                                     .Include(p => p.Tasks)
                                     .FirstOrDefaultAsync(p => p.Id == id);

        if (updatedProject is null) return null;

        _mapper.Map(updateProjectRequest, updatedProject);

        await _context.SaveChangesAsync();

        return _mapper.Map<ProjectResponseDto>(updatedProject);
    }
}
