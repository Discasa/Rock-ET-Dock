@echo off
setlocal

set "APP_NAME=Rock ET Dock"
set "CONFIG_ROOT=%LOCALAPPDATA%\%APP_NAME%"
set "CONFIG_FILE=%CONFIG_ROOT%\dock.config.json"
set "BACKUP_ROOT=%CONFIG_ROOT%-backups"

echo.
echo Resetting %APP_NAME% configuration for the current Windows user.
echo Managed dock items under "%USERPROFILE%\%APP_NAME%" will be preserved.
echo.

taskkill /IM "%APP_NAME%.exe" /F >nul 2>nul

if not exist "%CONFIG_ROOT%" (
    echo No configuration folder found:
    echo "%CONFIG_ROOT%"
) else (
    for /f %%I in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd-HHmmss"') do set "STAMP=%%I"
    set "BACKUP_DIR=%BACKUP_ROOT%\%STAMP%"

    mkdir "%BACKUP_DIR%" >nul 2>nul
    xcopy "%CONFIG_ROOT%\*" "%BACKUP_DIR%\" /E /I /H /Y >nul

    if exist "%CONFIG_FILE%" del /F /Q "%CONFIG_FILE%"
    if exist "%CONFIG_ROOT%\logs" rmdir /S /Q "%CONFIG_ROOT%\logs"

    echo Previous configuration backup:
    echo "%BACKUP_DIR%"
)

reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Run" /v "%APP_NAME%" /f >nul 2>nul

echo.
echo Done. Start %APP_NAME% again to create a fresh default dock configuration.
echo.
pause

endlocal
