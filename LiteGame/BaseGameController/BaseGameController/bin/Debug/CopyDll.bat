@echo off
echo --------Start--------
echo Current Path: %~dp0
set cur=%~dp0
cd %cur%
cd..
cd..
cd..
cd..
REM echo Delete Path: %cd%\DemoA\Assets\Plugins
REM del /q/a/f %cd%\DemoA\Assets\Plugins\*.*
REM if "%errorlevel%"=="0" (echo Delete Success) else (echo Delete Fail)
Copy %cur%\BaseGameController.dll %cd%\DemoA\Assets\Plugins\BaseGameController.dll
if "%errorlevel%"=="0" (echo Copy Dll Success) else (echo Copy Dll Fail)
Copy %cur%\BaseGameController.pdb %cd%\DemoA\Assets\Plugins\BaseGameController.pdb
if "%errorlevel%"=="0" (echo Copy Pdb Success) else (echo Copy Pdb Fail)