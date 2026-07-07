# Build and Publish VeterinariaPDF with ClickOnce

# Find MSBuild
$msbuildPaths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
)

$msbuild = $null
foreach ($path in $msbuildPaths) {
    if (Test-Path $path) {
        $msbuild = $path
        Write-Host "Found MSBuild at: $msbuild" -ForegroundColor Green
        break
    }
}

if ($null -eq $msbuild) {
    Write-Host "ERROR: MSBuild not found. Please install Visual Studio." -ForegroundColor Red
    exit 1
}

# Project path
$projectPath = "VeterinariaPDF\VeterinariaPDF.csproj"

# Build in Release mode
Write-Host "`nBuilding project in Release mode..." -ForegroundColor Cyan
& $msbuild $projectPath /p:Configuration=Release /t:Rebuild /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`nBuild completed successfully!" -ForegroundColor Green

# Publish with ClickOnce
Write-Host "`nPublishing with ClickOnce..." -ForegroundColor Cyan
& $msbuild $projectPath /t:Publish /p:Configuration=Release /p:PublishDir="bin\Release\app.publish\" /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`nPublish completed successfully!" -ForegroundColor Green
Write-Host "`nDeployment files are in: VeterinariaPDF\bin\Release\app.publish\" -ForegroundColor Yellow
