@echo off
setlocal

cd /d "%~dp0"
dotnet run --project "src\RockETDock.App\RockETDock.App.csproj"

endlocal
