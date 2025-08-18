using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class CheckAssetsUpdate : MonoBehaviour
{
    private AsyncOperationHandle<long> downloadHandle;
    AsyncOperationHandle remote;
    private StaticLoadingPage loadPage;

    void Start()
    {

        LoadDefDLL();
        StartCoroutine(CheckUpdate());
        loadPage=GetComponent<StaticLoadingPage>();
    }
    private void LoadDefDLL()
    {
        //����dll
        Debug.Log("Starting to check and download assets with label: all");
        List<string> aotDllList = new List<string>
        {
            "System.Core.dll",
            "System.dll",
            "Unity.Addressables.dll",
            "Unity.ResourceManager.dll",
            "UnityEngine.CoreModule.dll",
            "mscorlib.dll",
        };
        foreach (var dllName in aotDllList)
        {
            byte[] dllBytes = File.ReadAllBytes($"{Application.streamingAssetsPath}/{dllName}");
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
            if (err != LoadImageErrorCode.OK)
            {
                Debug.LogError($"Failed to load AOT DLL: {dllName}, Error: {err}");
                // If any AOT DLL fails to load, stop the process
            }
            else
            {
                Debug.Log($"{dllName} 加载成功");
            }
        }
    }
    private IEnumerator CheckUpdate()
    {

        downloadHandle = Addressables.GetDownloadSizeAsync("all");
        //Debug.Log("加载"+ downloadHandle);
        yield return downloadHandle;
        Debug.Log("检查下载资源");
        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            if (downloadHandle.Result <= 0)
            {
                Debug.Log("没有更新");
                EnterGame();
            }
            else
            {
                Debug.Log("更新游戏");
                StartCoroutine(Download());
           
            }
        }
        yield return null;
    }

    IEnumerator Download()
    {
        remote = Addressables.DownloadDependenciesAsync("all", true);
        while (!remote.IsDone)
        {
            var bytes = remote.GetDownloadStatus().DownloadedBytes;
            var totalBytes = remote.GetDownloadStatus().TotalBytes;
            var status = remote.GetDownloadStatus();
            float progress = status.Percent;
            Debug.Log($"Download progress : {progress}");
            loadPage.Loading(progress);
            yield return null;
        }

        EnterGame();
    }
    void EnterGame()
    {
 
        Debug.Log("加载了：HotUpdate.dll"+ remote);
        var loadDllAsync = Addressables.LoadAssetAsync<TextAsset>(FrameworkConfig.DLLName);
        loadDllAsync.Completed += OnHotUpdateDllLoaded;
    }

    void OnHotUpdateDllLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
           
            Debug.Log("DLL 加载完毕");

            Assembly hotUpdate = null;
            try
            {
                hotUpdate = Assembly.Load(handle.Result.bytes);
                Debug.Log("加载游戏");

                //GameRoot.Instance.Init();
                loadPage?.Loading(1);
                AsyncOperationHandle<SceneInstance> lastHandle= Addressables.LoadSceneAsync(FrameworkConfig.StartSceneName, LoadSceneMode.Single);
                lastHandle.Completed += (o) =>
                {
                };
            }
            catch (Exception ex)
            {
                Debug.LogError("DLL加载错误: " + ex.Message);
                return;
            }
      
            
   
        }

   
    }
}
