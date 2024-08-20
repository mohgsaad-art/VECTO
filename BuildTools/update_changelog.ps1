param([string]$RELEASE_VERSION=$null)

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
                $(Get-Content $contentFile).Replace("##", $newHeader)
              }
          }
      } |
  Set-Content $targetFile
}

# Set project default variables.
$CI_API_V4_URL="https://code.europa.eu/api/v4"
$CI_PROJECT_ID=89

if(!$RELEASE_VERSION){
    $numberOfDays = $(New-TimeSpan -Start $(Get-Date -Year 2015 -Month 1 -Day 1) -End $(Get-Date)).Days
    $RELEASE_VERSION = Read-Host "Version number (build number is $numberOfDays)"
}

# If multiple tokens are set for different repos rename $env:GITLAB_API_TOKEN by GITLAB_API_TOKEN_{CURRENT_REPO}
$GITLAB_API_TOKEN = $env:GITLAB_API_TOKEN
if(!$GITLAB_API_TOKEN){
    throw "GITLAB_API_TOKEN not assigned."
}

# Get release version and suffix from version string.
$VersionTag = $RELEASE_VERSION
$VersionTag -match '((\d+)\.\d+\.\d+\.\d+)(-(RC|DEV))?' > $null
$VersionNumber = $Matches[1]
$MajorVersionNumber = $Matches[2]
$VersionSuffix = $Matches[4]
$IsReleaseCandidate = $VersionSuffix -eq "RC"
$IsReleaseDeveloper = $VersionSuffix -eq "DEV"

# Get version string to include in changelog
if($IsReleaseCandidate -or $IsReleaseDeveloper) {
    $changelogVersion = "VECTO v$VersionNumber-$VersionSuffix"
} else {
    $changelogVersion = "VECTO v$VersionNumber Official Release"
}

# PREVIOUS_RELEASE_SHA is the commit the previous release tag points to.
# CURRENT_RELEASE_SHA  is the commit the current release tag points to.
$tags = @($(git tag -l --sort=-v:refname) | Where-Object { $_.Contains("Release/v$MajorVersionNumber") })
$CI_COMMIT_SHA = $(git rev-parse --verify HEAD)
$CURRENT_RELEASE_SHA = $CI_COMMIT_SHA
$PREVIOUS_RELEASE_SHA  = $(git rev-list -1 "tags/$($tags[0])")

if($CI_COMMIT_TAG){
  $CURRENT_RELEASE_SHA = $(git rev-list -1 "tags/$($tags[0])")
  $PREVIOUS_RELEASE_SHA  = $(git rev-list -1 "tags/$($tags[1])")
}

Write-Host "Current version points to $($CURRENT_RELEASE_SHA)"
Write-Host "Previous version points to $($PREVIOUS_RELEASE_SHA)"

# Get the latest changes for the release changelog.
$response = Invoke-WebRequest "$($CI_API_V4_URL)/projects/$($CI_PROJECT_ID)/repository/changelog?version=$($changelogVersion)&from=$PREVIOUS_RELEASE_SHA&to=$CURRENT_RELEASE_SHA" `
-UseBasicParsing `
-Method "Get" `
-Headers @{
    Accept          = 'application/json'
    "PRIVATE-TOKEN" = "$GITLAB_API_TOKEN"
}

$ReleaseNotesUpdateMarkdown = "Documentation/User Manual Source/ReleaseNotesMDs/release_notes.md";
if($(Test-Path $ReleaseNotesUpdateMarkdown)){
  Clear-Content $ReleaseNotesUpdateMarkdown
}

($response | ConvertFrom-Json).notes > ./$ReleaseNotesUpdateMarkdown

# Update Release Notes and changelog markdowns.
# Based on the major, determine the ReleaseNotes for the given version.
if ($MajorVersionNumber -ne 3 -and $MajorVersionNumber -ne 4){
    throw "Release Notes version ${MajorVersionNumber} not supported."
} else {
    $ReleaseNotesMarkdown = "Documentation/User Manual Source/ReleaseNotesMDs/ReleaseNotesVecto${MajorVersionNumber}x.md"
}

# Insert new changelog features into Release Notes.
$InjectNewFeaturesMark = "<!-- Cover Slide -->"
Update-MarkdownContent $ReleaseNotesMarkdown $ReleaseNotesUpdateMarkdown $InjectNewFeaturesMark

# Insert new changelog features into VECTO changelog.
$ChangelogInjectMark = "# Changelog"
$ChangelogFilePath = "Documentation/User Manual/6-changelog/changelog.md"
Update-MarkdownContent $ChangelogFilePath $ReleaseNotesUpdateMarkdown $ChangelogInjectMark

# Stage the modified files by the script in git.
git add $ReleaseNotesMarkdown
git add $ChangelogFilePath
git add $ReleaseNotesUpdateMarkdown

Write-Host "----- Next release tag -----"
Write-Host "     ${VersionTag}"