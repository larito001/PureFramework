using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using YOTO;

public class DemoGameClient : GameClientBase
{
    private int connectionId;
    
    #region 生命周期

    public override void Update()
    {
        // 每帧客户端逻辑
    }

    public override void OnStartClient()
    {
        Debug.Log("✅ Client started (attempting connection...)");
    }

    public override void OnStopClient()
    {
        Debug.Log("🛑 Client stopped");
        LoginPlugin.Instance.OnNetUninstall();
        PlayerPlugin.Instance.OnNetUninstall();
    }

    public override void OnClientConnect()
    {
        Debug.Log("✅ Client connected");
        LoginPlugin.Instance.OnNetInstall();
        PlayerPlugin.Instance.OnNetInstall();
        LoginPlugin.Instance.LoginRequest();
    }

    public override void OnClientDisconnect()
    {
        Debug.Log("🛑 Client disconnected from server");
        YOTOFramework.netMgr.LeaveHost();
    }

    public override void OnClientNotReady()
    {
        Debug.Log("⚠️ Client is not ready");
        YOTOFramework.netMgr.LeaveHost();
    }

    public override void OnClientChangeScene()
    {
        Debug.Log("🌍 Client changing scene");
    }

    public override void OnClientSceneChanged()
    {
        Debug.Log("🌍 Client scene changed");
    }

    #endregion
   
}