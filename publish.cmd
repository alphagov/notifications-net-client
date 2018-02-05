@ECHO OFF

IF "%publish%"=="" SET publish=""
IF %publish% NEQ true (
   echo No need to publish
   EXIT /B
)

dotnet pack -c=Release C:\projects\notifications-net-client\src\Notify\Notify.csproj -o=publish

FOR %%i IN ("C:/projects/notifications-net-client/src/Notify/publish/*.nupkg") DO (
    set filename=%%~nxi
)

nuget push "C:/projects/notifications-net-client/src/Notify/publish/%filename%" -Source https://api.bintray.com/nuget/gov-uk-notify/nuget -apikey %BINTRAY_API_KEY%
