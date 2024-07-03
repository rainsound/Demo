using Aliyun.OSS;
using Aliyun.OSS.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

public static class BuildPackageTool
{
    private const string RESOURCES_OUTPUT_PATH = "D:/Resources";

    private const string ACCESS_KEY_ID = "";

    private const string ACCESS_KEY_SECRET = "";

    private const string END_POINT = "oss-cn-hangzhou.aliyuncs.com";

    private const string BUCKET = "metaverse3d";

    private const string BUILD_OUTPUT_PATH = "D:/BuildPackage";

    private const string KEYSTORE_PATH = "AndroidKeyStore/user.keystore";

    [MenuItem("Tools/AutoBuild/BuildResources")]
    public static void BuildResources()
    {
        AddVerionCode();
        ResourceBuilderController controller = new ResourceBuilderController();
        controller.Load();
        controller.Platforms = Platform.Android;
        controller.OutputDirectory = RESOURCES_OUTPUT_PATH;
        //controller.InternalResourceVersion += 1; //Load时会+1
        controller.CompressionHelperTypeName = "UnityGameFramework.Runtime.DefaultCompressionHelper";
        controller.BuildEventHandlerTypeName = "GameClient.Editor.BuildEventHandler";
        if (controller.RefreshCompressionHelper() == false || controller.RefreshBuildEventHandler() == false)
        {
            return;
        }
        controller.Save();
        if (controller.BuildResources())
        {
            controller.Save();
        }
        else
        {
            throw new Exception("Build Resources Error"); 
        }
    }

    [MenuItem("Tools/AutoBuild/UploadResources")]
    public static void UploadResources()
    {
        var config = new ClientConfiguration();
        var client = new OssClient(END_POINT, ACCESS_KEY_ID, ACCESS_KEY_SECRET, config);
        try
        {
            string localConfigFile = $"{RESOURCES_OUTPUT_PATH}/AndroidVersion.txt";
            string bucketConfigFilePath = $"{Application.version}/AndroidVersion.txt";
            client.PutObject(BUCKET, bucketConfigFilePath, localConfigFile);

            ResourceBuilderController controller = new ResourceBuilderController();
            controller.Load();
            string targetPath = $"Full/{Application.version.Replace('.', '_')}_{controller.InternalResourceVersion - 1}";
            string fileDirPath = $"{RESOURCES_OUTPUT_PATH}/{targetPath}";
            string[] files = Directory.GetFiles(fileDirPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string bucketFilePath = file.Remove(0, RESOURCES_OUTPUT_PATH.Length + 1).Replace('\\','/');
                Debug.Log(bucketFilePath);
                client.PutObject(BUCKET, bucketFilePath, file);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Put object fail, caused by: {ex}");
        }
        
    }

    private static void AddVerionCode()
    {
        string version = PlayerSettings.bundleVersion;
        int androidVersion = PlayerSettings.Android.bundleVersionCode;
        //PlayerSettings.iOS.buildNumber //IOS Version
        //PlayerSettings.macOS.buildNumber //Mac Version
        if (string.IsNullOrEmpty(version))
        {
            return;
        }
        var versionStrs = version.Split('.');
        if (versionStrs != null && versionStrs.Length > 0)
        {
            var versionEndStr = versionStrs[versionStrs.Length - 1];
            if (int.TryParse(versionEndStr, out int versionEnd))
            {
                string newVersion = version.Remove(version.Length - versionEndStr.Length) + (++versionEnd).ToString();
                PlayerSettings.bundleVersion = newVersion;
                PlayerSettings.Android.bundleVersionCode = ++androidVersion;
                if (Type.GetType("UnityEditor.EditorApplication, UnityEditor") != null)
                {
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    [MenuItem("Tools/AutoBuild/BuildPackage/Android")]
    public static void BuildAndroid()
    {
        Debug.Log("Start Build");
        if (CheckPlatform(BuildTarget.Android) == false)
        {
            Debug.LogError("Build target error!");
            return;
        }
        if (CheckKeystoreFile(BuildTarget.Android) == false)
        {
            Debug.LogError("Android keystore File is Empty!");
            return;
        }
        BuildPackage(BuildTarget.Android);
    }

    [MenuItem("Tools/AutoBuild/BuildPackage/Force/Android")]
    public static void ForceBuildAndroid()
    {
        Debug.Log("Start Force Build");
        if (CheckKeystoreFile(BuildTarget.Android) == false)
        {
            Debug.LogError("Android keystore File is Empty!");
            return;
        }
        if (TrySwitchPlatform(BuildTarget.Android) == false)
        {
            Debug.LogError("Switch build target error!");
            return;
        }
        BuildPackage(BuildTarget.Android);
    }

    private static void BuildPackage(BuildTarget target)
    {
        var date = DateTime.Now;
        string dataStr = $"{date.Year}{date.Month:D2}{date.Day:D2}{date.Hour:D2}{date.Minute:D2}{date.Second:D2}";
        var outputPath = $"{BUILD_OUTPUT_PATH}\\{target}\\{dataStr}";
        if (CheckOutputPath(outputPath) == false)
        {
            return;
        }
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
                outputPath = $"{outputPath}\\Start.exe";
                break;
            case BuildTarget.iOS:
                outputPath = $"{outputPath}\\Release-{dataStr}.ipa";
                break;
            case BuildTarget.Android:
                outputPath = $"{outputPath}\\Release-{dataStr}.apk";
                break;
            default:
                break;
        }

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = GetScenes();
        options.locationPathName = outputPath;
        options.target = target;
        options.targetGroup = BuildPipeline.GetBuildTargetGroup(target);
        options.options = BuildOptions.None;
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report == null)
        {
            throw new Exception($"Unsupported packaging target:{target}");
        }
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("打包成功!");
        }
        else
        {
            throw new Exception("Build Fail !!");
        }
    }

    private static bool CheckPlatform(BuildTarget target)
    {
        return target == EditorUserBuildSettings.activeBuildTarget;
    }

    private static bool TrySwitchPlatform(BuildTarget target)
    {
        if (target != EditorUserBuildSettings.activeBuildTarget)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
                    return true;
                case BuildTarget.iOS:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                    return true;
                case BuildTarget.Android:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    return true;
                default:
                    return false;
            }
        }
        return true;
    }

    private static bool CheckKeystoreFile(BuildTarget target)
    {
        if (target == BuildTarget.Android && PlayerSettings.Android.useCustomKeystore)
        {
            if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass))
            {
                if (File.Exists(KEYSTORE_PATH))
                {
                    PlayerSettings.Android.keystoreName = KEYSTORE_PATH;
                    PlayerSettings.Android.keystorePass = "dxzSZDvZxqUMLHOv8k9YEWQiv6oiaR";
                    PlayerSettings.Android.keyaliasName = "mainproject";
                    PlayerSettings.Android.keyaliasPass = "FtT0k7ZcF45eWXyHdZwa2SK2X87n3B";
                }
                else
                {
                    Debug.LogError($"Lack of keystore file in path:{KEYSTORE_PATH}!!!");
                    return false;
                }
            }
        }
        return true;
    }

    private static bool CheckOutputPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        if (Directory.Exists(path))
        {
            Debug.Log($"Output path: {path}");
            return true;
        }
        var folder = Directory.CreateDirectory(path);
        if (folder != null && folder.Exists)
        {
            Debug.Log($"Output path: {path}");
            return true;
        }
        return false;
    }

    private static string[] GetScenes()
    {
        var addedScenes = EditorBuildSettings.scenes;
        if (addedScenes == null || addedScenes.Length <= 0)
        {
            return null;
        }
        List<string> targetPaths = new List<string>();
        for (int i = 0; i < addedScenes.Length; i++)
        {
            var scene = addedScenes[i];
            if (scene == null)
            {
                continue;
            }
            if (scene.enabled && !string.IsNullOrEmpty(scene.path) && File.Exists(scene.path))
            {
                Debug.Log($"Add Scene: {scene.path}");
                targetPaths.Add(scene.path);
            }
        }
        return targetPaths.ToArray();
    }
}
