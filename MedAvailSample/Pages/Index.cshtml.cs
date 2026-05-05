using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MedAvailSample.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/ListPackages");
}
