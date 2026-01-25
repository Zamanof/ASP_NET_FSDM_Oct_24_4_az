namespace ASP_NET_08._TaskFlow_DTO.DTOs.Task_Items_DTOs;

public class CreateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProjectId { get; set; }
}
