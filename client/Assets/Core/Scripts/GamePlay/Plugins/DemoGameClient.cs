using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DemoGameClient : GameClientBase
{
    private int connectionId;
    public void LoginRequest(string name)
    {
        var mgr = ClientMessageManager.Instance;
        mgr.SendRequest(new LoginRequest()
        {
            playerName = name,
            
        });
    }
    private void LoginNotify(LoginNotify obj)
    {
        Debug.Log($"当前人数:{obj. playerDatas.Count}");
    }

    private void LoginResponse(LoginResponse obj)
    {
        Debug.Log($"Login:{obj.isSuccess}");
        if (obj.isSuccess)
        {
            Debug.Log($"Login:{obj.playerData.playerId}");
        }

    }

    public override void Update()
    {
        // 每帧客户端逻辑
    }

    // ---------------- Client Lifecycle ----------------
    public override void OnStartClient()
    {
        Debug.Log("✅ Client started (attempting connection...)");
        var mgr = ClientMessageManager.Instance;
        Debug.Log("✅ Client connected to server");
        mgr.RegisterResponseHandler<LoginResponse>(LoginResponse);
        mgr.RegisterResponseHandler<LoginNotify>(LoginNotify);
        
    }

    public override void OnStopClient()
    {
        Debug.Log("🛑 Client stopped");
    }

    public override void OnClientConnect()
    {   
#if !UNITY_EDITOR
        LoginRequest("其他玩家");
#else
        LoginRequest("房主");
#endif
    }

    public override void OnClientDisconnect()
    {
        Debug.Log("🛑 Client disconnected from server");
    }

    public override void OnClientNotReady()
    {
        Debug.Log("⚠️ Client is not ready");
    }

    public override void OnClientChangeScene()
    {
        Debug.Log("🌍 Client changing scene");
    }

    public override void OnClientSceneChanged()
    {
        Debug.Log("🌍 Client scene changed");
    }
}