using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MedAvailSample.Data;
using MedAvailSample.Models;

namespace MedAvailSample.Pages;

public class AddPackageModel(MedAvailDbContext db, ILogger<AddPackageModel> logger) : PageModel
{
    // Allowlist of valid lookup table/column pairs for FirstId queries
    private static readonly Dictionary<string, string> AllowedLookups = new()
    {
        ["lookup_code_absolute_location"] = "code_abs_location_id",
        ["lookup_code_relative_location"] = "code_rel_location_id",
        ["lookup_package_shape"] = "shape_id",
        ["lookup_definition_state"] = "definition_state_id",
        ["lookup_product_category"] = "product_category_id",
        ["lookup_product_code_type"] = "product_code_type_id",
        ["lookup_package_size_uom"] = "package_size_uom_id",
        ["lookup_lot_code_source"] = "lotcode_source_id",
    };

    [BindProperty]
    public PackageDefinition Package { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await ApplyLookupDefaults(Package);

        Package.ChangedDate = DateTime.UtcNow;
        Package.CreatedOn = DateTime.UtcNow;
        Package.ChangedBy = "demo_user";
        Package.CreatedBy = "demo_user";
        Package.IsDemoPackage = true;

        try
        {
            db.PackageDefinitions.Add(Package);
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to save package definition");
            ModelState.AddModelError("", "Failed to save to database. Please try again.");
            return Page();
        }

        TempData["Success"] = $"Package definition '{Package.ProductName}' created with ID {Package.PackageDefinitionId}.";
        return RedirectToPage("/ListPackages");
    }

    protected virtual async Task ApplyLookupDefaults(PackageDefinition pkg)
    {
        var absLocationId = await FirstId("lookup_code_absolute_location");
        var relLocationId = await FirstId("lookup_code_relative_location");

        pkg.PackageCodeAbsLocationId = absLocationId;
        pkg.LotCodeAbsLocation = absLocationId;
        pkg.ExpiryAbsLocation = absLocationId;
        pkg.LotCodeRelLocation = relLocationId;
        pkg.ExpiryRelLocation = relLocationId;
        pkg.Shape = await FirstId("lookup_package_shape");
        pkg.DefinitionStateId = await FirstId("lookup_definition_state");
        pkg.ProductCategoryId = await FirstId("lookup_product_category");
        pkg.ProductCodeTypeId = await FirstId("lookup_product_code_type");
        pkg.PackageSizeUomId = await FirstId("lookup_package_size_uom");
        pkg.LotCodeSource = await FirstId("lookup_lot_code_source");
    }

    private async Task<int> FirstId(string table)
    {
        if (!AllowedLookups.TryGetValue(table, out var column))
            throw new ArgumentException($"Unknown lookup table: {table}");

        var result = await db.Database
            .SqlQueryRaw<int>($"SELECT TOP 1 [{column}] AS [Value] FROM [dbo].[{table}]")
            .FirstOrDefaultAsync();
        return result;
    }
}
