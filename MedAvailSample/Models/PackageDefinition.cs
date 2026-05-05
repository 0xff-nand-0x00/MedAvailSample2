using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedAvailSample.Models;

[Table("package_definition")]
public class PackageDefinition
{
    [Key]
    [Column("package_definition_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PackageDefinitionId { get; set; }

    [Column("description")]
    [Required, MaxLength(255)]
    public string Description { get; set; } = "";

    [Column("package_code")]
    [Required, MaxLength(255)]
    public string PackageCode { get; set; } = "";

    [Column("package_code_abs_location_id")]
    public int PackageCodeAbsLocationId { get; set; } = 1;

    [Column("package_height")]
    public decimal PackageHeight { get; set; } = 1.0m;

    [Column("package_width")]
    public decimal PackageWidth { get; set; } = 1.0m;

    [Column("package_length")]
    public decimal PackageLength { get; set; } = 1.0m;

    [Column("shape")]
    public int Shape { get; set; } = 1;

    [Column("cap")]
    public bool Cap { get; set; }

    [Column("cap_diameter")]
    public decimal CapDiameter { get; set; }

    [Column("cap_length")]
    public decimal CapLength { get; set; }

    [Column("weight")]
    public decimal Weight { get; set; } = 1.0m;

    [Column("fragile_scale")]
    public int FragileScale { get; set; } = 1;

    [Column("lot_code_abs_location")]
    public int LotCodeAbsLocation { get; set; } = 1;

    [Column("lot_code_rel_location")]
    public int LotCodeRelLocation { get; set; } = 1;

    [Column("expiry_abs_location")]
    public int ExpiryAbsLocation { get; set; } = 1;

    [Column("expiry_rel_location")]
    public int ExpiryRelLocation { get; set; } = 1;

    [Column("valid")]
    public bool Valid { get; set; } = true;

    [Column("definition_state_id")]
    public int DefinitionStateId { get; set; } = 1;

    [Column("definition_reject_reason")]
    [MaxLength(255)]
    public string? DefinitionRejectReason { get; set; }

    [Column("product_category_id")]
    public int ProductCategoryId { get; set; } = 1;

    [Column("product_name")]
    [Required, MaxLength(1000)]
    public string ProductName { get; set; } = "";

    [Column("product_code_type_id")]
    public int ProductCodeTypeId { get; set; } = 1;

    [Column("product_code")]
    [Required, MaxLength(255)]
    public string ProductCode { get; set; } = "";

    [Column("product_manufacturer")]
    [Required, MaxLength(255)]
    public string ProductManufacturer { get; set; } = "";

    [Column("package_size")]
    public decimal PackageSize { get; set; } = 1.0m;

    [Column("package_size_uom_id")]
    public int PackageSizeUomId { get; set; } = 1;

    [Column("drug_schedule")]
    [MaxLength(255)]
    public string? DrugSchedule { get; set; }

    [Column("changed_date")]
    public DateTime ChangedDate { get; set; }

    [Column("changed_by")]
    [Required, MaxLength(50)]
    public string ChangedBy { get; set; } = "demo_user";

    [Column("expiration_method")]
    public int? ExpirationMethod { get; set; }

    [Column("days_to_advised_expiration")]
    public int? DaysToAdvisedExpiration { get; set; }

    [Column("drug_schedule_id")]
    public int? DrugScheduleId { get; set; }

    [Column("controlled_substance")]
    public bool ControlledSubstance { get; set; }

    [Column("created_by")]
    [Required, MaxLength(50)]
    public string CreatedBy { get; set; } = "demo_user";

    [Column("created_on")]
    public DateTime CreatedOn { get; set; }

    [Column("lot_code_source")]
    public int LotCodeSource { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("package_definition_type_id")]
    public int PackageDefinitionTypeId { get; set; } = 1;

    [Column("is_demo_package")]
    public bool IsDemoPackage { get; set; } = true;
}
