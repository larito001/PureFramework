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

    public string Name = "TestName";
    private int _playerId = -1;

    public int PlayerId
    {
        get { return _playerId; }
        
        private set { _playerId = value; }
    }
    private List<PlayerData> _playerDatas= new List<PlayerData>();
    protected override void OnInstall()
    {
        base.OnInstall();
    
    }

    protected override void OnUninstall()
    {
   
        base.OnUninstall();
    }
    public void OnNetInstall()
    {
        ClientMessageManager.Instance.RegisterResponseHandler<LoginResponse>(LoginResponse);
        ClientMessageManager.Instance.RegisterResponseHandler<LoginNotify>(LoginNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<GameStartNotify>(OnGameStartNotify);
        
        ClientMessageManager.Instance.RegisterResponseHandler<GameStartResponse>(OnGameStartResponse);
    }
    
    public void OnNetUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<LoginResponse>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<LoginNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<GameStartNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<GameStartResponse>();
    }

    public void GameStartRequest()
    {
        var mgr = ClientMessageManager.Instance;
        Debug.Log("GameStartRequest");
        mgr.SendRequest(new GameStartRequest()
        {
            isSuccess = true,
        });
    }
    private void OnGameStartResponse(GameStartResponse obj)
    {
        
    }

    private void OnGameStartNotify(GameStartNotify obj)
    {
        Debug.Log("游戏开始！");
        
        //todo:生成游戏角色
        StagePlugin.Instance.OnGameStart(_playerId,_playerDatas);
    }
    public void LoginRequest()
    {
        var mgr = ClientMessageManager.Instance;
        Debug.Log("LoginRequest");
        mgr.SendRequest(new LoginRequest()
        {
            playerName = Name,
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
            _playerId= obj.playerData.playerId;
            YOTOFramework.uIMgr.Show(UIEnum.RoomPanel);
        }
    }


    public List<PlayerData> GetPlayerDatas()
    {
        return _playerDatas;
    }
}