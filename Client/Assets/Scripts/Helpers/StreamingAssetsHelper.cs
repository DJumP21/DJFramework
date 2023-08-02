using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

#if  UNITY_EDITOR
/// <summary>
/// StreamingAssets目录下资源查询帮助类
/// </summary>
public sealed class StreamingAssetsHelper
{
    public static void Init()
    {
    }

    public static bool FileExist(string packageName, string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, StreamingAssetDefine.RootFoldName, packageName,
            fileName);
        return File.Exists(path);
    }
}
#else
/// <summary>
/// StreamingAssets目录下资源查询帮助类
/// </summary>
public sealed class StreamingAssetsHelper
{
    public static bool isInit=false;
    //缓存数据
    public static readonly HashSet<string> cacheData = new HashSet<string>();
    
    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            var manifest = Resources.Load<BuildinFileManifest>("BuildinFileManifest");
            if (manifest != null)
            {
                foreach (var fileName in manifest.BuildinFiles)
                {
                    cacheData.Add(fileName);
                }
            }
        }
    }

    public static bool FileExist(string packageName, string fileName)
    {
        if (!isInit)
        {
            Init();
        }
        return cacheData.Contains(fileName);
    }
}
#endif

/// <summary>
/// 内置文件查询服务
/// </summary>
public  class GameQueryServices : IQueryServices
{
    /// <summary>
    /// 查询包内是否存在某个文件
    /// </summary>
    /// <param name="packageName">包名</param>
    /// <param name="fileName">文件名</param>
    /// <returns></returns>
    public bool QueryStreamingAssets(string packageName, string fileName)
    {
        return StreamingAssetsHelper.FileExist(packageName, fileName);
    }
}

#if UNITY_EDITOR
/// <summary>
/// 提前构建
/// </summary>
internal class PreprogressBuild : UnityEditor.Build.IPreprocessBuildWithReport
{
    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }
    /// <summary>
    /// 构建应用程序前处理
    /// </summary>
    /// <param name="report"></param>
    public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        string saveFilePath = "Assets/Resources/BuildinFileManifest.asset";
        if(File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
        string folderPath = $"{Application.dataPath}/StreamingAssets/{StreamingAssetDefine.RootFoldName}";
        DirectoryInfo root = new DirectoryInfo(folderPath);
        if(!root.Exists)
        {
            Debug.Log($"没有发现YooAsset内置目录：{folderPath}");
            return;
        }
        var manifest = ScriptableObject.CreateInstance<BuildinFileManifest>();
        FileInfo[] files= root.GetFiles("*",SearchOption.AllDirectories);
        foreach( FileInfo fileInfo in files)
        {
            if(fileInfo.Extension==".meta")
            {
                continue;
            }
            if(fileInfo.Name.StartsWith("PackageManifest_"))
            {
                continue;
            }
            manifest.BuildinFiles.Add(fileInfo.Name);
        }
        //不存在Resources目录，则创建
        if(!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
        }
        UnityEditor.AssetDatabase.CreateAsset(manifest, saveFilePath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log($"一共{manifest.BuildinFiles.Count}个内置文件，内置资源清单保存成功 : {saveFilePath}");
    }
}
#endif

