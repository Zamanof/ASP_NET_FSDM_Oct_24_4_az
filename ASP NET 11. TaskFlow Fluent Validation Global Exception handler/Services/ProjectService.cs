using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Data;
using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.DTOs.Project_DTOs;
using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Models;
using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Services;

public class ProjectService : IProjectService
{

    private readonly TaskFlowDbContext _context;
    private readonly IMapper _mapper;

    public ProjectService(TaskFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProjectResponseDto> CreateAsync(CreateProjectRequest createProjectRequest)
    {
        var project = _mapper.Map<Project>(createProjectRequest);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        await _context
                    .Entry(project)
                    .Collection(p => p.Tasks)
                    .LoadAsync();

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

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await _context
                            .Projects
                            .Include(p => p.Tasks)
                            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (project is null) return null;

        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<ProjectResponseDto?> UpdateAsync(int id, UpdateProjectRequest updateProjectRequest)
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
