echo off

set workspace=%~dp0

set editor_path="C:\Program Files\Unity\Hub\Editor\2021.3.16f1\Editor\Unity.exe"
if not exist %editor_path% (
	echo Unity路径配置错误, %editor_path%不存在
	exit 1
)

set project_path=D:\JenkinsWorkSpace\Project\Client
if not exist %project_path% (
	echo 项目路径配置错误, %project_path%不存在
	exit 1
)

call :CheckProgress

call :BuildResources

call :BuildPackage

exit

:CheckProgress
tasklist | findstr Unity.exe > nul
if %errorlevel%==0 ( 
	echo Unity运行中, 执行关闭操作
	taskkill /f /im Unity.exe
	REM 延时5s
	ping 127.0.0.1 -n 6 >nul
) else (
	echo Unity未运行
)
echo Unity进程处理完毕, 开始静默打包
goto :eof

:BuildResources
goto :eof

:BuildPackage
%editor_path% -projectPath %project_path% -quit -batchmode -executeMethod BuildPackageTool.BuildAndroid -logFile %workspace%output.log
if %errorlevel%==0 ( 
	echo Unity出包成功！
) else ( 
	echo Unity出包失败！
	exit 1
)
goto :eof