using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DemoGameClient : GameClientBase
{
    private int connectionId;


    private void AddEvent()
    {
        ClientMessageManager.Instance.RegisterResponseHandler<LoginResponse>(LoginResponse);
        ClientMessageManager.Instance.RegisterResponseHandler<LoginNotify>(LoginNotify);
    }
    private void RemoveEvent()
    {
        
    }
    
    #region 登录登出
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
        Debug.Log($"当前人数:{obj.playerDatas.Count}");
    }

    private void LoginResponse(LoginResponse obj)
    {
        Debug.Log($"Login:{obj.isSuccess}");
        if (obj.isSuccess)
        {
            Debug.Log($"Login:{obj.playerData.playerId}");
        }
    }


    #endregion



    #region 生命周期

    public override void Update()
    {
        // 每帧客户端逻辑
    }

    public override void OnStartClient()
    {
        Debug.Log("✅ Client started (attempting connection...)");
        AddEvent();
     
    }

    public override void OnStopClient()
    {
        Debug.Log("🛑 Client stopped");
        RemoveEvent();
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

    #endregion
   
}