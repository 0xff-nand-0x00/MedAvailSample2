# Next Steps

## Overview

The solution transformation appears to have completed successfully. Both projects in the solution — `MedAvailSample` and `MedAvailSample.Tests` — produced no build errors after the transformation to cross-platform .NET.

## Validation Steps

### 1. Restore Dependencies

Run the following command from the solution root to ensure all NuGet packages are properly restored:

```bash
dotnet restore
```

Review the output for any warnings related to missing packages or version conflicts.

### 2. Build the Solution

Perform a full solution build to confirm the error-free state is consistent across all configurations:

```bash
dotnet build --configuration Release
```

Verify that the output confirms zero errors and review any warnings that may indicate deprecated APIs or compatibility concerns.

### 3. Run the Test Suite

Execute the tests in `MedAvailSample.Tests` to validate that the application logic behaves correctly under the new runtime:

```bash
dotnet test --configuration Release --verbosity normal
```

Review the test results for:
- Any tests that were previously passing but are now failing
- Any tests that were skipped or ignored
- Code coverage if a coverage tool is configured

### 4. Review Target Framework

Open both `.csproj` files and confirm the `<TargetFramework>` element is set to the intended cross-platform .NET version (e.g., `net8.0`):

```xml
<TargetFramework>net8.0</TargetFramework>
```

Ensure both projects are targeting the same framework version to avoid compatibility mismatches.

### 5. Check for Removed or Changed APIs

Even without build errors, some APIs behave differently between .NET Framework and modern .NET. Specifically, review the application code for:

- Any usage of `System.Configuration.ConfigurationManager` (requires the `System.Configuration.ConfigurationManager` NuGet package)
- Any usage of `System.Web` namespaces, which are not available in cross-platform .NET
- Any P/Invoke or platform-specific code that may only function correctly on Windows

### 6. Run the Application Manually

Execute the main application directly to validate runtime behavior:

```bash
dotnet run --project MedAvailSample/MedAvailSample.csproj --configuration Release
```

Step through the primary workflows of the application and confirm the output matches the expected behavior from the legacy version.

### 7. Verify Output Artifacts

Check the `bin/Release` output directory to confirm the expected assemblies, configuration files, and any other required assets are present and correctly structured.

```bash
ls MedAvailSample/bin/Release/
```

### 8. Address Any Runtime Warnings

Even if the build is clean, run the application with detailed logging enabled and review the output for any runtime exceptions, unhandled edge cases, or behavioral regressions that unit tests may not cover.