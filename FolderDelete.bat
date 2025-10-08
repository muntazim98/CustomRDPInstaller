@echo off
setlocal

if "%~1"=="" exit /b

set "target=%~1"

timeout /t 4 /nobreak >nul 2>&1
rmdir /s /q "%target%" >nul 2>&1

endlocal
exit /b
