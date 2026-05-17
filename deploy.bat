@echo off
echo ========================================
echo   JinguMod Deploy Script (dev build)
echo ========================================
echo.

set "TOOL_DIR=%~dp0"
set "TOOL_DIR=%TOOL_DIR:~0,-1%"
for %%I in ("%TOOL_DIR%") do set "GAME_DIR=%%~dpI"
set "GAME_DIR=%GAME_DIR:~0,-1%"

set "SRC_DIR=%TOOL_DIR%\src"
set "PATCHER_DIR=%SRC_DIR%\Patcher\bin\Release"

if not exist "%GAME_DIR%\JinGu.exe" (
    echo [ERROR] JinGu.exe not found in %GAME_DIR%
    echo         Make sure this tool folder is placed inside the game root directory.
    pause
    exit /b 1
)

echo Game directory : %GAME_DIR%
echo.

echo [1/4] Building Stubs...
powershell -Command "& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' '%SRC_DIR%\Stubs\Stubs.csproj' /p:Configuration=Release /verbosity:minimal"

echo [2/4] Building all projects...
powershell -Command "& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' '%TOOL_DIR%\JinguPatcher.sln' /p:Configuration=Release /verbosity:minimal"

echo [3/4] Packaging to tool directory...
if not exist "%TOOL_DIR%\Mods" mkdir "%TOOL_DIR%\Mods"
copy /Y "%SRC_DIR%\JinguMod\bin\Release\JinguMod.dll" "%TOOL_DIR%\JinguMod.dll"
copy /Y "%SRC_DIR%\Patch\bin\Release\JinguModPatch.dll" "%TOOL_DIR%\Mods\JinguModPatch.dll"
copy /Y "%PATCHER_DIR%\JinguPatcher.exe" "%TOOL_DIR%\JinguPatcher.exe"
copy /Y "%PATCHER_DIR%\Mono.Cecil.dll" "%TOOL_DIR%\Mono.Cecil.dll"

echo [4/4] Running JinguPatcher...
"%TOOL_DIR%\JinguPatcher.exe"

echo.
echo ========================================
echo   Deploy complete!
echo ========================================
pause
