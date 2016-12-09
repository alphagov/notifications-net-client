. .\get_project_version.ps1

$currentVersion = Get-ProjectVersion -ProjectName "Notify"

# Assumes you have added the Nuget Bintray source
nuget push "Notify.$currentVersion.nupkg" -Source Bintray