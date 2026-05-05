using System;
using Microsoft.EntityFrameworkCore;
using MedAvailSample.Models;

namespace MedAvailSample.Data;

public class MedAvailDbContext(DbContextOptions<MedAvailDbContext> options) : DbContext(options)
{
    static MedAvailDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<PackageDefinition> PackageDefinitions => Set<PackageDefinition>();

    // Note: The DB has an AuditID_MA timestamp/rowversion column on package_definition.
    // EF Core ignores it automatically since there's no corresponding CLR property.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PackageDefinition>(entity =>
        {
            entity.ToTable("package_definition", "public");

            entity.Property(e => e.PackageDefinitionId).HasColumnName("package_definition_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PackageCode).HasColumnName("package_code");
            entity.Property(e => e.PackageCodeAbsLocationId).HasColumnName("package_code_abs_location_id");
            entity.Property(e => e.PackageHeight).HasColumnName("package_height");
            entity.Property(e => e.PackageWidth).HasColumnName("package_width");
            entity.Property(e => e.PackageLength).HasColumnName("package_length");
            entity.Property(e => e.Shape).HasColumnName("shape");
            entity.Property(e => e.Cap).HasColumnName("cap").HasConversion<int>();
            entity.Property(e => e.CapDiameter).HasColumnName("cap_diameter");
            entity.Property(e => e.CapLength).HasColumnName("cap_length");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.FragileScale).HasColumnName("fragile_scale");
            entity.Property(e => e.LotCodeAbsLocation).HasColumnName("lot_code_abs_location");
            entity.Property(e => e.LotCodeRelLocation).HasColumnName("lot_code_rel_location");
            entity.Property(e => e.ExpiryAbsLocation).HasColumnName("expiry_abs_location");
            entity.Property(e => e.ExpiryRelLocation).HasColumnName("expiry_rel_location");
            entity.Property(e => e.Valid).HasColumnName("valid").HasConversion<int>();
            entity.Property(e => e.DefinitionStateId).HasColumnName("definition_state_id");
            entity.Property(e => e.DefinitionRejectReason).HasColumnName("definition_reject_reason");
            entity.Property(e => e.ProductCategoryId).HasColumnName("product_category_id");
            entity.Property(e => e.ProductName).HasColumnName("product_name");
            entity.Property(e => e.ProductCodeTypeId).HasColumnName("product_code_type_id");
            entity.Property(e => e.ProductCode).HasColumnName("product_code");
            entity.Property(e => e.ProductManufacturer).HasColumnName("product_manufacturer");
            entity.Property(e => e.PackageSize).HasColumnName("package_size");
            entity.Property(e => e.PackageSizeUomId).HasColumnName("package_size_uom_id");
            entity.Property(e => e.DrugSchedule).HasColumnName("drug_schedule");
            entity.Property(e => e.ChangedDate).HasColumnName("changed_date");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.ExpirationMethod).HasColumnName("expiration_method");
            entity.Property(e => e.DaysToAdvisedExpiration).HasColumnName("days_to_advised_expiration");
            entity.Property(e => e.DrugScheduleId).HasColumnName("drug_schedule_id");
            entity.Property(e => e.ControlledSubstance).HasColumnName("controlled_substance").HasConversion<int>();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn).HasColumnName("created_on");
            entity.Property(e => e.LotCodeSource).HasColumnName("lot_code_source");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PackageDefinitionTypeId).HasColumnName("package_definition_type_id");
            entity.Property(e => e.IsDemoPackage).HasColumnName("is_demo_package").HasConversion<int>();
        });
    }
}
