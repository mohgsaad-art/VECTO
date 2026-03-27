<#
.SYNOPSIS
    Updates all Release Notes related files.

.DESCRIPTION
    Updates all Release Notes related files.

    Such as:
    - CHANGES.md
    - Directory.Build.props
    - Documentation/User Manual/6-changelog/changelog.md
    - Documentation/User Manual Source/Release Notes Vecto4.x.pdf
    - Documentation/User Manual Source/ReleaseNotesMDs/release_notes.md (manually updated if custom content needed)

.PARAMETER Force
    Enables custom modification of all Release Notes files by overriding git cliff output with release_notes.md content.

.EXAMPLE
    ./BuildTools/update_changelog.ps1 4.5.5

.EXAMPLE
    ./BuildTools/update_changelog.ps1 4.5.5 -Force
#>

param([string]$RELEASE_VERSION=$null, [string]$PREVIOUS_RELEASE_VERSION=$null, [switch]$Force)

function Update-MarkdownContent ([string] $targetFile, [string] $contentFile, [string] $injectionMarker = ""){
  $changelogAdded = $false
  if(!$(Test-Path $targetFile)){
    Write-Host "Markdown file not updated because it does not exist" -ForegroundColor DarkYellow
    Write-Host "File: '${targetFile}'" -ForegroundColor DarkYellow
    return
  }

  (Get-Content $targetFile) |
      Foreach-Object {
          if($injectionMarker -eq ""){
              if(-not $changelogAdded){
                  $(Get-Content $contentFile)
                  $changelogAdded = $true
              }
              $_ # Send the current line to output
          } else {
              $_ # Send the current line to output
              if($_ -match $injectionMarker){
                if ($targetFile.Contains("changelog.md")) {
                    $newHeader = "##"
                } else {
                    $newHeader = "#"
                }
                "`r`n"
                $(Get-Content $contentFile).Replace("##", $newHeader)
              }
          }
      } |
  Set-Content $targetFile
}

function Update-BuildPropsVersion([string]$version) {
    $VersionRgx = "<Version>\d+\.\d+\.\d+<\/Version>"
    $VersionNode = "<Version>$version</Version>"

    (Get-Content -Path $BuildPropsFile) |
        ForEach-Object {$_ -Replace $VersionRgx, $VersionNode} |
            Set-Content -Path $BuildPropsFile
}

function Remove-CliffArtifacts([string] $filename){
    # When git trailer/footers are empty git cliff produces undesired artifacts that need removal.
    $ArtifactFreeChangelog = $(Get-Content $filename).Replace("description::", ":").Replace("CodeEU : ", "")

    # Gitlab issue closing pattern: https://code.europa.eu/help/user/project/issues/managing_issues.md#default-closing-pattern
    $GITLAB_ISSUE_CLOSING_REGEX = "\b((?:[Cc]los(?:e[sd]?|ing)|\b[Ff]ix(?:e[sd]|ing)?|\b[Rr]esolv(?:e[sd]?|ing)|\b[Ii]mplement(?:s|ed|ing)?)(:?)) "
    $ArtifactFreeChangelog -replace $GITLAB_ISSUE_CLOSING_REGEX | Set-Content $filename
}

if(!$RELEASE_VERSION){
    $RELEASE_VERSION = Read-Host "Version number"
    $PREVIOUS_RELEASE_VERSION = Read-Host "Previous version number (optional)"
}

# Get release version and suffix from version string.
$VersionTag = $RELEASE_VERSION
$VersionTag -match '((\d+)\.\d+\.\d+(\.\d+)?)(-(RC|DEV))?' > $null
$VersionNumber = $Matches[1]
$MajorVersionNumber = $Matches[2]
$VersionSuffix = $Matches[5]
$IsReleaseCandidate = $VersionSuffix -eq "RC"
$IsReleaseDeveloper = $VersionSuffix -eq "DEV"

$PreviousVersionTag = $PREVIOUS_RELEASE_VERSION
$PreviousVersionTag -match '((\d+)\.\d+\.\d+(\.\d+)?)(-(RC|DEV))?' > $null
$PreviousMajorVersionNumber = $Matches[2]

# Get version string to include in changelog
if($IsReleaseCandidate -or $IsReleaseDeveloper) {
    $changelogVersion = "VECTO v$VersionNumber-$VersionSuffix"
} else {
    $changelogVersion = "VECTO v$VersionNumber Official Release"
}

# PREVIOUS_RELEASE_SHA is the commit the previous release tag points to.
# CURRENT_RELEASE_SHA  is the commit the current release tag points to.
$tags = @($(git tag -l --sort=-v:refname) | Where-Object { $_.Contains("Release/v$MajorVersionNumber") })
if($null -ne $PREVIOUS_RELEASE_VERSION -and $PREVIOUS_RELEASE_VERSION -ne ""){
    $previous_tags = @($(git tag -l --sort=-v:refname) | Where-Object { $_.Contains("Release/v$PreviousMajorVersionNumber") })
}

$CI_COMMIT_SHA = $(git rev-parse --verify HEAD)
$CURRENT_RELEASE_SHA = $CI_COMMIT_SHA

# Set previous release SHA based on PREVIOUS_RELEASE_VERSION tag.
if($null -ne $PREVIOUS_RELEASE_VERSION -and $PREVIOUS_RELEASE_VERSION -ne ""){
    # If previous release version is provided, use it to find the previous tag.
    $PREVIOUS_RELEASE_SHA  = $(git rev-list -1 "tags/$($previous_tags[0])")
} else{
    $PREVIOUS_RELEASE_SHA  = $(git rev-list -1 "tags/$($tags[0])")
}

if($CI_COMMIT_TAG){
  $CURRENT_RELEASE_SHA = $(git rev-list -1 "tags/$($tags[0])")
  $PREVIOUS_RELEASE_SHA  = $(git rev-list -1 "tags/$($tags[1])")
}

Write-Host "Current version points to $($CURRENT_RELEASE_SHA)"
Write-Host "Previous version points to $($PREVIOUS_RELEASE_SHA)"

$CliffReleaseNotesMarkdown = "Documentation/User Manual Source/ReleaseNotesMDs/release_notes.md";
$CliffReleaseNotesHtmlMarkdown = "Documentation/User Manual Source/ReleaseNotesMDs/release_notes_html.md";
if(-not $Force){
    # Get the latest changes for the release changelog.
    git cliff "$CURRENT_RELEASE_SHA..$PREVIOUS_RELEASE_SHA" --unreleased --tag "$changelogVersion" -o $CliffReleaseNotesMarkdown --config ./BuildTools/cliff.toml
    git cliff "$CURRENT_RELEASE_SHA..$PREVIOUS_RELEASE_SHA" --unreleased --tag "$changelogVersion" -o $CliffReleaseNotesHtmlMarkdown --config ./BuildTools/cliff_html.toml

    Remove-CliffArtifacts $CliffReleaseNotesMarkdown
    Remove-CliffArtifacts $CliffReleaseNotesHtmlMarkdown
}

# Update Release Notes and changelog markdowns.
# Based on the major, determine the ReleaseNotes for the given version.
$ReleaseNotesPdfMarkdown = "Documentation/User Manual Source/ReleaseNotesMDs/ReleaseNotesVecto${MajorVersionNumber}x.md"
$ReleaseNotesPdf = "Documentation/User Manual Source/Release Notes Vecto${MajorVersionNumber}.x.pdf"
if ($(Test-Path -Path $ReleaseNotesPdfMarkdown) -eq $false) {
    throw "Release Notes version ${MajorVersionNumber} not supported. Consider creating 'ReleaseNotesVecto${MajorVersionNumber}x.md' file."
}

# Declare files to update.
$UserManualHtml = "Documentation/User Manual/help.html"
$ChangelogMarkdownPath = "Documentation/User Manual/6-changelog/changelog.md"
$BuildPropsFile = "Directory.Build.props"

# Reset files content to clean previous executions output.
git reset -- $ChangelogMarkdownPath $ChangesMarkdown $ReleaseNotesPdf $UserManualHtml $ReleaseNotesPdfMarkdown -q
git checkout -- $ChangelogMarkdownPath $ChangesMarkdown $ReleaseNotesPdf $UserManualHtml $ReleaseNotesPdfMarkdown

# Insert new changelog features into Release Notes.
$InjectNewFeaturesMark = "<!-- Cover Slide -->"
Update-MarkdownContent $ReleaseNotesPdfMarkdown $CliffReleaseNotesMarkdown $InjectNewFeaturesMark

# Insert new changelog features into VECTO changelog.
$ChangelogInjectMark = "# Changelog"
Update-MarkdownContent $ChangelogMarkdownPath $CliffReleaseNotesHtmlMarkdown $ChangelogInjectMark

$ChangesMarkdown = "CHANGES.md"
Copy-Item $ChangelogMarkdownPath $ChangesMarkdown -Force

# Convert md to pdf.
Push-Location "Documentation/User Manual Source/ReleaseNotesMDs"
pandoc "..\..\..\$ReleaseNotesPdfMarkdown" -o "..\..\..\$ReleaseNotesPdf" --css "..\..\..\BuildTools\templates\md-style.css" --pdf-engine=$Env:weasyprint  --title="Changelog"
Pop-Location

# Create Release Notes Dev for Vecto GUI help dialog.
$HelpDialogReleaseNotesPdf = "Documentation/User Manual/Release Notes Vecto.pdf"
Copy-Item $ReleaseNotesPdf $HelpDialogReleaseNotesPdf -Force

# User Manual HTML conversion script.
Push-Location "Documentation/User Manual/"
& './convert.bat'
Pop-Location

Update-BuildPropsVersion $VersionNumber

# Stage the modified files by the script in git.
git add $CliffReleaseNotesMarkdown
git add $HelpDialogReleaseNotesPdf
git add $ReleaseNotesPdfMarkdown
git add $ChangelogMarkdownPath
git add $ChangesMarkdown
git add $ReleaseNotesPdf
git add $UserManualHtml
git add $BuildPropsFile
