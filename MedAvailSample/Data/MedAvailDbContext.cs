using Microsoft.EntityFrameworkCore;
using MedAvailSample.Models;

namespace MedAvailSample.Data;

public class MedAvailDbContext(DbContextOptions<MedAvailDbContext> options) : DbContext(options)
{
    public DbSet<PackageDefinition> PackageDefinitions => Set<PackageDefinition>();

    // Note: The DB has an AuditID_MA timestamp/rowversion column on package_definition.
    // EF Core ignores it automatically since there's no corresponding CLR property.
}
