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

set project_root_path=D:\Project\metaverseU3dMaster
if not exist %project_root_path% (
	echo 项目根路径配置错误, %project_root_path%不存在
	exit 1
)

set jenkins_project_root_path=D:\JenkinsWorkSpace\Project
if not exist %jenkins_project_root_path% (
	echo jenkins项目根路径配置错误, %jenkins_project_root_path%不存在
	exit 1
)

call :CheckProgress

call :BuildResources

call :UploadResources

call :PushVersionToGit
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
echo Unity进程处理完毕, 开始打资源
goto :eof

:BuildResources
%editor_path% -projectPath %project_path% -quit -batchmode -executeMethod BuildPackageTool.BuildResources -logFile %workspace%output.log
if %errorlevel%==0 ( 
	echo Unity资源构建成功！
) else ( 
	echo Unity资源构建失败！
	exit 1
)
goto :eof

:UploadResources
%editor_path% -projectPath %project_path% -quit -batchmode -executeMethod BuildPackageTool.UploadResources -logFile %workspace%output.log
if %errorlevel%==0 ( 
	echo Unity资源上传OSS成功！
) else ( 
	echo Unity资源上传OSS失败！
	exit 1
)
goto :eof

:PushVersionToGit
cd /d %project_root_path%
git fetch
git pull
copy %jenkins_project_root_path%\Client\ProjectSettings\ProjectSettings.asset %project_root_path%\Client\ProjectSettings\ProjectSettings.asset
copy %jenkins_project_root_path%\Client\Assets\GameMain\Configs\ResourceBuilder.xml %project_root_path%\Client\Assets\GameMain\Configs\ResourceBuilder.xml
git add Client\ProjectSettings\ProjectSettings.asset
git add Client\Assets\GameMain\Configs\ResourceBuilder.xml
git commit -m "Project Version Change"
git push origin feature-auto-build
if %errorlevel%==0 ( 
	echo Version推送成功！
) else ( 
	echo Version推送失败！
	exit 1
)
goto :eof