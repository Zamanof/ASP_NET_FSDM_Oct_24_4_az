using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.DTOs.Project_DTOs;

namespace ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Services
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
