<#
.SYNOPSIS
    Builds and manages beta NuGet packages for the local registry.

    .\build_beta_release.ps1 4.5.5 -Publish
    .\build_beta_release.ps1 4.5.5 -Delete

.DESCRIPTION
    Builds the VECTO solution as a beta release and manages NuGet packages
    in the local-registry source.

.PARAMETER Version
    Version number (e.g. 4.5.5). Required for -Publish and -Delete.

.PARAMETER Setup
    Register 'local-registry' as a NuGet source (idempotent).

.PARAMETER Publish
    Build the solution and push NuGet packages to local-registry.

.PARAMETER Delete
    Delete VectoCore, VectoCommon and VectoHashing from local-registry for the given version.

.PARAMETER Republish
    Delete and re-publish NuGet packages for the given version. Equivalent to running -Delete followed by -Publish.

.PARAMETER LocalRegistryPath
    Path to the local NuGet feed folder. Prompted if not provided when using -Setup.

.PARAMETER Debug
    Include debug symbols in nuget package
	
.PARAMETER ReleaseCandidateBuild
	Set compile flag for release candidate version (disables deprecated XML versions, for example)

.PARAMETER Help
    Display full help for this script.

.EXAMPLE
    .\build_beta_release.ps1 -Setup

.EXAMPLE
    .\build_beta_release.ps1 -Version 4.5.5 -Publish

.EXAMPLE
    .\build_beta_release.ps1 -Version 4.5.5 -Publish -Debug

.EXAMPLE
    .\build_beta_release.ps1 -Version 4.5.5 -Delete

.EXAMPLE
    .\build_beta_release.ps1 -Version 4.5.5 -Republish

.EXAMPLE
    .\build_beta_release.ps1 -Version 4.5.5 -Setup -Publish

.EXAMPLE
    .\build_beta_release.ps1 -Help
#>
param(
    [switch]$Setup,
    [switch]$Publish,
    [switch]$Delete,
    [switch]$Debug,
	[switch]$ReleaseCandidateBuild,
    [switch]$Republish,
    [switch]$Help,
    [string]$Version = $null,
    [string]$LocalRegistryPath = $null
)

# docker run -dit --mount type=bind,source="C:\Users\David Amo\Documents\EC\Development\vecto_aclone",target="C:\Users\Public\Documents\vecto" code.europa.eu:4567/vecto/vectoci/vecto-dev-win:v3 pwsh -NoLogo -NonInteractive -File "C:\Users\Public\Documents\vecto\build_release.ps1"

function Add-LocalNugetRegistry([string]$path) {
    $existing = dotnet nuget list source | Select-String "local-vecto-registry"
    if (-not $existing) {
        if (-not (Test-Path $path)) {
            New-Item -ItemType Directory -Path $path | Out-Null
        }
        dotnet nuget add source $path --name "local-vecto-registry"
        Write-Host "Added NuGet source 'local-vecto-registry' at $path" -ForegroundColor Cyan
    } else {
        Write-Host "NuGet source 'local-vecto-registry' already configured, skipping." -ForegroundColor Yellow
    }
}

function Remove-LocalNugetPackages([string]$version) {
    $packages = @("VectoCore", "VectoCommon", "VectoHashing")
    foreach ($pkg in $packages) {
        Write-Host "Deleting package: $pkg $version" -ForegroundColor Magenta
        dotnet nuget delete $pkg $version -s "local-vecto-registry" --non-interactive
    }
}

function Update-BuildPropsSymbols([switch]$includeSymbols){
    $BuildPropsFile = "Directory.Build.props"
    $DebugRgx = "<DebugSymbols>(?:tru|fals)e<\/DebugSymbols>"
    $DebugNode = "<DebugSymbols>true</DebugSymbols>"


    (Get-Content -Path $BuildPropsFile) |
        ForEach-Object {$_ -Replace $DebugRgx, $DebugNode} |
            Set-Content -Path $BuildPropsFile

    $BuildPropsFile = "Directory.Build.props"
    $IncludeRgx = "<IncludeSymbols>(?:tru|fals)e<\/IncludeSymbols>"
    $IncludeNode = "<IncludeSymbols>true</IncludeSymbols>"


    (Get-Content -Path $BuildPropsFile) |
        ForEach-Object {$_ -Replace $IncludeRgx, $IncludeNode} |
            Set-Content -Path $BuildPropsFile

    $DebugTypeRgx = "<DebugType>*<\/DebugType>"
    $DebugTypeNode = "<DebugType>full</DebugType>"

    (Get-Content -Path $BuildPropsFile) |
        ForEach-Object {$_ -Replace $DebugTypeRgx, $DebugTypeNode} |
            Set-Content -Path $BuildPropsFile

}

function Update-BuildPropsVersion([string]$version) {
    $BuildPropsFile = "Directory.Build.props"
    $VersionRgx = "<Version>\d+\.\d+\.\d+<\/Version>"
    $VersionNode = "<Version>$version</Version>"

    (Get-Content -Path $BuildPropsFile) |
        ForEach-Object {$_ -Replace $VersionRgx, $VersionNode} |
            Set-Content -Path $BuildPropsFile
}

function Set-ReleaseVersion() {
    if(!$Version) {
        $Version = Read-Host "Beta version number (e.g. 4.5.5)"
    }

    return $Version + "-beta"
}

if ($Help) {
    Get-Help $PSCommandPath -Full
    exit 0
}

if (-not $Setup -and -not $Publish -and -not $Delete -and -not $Republish) {
    Get-Help $PSCommandPath
    exit 0
}

Push-Location $PSScriptRoot/..

if ($Setup) {
    if (-not $LocalRegistryPath) {
        $LocalRegistryPath = Read-Host "Local registry path (e.g. C:\NuGet\a-local-nuget-registry)"
    }

    Add-LocalNugetRegistry $LocalRegistryPath
}

if($Republish) {
    $VersionNumber = Set-ReleaseVersion
    Write-Host "Republishing package $VersionNumber" -ForegroundColor Yellow
    $Delete = $true
    $Publish = $true
}

if ($Delete) {
    $VersionNumber = Set-ReleaseVersion
    Remove-LocalNugetPackages $VersionNumber
    Write-Host "Restored packages might persist in the local nuget cache, i.e. $env:USERPROFILE\.nuget\packages." -ForegroundColor Yellow
}

if ($Debug) {
    Write-Host "Added Debug Symbols to NuGet package" -ForegroundColor Yellow
    Update-BuildPropsSymbols $Debug
}

if ($ReleaseCandidateBuild) {
	Write-Host "Building ReleaseCandidate version"
	$ReleaseType = "RELEASE_CANDIDATE"
}

if ($Publish) {
    $VersionNumber = Set-ReleaseVersion

    # Clean nuget folder contents
    Remove-Item -LiteralPath "./Nuget" -Force -Recurse

    # Prepare sln for beta release
    Update-BuildPropsVersion $VersionNumber
    dotnet sln VECTO.sln remove "../VECTO-Article10_DEV/Article10Notifications/Article10Notifications.csproj" "../vecto-SchemaDocumentation/ParameterDocumentation/ParameterDocumentation.csproj" "../vecto-SchemaDocumentation/ParameterDocumentationTest/ParameterDocumentationTest.csproj"
    dotnet build "VECTO.sln" -v detailed -c Deploy /p:RunCodeAnalysis=false /p:WarningLevel=0 /p:MainProductVersion=$VersionNumber /p:VersionPrefix=$VersionNumber /p:DefineConstants=$ReleaseType /p:RepositoryCommit=$(git rev-parse HEAD)

    # Push nuget packages to local registry
    $NugetReleaseVersion = $(Get-ChildItem -Path ./Nuget)[0].BaseName
    Get-ChildItem -Path "Nuget/$NugetReleaseVersion" | ForEach-Object {
        Write-Host "Pushing package: $($_.FullName)" -ForegroundColor Green
        dotnet nuget push $_.FullName -s "local-vecto-registry"
    }



    # Revert all changes made to the repo
    git restore .\VECTO.sln
    git restore .\Directory.Build.props
}

if($Delete){
    Write-Host "Clearing nuget cache" -ForegroundColor Yellow
    dotnet nuget locals all --clear
}