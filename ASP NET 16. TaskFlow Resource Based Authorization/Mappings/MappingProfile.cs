using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Project_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Task_Items_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;
using AutoMapper;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Mappings;

public class MappingProfile:Profile
{
	public MappingProfile()
	{
		// Project

		CreateMap<Project, ProjectResponseDto>()
			.ForMember(dest=>dest.TaskCount, opt=> opt.MapFrom(src=> src.Tasks.Count()));

		CreateMap<CreateProjectRequest, Project>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
			.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
			.ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<UpdateProjectRequest, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

		// TaskItem

		CreateMap<TaskItem, TaskItemResponseDto>()
			.ForMember(dest=> dest.Status, opt=> opt.MapFrom(src=> src.Status.ToString()))
			.ForMember(dest=> dest.Priority, opt=> opt.MapFrom(src=> src.Priority.ToString()))
			.ForMember(dest=> dest.ProjectName, opt=> opt.MapFrom(src=> src.Project.Name));

        CreateMap<CreateTaskItemRequest, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Project, opt => opt.Ignore());

        CreateMap<UpdateTaskItemRequest, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectId, opt => opt.Ignore());
    }
}
