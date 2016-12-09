function Get-ProjectVersion([string] $ProjectName)
{
    $project = Get-Project -Name $ProjectName
    $assemblyPath = $project.ProjectItems.Item("Properties").ProjectItems.Item("AssemblyInfo.cs").Properties.Item("FullPath").Value

    $assemblyInfo = Get-Content -Path $assemblyPath
    if (!$assemblyInfo)
    {
        return $null
    }                

    $versionMatches = [regex]::Match($assemblyInfo, "\[assembly\: AssemblyVersion\(`"((\d+)\.(\d+)\.(\d+)\.?(\d*)){1}`"\)\]")

    if (!$versionMatches -or !$versionMatches.Groups -or $versionMatches.Groups.Count -le 0)
    {
        return $null
    }

    return $versionMatches.Groups[1].Value
}