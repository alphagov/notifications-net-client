. .\get_project_version.ps1

$currentVersion = Get-ProjectVersion -ProjectName "Notify"

nuget pack ..\src\Notify\Notify.csproj -Build -properties Configuration=Release -version "$currentVersion"