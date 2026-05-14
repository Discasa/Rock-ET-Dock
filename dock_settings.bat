@echo off
setlocal
pushd "%~dp0"
dotnet run --project "src\RockETDock.App\RockETDock.App.csproj" -- --settings
popd
