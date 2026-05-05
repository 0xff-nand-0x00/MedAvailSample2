# MedAvail Package Management — EF Core Demo

A dead-simple .NET 10 web application that demonstrates using **Entity Framework Core** with an existing **SQL Server** database. The app connects to the `MedAvailPackageManagementDb` database and provides a minimal UI for inserting and viewing `package_definition` records.

## Purpose

This is a **sample/demo application** — not production software. It exists to prove out that:

1. A .NET 10 Razor Pages app can connect to the existing MedAvail SQL Server database
2. Entity Framework Core can map to the existing `package_definition` table schema
3. Basic CRUD operations (Create + Read) work end-to-end through EF Core

There is **no authentication, no authorization, and no input validation** beyond what EF Core provides by default.

## Tech Stack

| Component | Version |
|---|---|
| .NET | 10.0 |
| ASP.NET Core Razor Pages | 10.0 |
| Entity Framework Core (SQL Server) | 10.0.7 |
| Bootstrap | 5.x (included via template) |
| SQL Server | on EC2 (Windows with SQL Server Standard) |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Network access to the SQL Server instance at `54.198.2.189:1433`
  - If running locally, ensure your IP is allowed through the EC2 security group (`niklele-DBConnector-setup-testInfra-security-group`)
  - If running inside the same VPC, you can switch to the private IP `172.31.38.132` in `appsettings.json`

## Quick Start

```bash
cd MedAvailSample
dotnet run
```

The app starts on `http://localhost:5050` (configured in `Properties/launchSettings.json`). Open it in your browser.

## Project Structure

```
MedAvailSample/
├── Program.cs                          # App entry point — registers EF Core DbContext + Razor Pages
├── appsettings.json                    # Connection string (SQL Server credentials)
├── MedAvailSample.csproj               # Project file — .NET 10, EF Core SQL Server package
│
├── Models/
│   └── PackageDefinition.cs            # EF Core entity mapped to dbo.package_definition
│
├── Data/
│   └── MedAvailDbContext.cs            # DbContext with PackageDefinitions DbSet
│
├── Pages/
│   ├── Index.cshtml / .cshtml.cs       # Landing page with navigation links
│   ├── AddPackage.cshtml / .cshtml.cs  # Form to insert a new package_definition record
│   ├── ListPackages.cshtml / .cshtml.cs# Table listing the latest 100 records
│   ├── Error.cshtml / .cshtml.cs       # Default error page
│   └── Shared/
│       └── _Layout.cshtml              # Shared layout with Bootstrap navbar
│
├── wwwroot/                            # Static assets (CSS, JS, Bootstrap, jQuery)
└── Properties/
    └── launchSettings.json             # Dev server configuration
```

## Pages

### Home (`/`)

Landing page with two buttons linking to the Add and List pages. Also accessible via the navbar.

### Add Package Definition (`/AddPackage`)

A form with five fields that map to the most important columns in `package_definition`:

| Field | Column | Description |
|---|---|---|
| Description | `description` | Free-text description of the package |
| Product Name | `product_name` | Name of the pharmaceutical product |
| Product Manufacturer | `product_manufacturer` | Manufacturer name |
| Product Code | `product_code` | NDC-format code (e.g., `12345-6789-01`) |
| Package Code | `package_code` | 12-digit barcode identifier |

Each field has a **🎲 Randomize** button that fills in a random value:
- **Product Name**: shuffles through 50 real pharmaceutical product names (e.g., "Amoxicillin 500mg Capsules")
- **Manufacturer**: shuffles through 50 real pharmaceutical manufacturers (e.g., "Pfizer Inc.")
- **Description**: picks from 10 lorem ipsum sentences
- **Product Code**: generates a random NDC-format code (`XXXXX-XXXX-XX`)
- **Package Code**: generates a random 12-digit barcode number

All other columns in `package_definition` are filled with sensible defaults (see `Models/PackageDefinition.cs` for the full mapping).

### List Package Definitions (`/ListPackages`)

Displays `package_definition` records with pagination (25 per page), ordered by ID descending. Includes Previous/Next controls and page number links. Columns shown:

- ID, Product Name, Manufacturer, Product Code, Package Code, Description, Created Date

## Database

### Connection

The connection string is in `appsettings.json`:

```
Server=54.198.2.189,1433;Database=MedAvailPackageManagementDb;User Id=atx_user;Password=Atx12345;TrustServerCertificate=True;
```

| Property | Value |
|---|---|
| Host | `54.198.2.189` (public IP of EC2 instance `i-002a343f6a997d009`) |
| Port | `1433` (default SQL Server) |
| Database | `MedAvailPackageManagementDb` |
| User | `atx_user` |
| Auth | SQL Server authentication |
| EC2 Region | `us-east-1` |
| EC2 Platform | Windows with SQL Server Standard |

### Schema

The database contains 25 tables. This demo app only interacts with **`dbo.package_definition`** (42 columns, identity PK on `package_definition_id`).

The full DDL is in `MedAvailPackageManagementDB.sql` at the repository root.

Key tables in the schema (for reference):

| Table | Description |
|---|---|
| `package_definition` | **Used by this app.** Master product/package definitions |
| `package` | Individual package instances tied to a definition |
| `PackageReservation` | Reservation records for packages at medcenters |
| `container` | Physical container definitions |
| `Packaging` | Packaging workflow records (fill, verify, etc.) |
| `lookup_*` | Reference/lookup tables (status, shape, UOM, drug schedule, etc.) |

### Entity Framework Core Mapping

The EF Core model (`Models/PackageDefinition.cs`) maps all 42 columns of `package_definition` using data annotations:

- `[Table("package_definition")]` — maps to the SQL table
- `[Column("column_name")]` — maps each property to its SQL column
- `[DatabaseGenerated(DatabaseGeneratedOption.Identity)]` — respects the `IDENTITY(1,1)` PK
- The `AuditID_MA` timestamp column is **not mapped** (it's managed by SQL Server automatically)

The DbContext is configured in `Program.cs` with a single line:

```csharp
builder.Services.AddDbContext<MedAvailDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedAvailDb")));
```

**Important**: This app does **not** use EF Core Migrations. The database schema already exists and is managed externally via the DDL script. EF Core is used in a "database-first" style with hand-written entity classes.

## Configuration

All configuration is in `appsettings.json`. The only setting you're likely to change:

| Key | Purpose | Default |
|---|---|---|
| `ConnectionStrings:MedAvailDb` | SQL Server connection string | Points to `54.198.2.189` |

To point at a different database instance, update the connection string. If running inside the same VPC as the EC2 instance, switch to the private IP `172.31.38.132`.

## Testing with curl

With the app running (`dotnet run`), you can test all pages from the command line:

### Verify pages load

```bash
# Index page
curl -s -o /dev/null -w "%{http_code}" http://localhost:5050/

# Add Package form
curl -s -o /dev/null -w "%{http_code}" http://localhost:5050/AddPackage

# List Packages
curl -s -o /dev/null -w "%{http_code}" http://localhost:5050/ListPackages
```

All should return `200`.

### Submit a new package definition

The form uses ASP.NET Core antiforgery tokens, so you need to grab the token first:

```bash
# 1. Get the antiforgery token and session cookie
TOKEN=$(curl -s -c /tmp/cookies.txt http://localhost:5050/AddPackage \
  | grep '__RequestVerificationToken' \
  | sed 's/.*value="\([^"]*\)".*/\1/')

# 2. POST the form with the token and cookie
curl -s -b /tmp/cookies.txt -X POST http://localhost:5050/AddPackage \
  -d "__RequestVerificationToken=$TOKEN" \
  -d "Package.Description=Test+package+from+curl" \
  -d "Package.ProductName=Ibuprofen+200mg+Tablets" \
  -d "Package.ProductManufacturer=Pfizer+Inc." \
  -d "Package.ProductCode=12345-6789-01" \
  -d "Package.PackageCode=999888777666" \
  -o /dev/null -w "%{http_code}"
```

A successful insert returns `302` (redirect back to the form). A `500` indicates a server error — check the `dotnet run` console output for details.

### Verify the record was created

```bash
curl -s http://localhost:5050/ListPackages | grep "Ibuprofen 200mg Tablets"
```

### Verify directly via SQL Server

If you have `sqlcmd` installed:

```bash
sqlcmd -S 54.198.2.189,1433 -U atx_user -P Atx12345 \
  -d MedAvailPackageManagementDb \
  -Q "SELECT package_definition_id, product_name, product_manufacturer FROM package_definition"
```

## Troubleshooting

### Cannot connect to SQL Server

1. **Security group**: Ensure your IP is allowed inbound on port 1433 in the EC2 security group `niklele-DBConnector-setup-testInfra-security-group` (sg-07d8f0370a82643fe)
2. **EC2 instance state**: Verify the instance `i-002a343f6a997d009` is running
3. **Windows Firewall**: The SQL Server instance runs on Windows — the Windows firewall must also allow port 1433
4. **VPN/Network**: If on a corporate network, port 1433 may be blocked — try from a direct internet connection

### EF Core errors about unmapped columns

The `AuditID_MA` column is a SQL Server `timestamp` (rowversion) column. It is intentionally not mapped in the EF Core model — EF Core automatically ignores database columns that have no corresponding CLR property.

### Build errors

Ensure you have .NET 10 SDK installed:

```bash
dotnet --version
# Should output 10.0.x
```
