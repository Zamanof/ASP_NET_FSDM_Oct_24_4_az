using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.DTOs.Project_DTOs;

namespace ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Services.Interfaces
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
