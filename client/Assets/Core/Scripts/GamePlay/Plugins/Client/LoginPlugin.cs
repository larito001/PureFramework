using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class LoginPlugin : LogicPluginBase
{
    public static LoginPlugin Instance;
    public LoginPlugin()
    {
        Instance = this;
    }

    private List<PlayerData> _playerDatas= new List<PlayerData>();
    protected override void OnInstall()
    {
        base.OnInstall();
        ClientMessageManager.Instance.RegisterResponseHandler<LoginResponse>(LoginResponse);
        ClientMessageManager.Instance.RegisterResponseHandler<LoginNotify>(LoginNotify);
    }

    protected override void OnUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<LoginResponse>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<LoginNotify>();
        base.OnUninstall();
    }
    
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
        _playerDatas = obj.playerDatas;
        YOTOFramework.eventMgr.TriggerEvent(YOTOEventType.RefreshRoleList);
    }
    
    private void LoginResponse(LoginResponse obj)
    {
        Debug.Log($"Login:{obj.isSuccess}");
        if (obj.isSuccess)
        {
            Debug.Log($"Login:{obj.playerData.playerId}");
        }
    }


    public List<PlayerData> GetPlayerDatas()
    {
        return _playerDatas;
    }
}