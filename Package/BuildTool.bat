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

call :CheckProgress

call :BuildResources

call :BuildPackage

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
echo Unity���̴������, ��ʼ��Ĭ���
goto :eof

:BuildResources
goto :eof

:BuildPackage
%editor_path% -projectPath %project_path% -quit -batchmode -executeMethod BuildPackageTool.BuildAndroid -logFile %workspace%output.log
if %errorlevel%==0 ( 
	echo Unity�����ɹ���
) else ( 
	echo Unity����ʧ�ܣ�
	exit 1
)
goto :eof