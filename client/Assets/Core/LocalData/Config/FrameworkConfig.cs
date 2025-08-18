using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class FrameworkConfig
{
   public static string DownLoadPath = "D:\\Desktop\\local\\test";//打包后，Adressable缓存地址（外部{}引用）
   public static string RemotePath = "http://47.103.43.98/files/";//Adressable的服务器地址（外部{}引用）
   public static string BaseUrl = "http://47.103.43.98/";
   public static string UploadPath = "http://47.103.43.98/upload";//打好的Addressable包的上传的地址
   public static string DeletePath = "http://47.103.43.98/files";//删除服务器远端仓库的请求地址
   public static string LoginPath = "http://47.103.43.98/login";//登录服务器远端仓库的请求地址
   public static string LogoutPath = "http://47.103.43.98/logout";//登出服务器远端仓库的请求地址
   public static string PackPath=@"F:\PureFrame\PureFramework\client\ServerData\StandaloneWindows64";//打好的本地Addressable包的地址，用于上传
   //   public static string RemoteBuildPath = "ServerData/[BuildTarget]";Build地址需要在Addressable里改
   public static string DLLName = "HotUpdate.dll.bytes";//热更dll在group中的索引
   public static string StartSceneName="Assets/HotUpdate/Scenes/StartScene.unity";//更新后启动场景的group中的索引
   public static string DLLPath = @"../HybridCLRData/HotUpdateDlls/StandaloneWindows64/HotUpdate.dll";//热更dll打包后迁移前的位置
   public static string NewDLLPath = "HotUpdate/Dlls";//热更dll打包后迁移后的位置
}
