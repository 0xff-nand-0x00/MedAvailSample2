using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MedAvailSample.Data;
using MedAvailSample.Models;

namespace MedAvailSample.Pages;

public class ListPackagesModel(MedAvailDbContext db) : PageModel
{
    public const int PageSize = 25;

    public List<PackageDefinition> Packages { get; set; } = [];
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public async Task OnGetAsync()
    {
        if (PageNumber < 1) PageNumber = 1;

        TotalCount = await db.PackageDefinitions.CountAsync();

        if (TotalPages > 0 && PageNumber > TotalPages)
            PageNumber = TotalPages;

        Packages = await db.PackageDefinitions
            .AsNoTracking()
            .OrderByDescending(p => p.PackageDefinitionId)
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, int pageNumber)
    {
        var pkg = await db.PackageDefinitions.FindAsync(id);
        if (pkg != null)
        {
            db.PackageDefinitions.Remove(pkg);
            await db.SaveChangesAsync();
        }
        return RedirectToPage(new { pageNumber });
    }
}
