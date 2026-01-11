using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_02._Dependency_injection.Pages;

public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;
    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

