@echo off
setlocal

set "TKPROPS_VARS=AppCenterTelemetry=no"

REM Flags to control skipping steps
set "SKIP1="
set "SKIP2="
set "SKIP3="

REM Build list of arguments to pass through to dotnet & python
set "PASSTHRU_ARGS="

REM ================================
REM  Parse arguments
REM  - skip1 / skip2 / skip3 -> control steps
REM  - everything else -> PASSTHRU_ARGS
REM ================================
:parseArgs
if "%~1"=="" goto argsDone

if /I "%~1"=="skip1" (
    set "SKIP1=1"
) else if /I "%~1"=="skip2" (
    set "SKIP2=1"
) else if /I "%~1"=="skip3" (
    set "SKIP3=1"
) else (
    if defined PASSTHRU_ARGS (
        set "PASSTHRU_ARGS=%PASSTHRU_ARGS% %~1"
    ) else (
        set "PASSTHRU_ARGS=%~1"
    )
)

shift
goto parseArgs

:argsDone

REM prepare colors
for /F "delims=" %%a in ('echo prompt $E^| cmd') do set "ESC=%%a"

echo.
echo %ESC%[32m==========================================%ESC%[0m
echo %ESC%[32m            Starting script...            %ESC%[0m
echo %ESC%[32m==========================================%ESC%[0m
echo.

if defined PASSTHRU_ARGS (
    echo %ESC%[90mPass-through args to apps: %PASSTHRU_ARGS%%ESC%[0m
    echo.
)

REM ====================================
REM Step 1: PowerShell Daily Job
REM ====================================
if defined SKIP1 goto step1_skip

echo %ESC%[32m[1/3] Running Daily Job...%ESC%[0m
powershell -NoProfile -ExecutionPolicy Bypass -File "C:\A\M\toolkit-scripts\tools\UpdateReposAndBuildWithDelay.ps1"
if errorlevel 1 goto :error
goto step1_done

:step1_skip
echo %ESC%[33m[1/3] Skipping Daily Job (skip1)%ESC%[0m

:step1_done
echo.

REM ====================================
REM Step 2: RepoReportGenerator
REM ====================================
if defined SKIP2 goto step2_skip

echo %ESC%[32m[2/3] Running RepoCoverageReportGenerator...%ESC%[0m
dotnet run --project "C:\Users\cam14754\Desktop\UnitTestingInternProject\CodeCoverageDashboard\RepoCoverageReportGenerator\RepoCoverageReportGenerator\RepoCoverageReportGenerator.csproj" -- %PASSTHRU_ARGS%
if errorlevel 1 goto :error
goto step2_done

:step2_skip
echo %ESC%[33m[2/3] Skipping RepoCoverageReportGenerator (skip2)%ESC%[0m

:step2_done
echo.

REM ====================================
REM Step 3: Python Script
REM ====================================
if defined SKIP3 goto step3_skip

echo %ESC%[32m[3/3] Running Python script...%ESC%[0m
py "C:\Users\cam14754\Desktop\UnitTestingInternProject\CodeCoverageDashboard\XMLToDBScript\main.py" %PASSTHRU_ARGS%
if errorlevel 1 goto :error
goto step3_done

:step3_skip
echo %ESC%[33m[3/3] Skipping Python script (skip3)%ESC%[0m

:step3_done
echo.
echo %ESC%[32m============================================%ESC%[0m
echo %ESC%[32m  All commands completed successfully %ESC%[0m
echo %ESC%[32m============================================%ESC%[0m
goto :end


:error
echo.
echo %ESC%[31m********************************************%ESC%[0m
echo %ESC%[31m  ERROR: One of the steps failed.%ESC%[0m
echo %ESC%[31m  Exit code: %errorlevel%%ESC%[0m
echo %ESC%[31m********************************************%ESC%[0m

:end
echo.
endlocal
pause
