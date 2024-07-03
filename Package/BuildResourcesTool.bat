echo off

set workspace=%~dp0

set editor_path="C:\Program Files\Unity\Hub\Editor\2021.3.16f1\Editor\Unity.exe"
if not exist %editor_path% (
	echo Unity·�����ô���, %editor_path%������
	exit 1
)

set project_path=D:\JenkinsWorkSpace\Project\Client
if not exist %project_path% (
	echo ��Ŀ·�����ô���, %project_path%������
	exit 1
)

set project_root_path=D:\Project\metaverseU3dMaster
if not exist %project_root_path% (
	echo ��Ŀ��·�����ô���, %project_root_path%������
	exit 1
)

set jenkins_project_root_path=D:\JenkinsWorkSpace\Project
if not exist %jenkins_project_root_path% (
	echo jenkins��Ŀ��·�����ô���, %jenkins_project_root_path%������
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
	echo Unity������, ִ�йرղ���
	taskkill /f /im Unity.exe
	REM ��ʱ5s
	ping 127.0.0.1 -n 6 >nul
) else (
	echo Unityδ����
)
echo Unity���̴������, ��ʼ����Դ
goto :eof

:BuildResources
%editor_path% -projectPath %project_path% -quit -batchmode -executeMethod BuildPackageTool.BuildResources -logFile %workspace%output.log
if %errorlevel%==0 ( 
	echo Unity��Դ�����ɹ���
) else ( 
	echo Unity��Դ����ʧ�ܣ�
	exit 1
)
goto :eof

:UploadResources
%editor_path% -projectPath %project_path% -quit -batchmode -executeMethod BuildPackageTool.UploadResources -logFile %workspace%output.log
if %errorlevel%==0 ( 
	echo Unity��Դ�ϴ�OSS�ɹ���
) else ( 
	echo Unity��Դ�ϴ�OSSʧ�ܣ�
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
	echo Version���ͳɹ���
) else ( 
	echo Version����ʧ�ܣ�
	exit 1
)
goto :eof