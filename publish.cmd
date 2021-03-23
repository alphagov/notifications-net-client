@ECHO OFF

IF "%publish%"=="" SET publish=""
IF %publish% NEQ true (
   echo No need to publish
   EXIT /B
)

dotnet pack -c=Release %APPVEYOR_BUILD_FOLDER%\src\GovukNotify\GovukNotify.csproj -o=publish

FOR %%i IN ("%APPVEYOR_BUILD_FOLDER%/src/GovukNotify/publish/*.nupkg") DO (
    set filename=%%~nxi
)

nuget push "%APPVEYOR_BUILD_FOLDER%/src/GovukNotify/publish/%filename%" -Source https://api.nuget.org/v3/index.json -apikey %NUGET_API_KEY%
