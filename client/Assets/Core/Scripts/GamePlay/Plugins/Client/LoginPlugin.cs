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

    private List<PlayerData> _playerDatas = new List<PlayerData>();

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
        ClientMessageManager.Instance.RegisterResponseHandler<RefreshPlayerDatas>(LoginNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<GameStartNotify>(OnGameStartNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<GameEndNotify>(OnGameEndNotify);
    }


    public void OnNetUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<LoginResponse>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<RefreshPlayerDatas>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<GameStartNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<GameEndNotify>();
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

    private void OnGameEndNotify(GameEndNotify obj)
    {   
        YOTOFramework.uIMgr.Show(UIEnum.FinishPanel);
        YOTOFramework.timeMgr.DelayCall(() =>
        {
            YOTOFramework.uIMgr.Show(UIEnum.LoadingPanel);
        }, 3f);
 
        YOTOFramework.timeMgr.DelayCall(StagePlugin.Instance.OnGameEnd, 5);
    }

    private void OnGameStartNotify(GameStartNotify obj)
    {
        Debug.Log("游戏开始！");
        //todo:打开loading
        YOTOFramework.uIMgr.Show(UIEnum.LoadingPanel);
        YOTOFramework.timeMgr.DelayCall( StagePlugin.Instance.OnGameStart,1.2f);
    }

    public void OnNetError()
    {
        FlyTextMgr.Instance.AddTextAtScreenCenter("链接失败", FlyTextType.Normal);
        YOTOFramework.netMgr.LeaveHost();
        _playerId = -1;
        StagePlugin.Instance.OnGameError();
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

    private void LoginNotify(RefreshPlayerDatas obj)
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
            _playerId = obj.playerData.playerId;
            YOTOFramework.uIMgr.Show(UIEnum.RoomPanel);
        }
        else
        {
            FlyTextMgr.Instance.AddTextAtScreenCenter("加入失败");
            //todo：强制断链
            YOTOFramework.netMgr.LeaveHost();
        }
    }


    public List<PlayerData> GetPlayerDatas()
    {
        return _playerDatas;
    }
}