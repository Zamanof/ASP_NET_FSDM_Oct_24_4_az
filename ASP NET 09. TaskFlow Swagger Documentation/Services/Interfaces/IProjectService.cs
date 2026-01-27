using ASP_NET_09._TaskFlow_Swagger_Documentation.DTOs.Project_DTOs;

namespace ASP_NET_09._TaskFlow_Swagger_Documentation.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
        Task<ProjectResponseDto?> GetByIdAsync(int id);
        Task<ProjectResponseDto> CreateAsync(CreateProjectRequest createProjectRequest);
        Task<ProjectResponseDto?> UpdateAsync(int id, UpdateProjectRequest updateProjectRequest);
        Task<bool> DeleteAsync(int id);
    }
}
