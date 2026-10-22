# Snipe-IT Playwright Automation (.NET 10)

Interview assessment automation for the Snipe-IT public demo (Global360).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PowerShell (Windows) or `pwsh` (macOS/Linux)

## Setup

From the **repository root**:

```powershell
dotnet restore
dotnet build
pwsh SnipeIt.Tests/bin/Debug/net10.0/playwright.ps1 install
```

### If pwsh is not available

```powershell
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

## Run the test

```powershell
$env:HEADED = "1"
dotnet test
```
