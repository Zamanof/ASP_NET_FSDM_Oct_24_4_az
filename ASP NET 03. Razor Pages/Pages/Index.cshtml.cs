using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_03._Razor_Pages.Pages;

public class IndexModel : PageModel
{
    public Person person1 { get; set; } = new() { Name = "Qadir", Age = 25 };
    public void OnGet(Person person)
    {
        ViewData["Name"] = person.Name;
        ViewData["Age"] = person.Age;
    }
    public string Foo(string str) => str.Replace('a', 'e');
}
