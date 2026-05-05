using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using MedAvailSample.Data;
using MedAvailSample.Models;
using MedAvailSample.Pages;

namespace MedAvailSample.Tests;

public class PackageDefinitionModelTests
{
    [Fact]
    public void Defaults_AreSetCorrectly()
    {
        var pkg = new PackageDefinition();

        Assert.True(pkg.Valid);
        Assert.True(pkg.IsDemoPackage);
        Assert.Equal("demo_user", pkg.ChangedBy);
        Assert.Equal("demo_user", pkg.CreatedBy);
        Assert.Equal(1, pkg.DefinitionStateId);
        Assert.Equal(1, pkg.PackageDefinitionTypeId);
    }
}

public class DbContextTests
{
    private static MedAvailDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MedAvailDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new MedAvailDbContext(options);
    }

    [Fact]
    public async Task CanInsertAndRetrievePackageDefinition()
    {
        using var db = CreateInMemoryContext();

        db.PackageDefinitions.Add(new PackageDefinition
        {
            Description = "Test Package",
            ProductName = "Aspirin 100mg",
            ProductManufacturer = "TestCorp",
            ProductCode = "12345-6789-01",
            PackageCode = "123456789012"
        });
        await db.SaveChangesAsync();

        var result = await db.PackageDefinitions.FirstAsync();
        Assert.Equal("Test Package", result.Description);
        Assert.Equal("Aspirin 100mg", result.ProductName);
        Assert.Equal("TestCorp", result.ProductManufacturer);
    }

    [Fact]
    public async Task MultipleInserts_GetDistinctIds()
    {
        using var db = CreateInMemoryContext();

        db.PackageDefinitions.Add(new PackageDefinition { Description = "A", ProductName = "A", ProductManufacturer = "A", ProductCode = "A", PackageCode = "A" });
        db.PackageDefinitions.Add(new PackageDefinition { Description = "B", ProductName = "B", ProductManufacturer = "B", ProductCode = "B", PackageCode = "B" });
        await db.SaveChangesAsync();

        var all = await db.PackageDefinitions.ToListAsync();
        Assert.Equal(2, all.Count);
        Assert.NotEqual(all[0].PackageDefinitionId, all[1].PackageDefinitionId);
    }
}

/// Test subclass that bypasses raw SQL lookup queries (SQL Server-specific)
file class TestableAddPackageModel(MedAvailDbContext db) : AddPackageModel(db, Microsoft.Extensions.Logging.Abstractions.NullLogger<AddPackageModel>.Instance)
{
    protected override Task ApplyLookupDefaults(PackageDefinition pkg) => Task.CompletedTask;
}

public class AddPackagePageTests
{
    private static (AddPackageModel model, MedAvailDbContext db) CreateModel()
    {
        var options = new DbContextOptionsBuilder<MedAvailDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new MedAvailDbContext(options);
        var model = new TestableAddPackageModel(db);
        model.TempData = new TestTempDataDictionary();
        model.PageContext = new PageContext { HttpContext = new DefaultHttpContext() };
        return (model, db);
    }

    private static PackageDefinition ValidPackage() => new()
    {
        Description = "Test",
        ProductName = "TestDrug 50mg",
        ProductManufacturer = "PharmaCo",
        ProductCode = "11111-2222-33",
        PackageCode = "999999999999"
    };

    [Fact]
    public async Task OnPostAsync_SavesPackageAndRedirects()
    {
        var (model, db) = CreateModel();
        model.Package = ValidPackage();

        var result = await model.OnPostAsync();

        Assert.IsType<RedirectToPageResult>(result);
        Assert.Single(db.PackageDefinitions);

        var saved = await db.PackageDefinitions.FirstAsync();
        Assert.Equal("TestDrug 50mg", saved.ProductName);
        Assert.Equal("demo_user", saved.CreatedBy);
        Assert.Equal("demo_user", saved.ChangedBy);
        Assert.True(saved.IsDemoPackage);
        Assert.True(saved.CreatedOn > DateTime.MinValue);
        Assert.True(saved.ChangedDate > DateTime.MinValue);
    }

    [Fact]
    public async Task OnPostAsync_SetsTempDataSuccessMessage()
    {
        var (model, _) = CreateModel();
        model.Package = ValidPackage();

        await model.OnPostAsync();

        Assert.Contains("TestDrug 50mg", model.TempData["Success"] as string);
    }

    [Fact]
    public async Task OnPostAsync_InvalidModelState_ReturnsPage()
    {
        var (model, db) = CreateModel();
        model.Package = ValidPackage();
        model.ModelState.AddModelError("Package.ProductName", "Required");

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.Empty(db.PackageDefinitions);
    }
}

public class ListPackagesPageTests
{
    private static (MedAvailDbContext db, ListPackagesModel model) Create()
    {
        var options = new DbContextOptionsBuilder<MedAvailDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new MedAvailDbContext(options);
        var model = new ListPackagesModel(db);
        return (db, model);
    }

    private static async Task SeedPackages(MedAvailDbContext db, int count)
    {
        for (int i = 0; i < count; i++)
            db.PackageDefinitions.Add(new PackageDefinition
            {
                Description = $"Pkg {i}", ProductName = $"Drug {i}",
                ProductManufacturer = "M", ProductCode = $"C{i}", PackageCode = $"P{i}"
            });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task OnGetAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var (_, model) = Create();
        await model.OnGetAsync();
        Assert.Empty(model.Packages);
        Assert.Equal(0, model.TotalCount);
    }

    [Fact]
    public async Task OnGetAsync_ReturnsPackagesOrderedByIdDesc()
    {
        var (db, model) = Create();
        await SeedPackages(db, 5);

        await model.OnGetAsync();

        Assert.Equal(5, model.Packages.Count);
        Assert.True(model.Packages[0].PackageDefinitionId > model.Packages[4].PackageDefinitionId);
    }

    [Fact]
    public async Task OnGetAsync_Page1_ReturnsPageSize()
    {
        var (db, model) = Create();
        await SeedPackages(db, 60);

        model.PageNumber = 1;
        await model.OnGetAsync();

        Assert.Equal(ListPackagesModel.PageSize, model.Packages.Count);
        Assert.Equal(60, model.TotalCount);
        Assert.Equal(3, model.TotalPages);
    }

    [Fact]
    public async Task OnGetAsync_LastPage_ReturnsRemainder()
    {
        var (db, model) = Create();
        await SeedPackages(db, 60);

        model.PageNumber = 3;
        await model.OnGetAsync();

        Assert.Equal(10, model.Packages.Count); // 60 - 25 - 25 = 10
    }

    [Fact]
    public async Task OnGetAsync_PageZero_ClampsToOne()
    {
        var (db, model) = Create();
        await SeedPackages(db, 5);

        model.PageNumber = 0;
        await model.OnGetAsync();

        Assert.Equal(1, model.PageNumber);
        Assert.Equal(5, model.Packages.Count);
    }

    [Fact]
    public async Task OnGetAsync_PageBeyondTotal_ClampsToLastPage()
    {
        var (db, model) = Create();
        await SeedPackages(db, 30); // 2 pages

        model.PageNumber = 99;
        await model.OnGetAsync();

        Assert.Equal(2, model.PageNumber);
        Assert.Equal(5, model.Packages.Count); // 30 - 25 = 5
    }

    [Fact]
    public async Task OnPostDeleteAsync_RemovesPackage()
    {
        var (db, model) = Create();
        await SeedPackages(db, 1);
        var id = (await db.PackageDefinitions.FirstAsync()).PackageDefinitionId;

        var result = await model.OnPostDeleteAsync(id, 1);

        Assert.IsType<RedirectToPageResult>(result);
        Assert.Empty(db.PackageDefinitions);
    }

    [Fact]
    public async Task OnPostDeleteAsync_NonexistentId_StillRedirects()
    {
        var (_, model) = Create();
        var result = await model.OnPostDeleteAsync(9999, 1);
        Assert.IsType<RedirectToPageResult>(result);
    }
}

/// Simple ITempDataDictionary backed by a Dictionary for unit tests.
file class TestTempDataDictionary : ITempDataDictionary
{
    private readonly Dictionary<string, object?> _data = new();
    public object? this[string key] { get => _data.TryGetValue(key, out var v) ? v : null; set => _data[key] = value; }
    public ICollection<string> Keys => _data.Keys;
    public ICollection<object?> Values => _data.Values;
    public int Count => _data.Count;
    public bool IsReadOnly => false;
    public void Add(string key, object? value) => _data.Add(key, value);
    public void Add(KeyValuePair<string, object?> item) => _data.Add(item.Key, item.Value);
    public void Clear() => _data.Clear();
    public bool Contains(KeyValuePair<string, object?> item) => _data.Contains(item);
    public bool ContainsKey(string key) => _data.ContainsKey(key);
    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex) { }
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _data.GetEnumerator();
    public void Keep() { }
    public void Keep(string key) { }
    public void Load() { }
    public object? Peek(string key) => this[key];
    public bool Remove(string key) => _data.Remove(key);
    public bool Remove(KeyValuePair<string, object?> item) => _data.Remove(item.Key);
    public void Save() { }
    public bool TryGetValue(string key, out object? value) => _data.TryGetValue(key, out value);
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _data.GetEnumerator();
}
