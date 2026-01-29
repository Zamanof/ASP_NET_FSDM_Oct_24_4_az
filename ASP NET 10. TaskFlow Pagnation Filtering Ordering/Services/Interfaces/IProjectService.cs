using ASP_NET_10._TaskFlow_Pagnation_Filtering_Ordering.DTOs.Project_DTOs;

namespace ASP_NET_10._TaskFlow_Pagnation_Filtering_Ordering.Services.Interfaces
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
