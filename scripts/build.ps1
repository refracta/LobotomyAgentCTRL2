param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$ProjectPath = Join-Path $RepoRoot "src\refracta_AgentCTRL2_MOD\refracta_AgentCTRL2_MOD.csproj"
$ModFolderName = "refracta_AgentCTRL2_MOD"
$ModTemplatePath = Join-Path $RepoRoot ("mod\" + $ModFolderName)
$BuildOutputDir = Join-Path $RepoRoot ("out\" + $Configuration)
$PackageRoot = Join-Path $RepoRoot ("dist\" + $ModFolderName)
$ZipPath = Join-Path $RepoRoot ("dist\" + $ModFolderName + ".zip")

if (Test-Path $BuildOutputDir) {
    Remove-Item -Path $BuildOutputDir -Recurse -Force
}
if (Test-Path $PackageRoot) {
    Remove-Item -Path $PackageRoot -Recurse -Force
}
if (Test-Path $ZipPath) {
    Remove-Item -Path $ZipPath -Force
}

New-Item -ItemType Directory -Path $BuildOutputDir -Force | Out-Null
New-Item -ItemType Directory -Path (Split-Path -Parent $PackageRoot) -Force | Out-Null

dotnet build $ProjectPath -c $Configuration -o $BuildOutputDir
if ($LASTEXITCODE -ne 0) {
    throw "Build failed."
}

$BuiltDllPath = Join-Path $BuildOutputDir ($ModFolderName + ".dll")
if (-not (Test-Path $BuiltDllPath)) {
    throw "Built dll not found: $BuiltDllPath"
}

New-Item -ItemType Directory -Path (Join-Path $PackageRoot "Info\kr") -Force | Out-Null
if (Test-Path (Join-Path $ModTemplatePath "Info\en\Info.xml")) {
    New-Item -ItemType Directory -Path (Join-Path $PackageRoot "Info\en") -Force | Out-Null
}
Copy-Item -Path $BuiltDllPath -Destination (Join-Path $PackageRoot ($ModFolderName + ".dll")) -Force
Copy-Item -Path (Join-Path $ModTemplatePath "Info\kr\Info.xml") -Destination (Join-Path $PackageRoot "Info\kr\Info.xml") -Force
if (Test-Path (Join-Path $ModTemplatePath "Info\en\Info.xml")) {
    Copy-Item -Path (Join-Path $ModTemplatePath "Info\en\Info.xml") -Destination (Join-Path $PackageRoot "Info\en\Info.xml") -Force
}

Compress-Archive -Path $PackageRoot -DestinationPath $ZipPath -Force

Write-Output ("Built DLL: " + $BuiltDllPath)
Write-Output ("Zip: " + $ZipPath)
