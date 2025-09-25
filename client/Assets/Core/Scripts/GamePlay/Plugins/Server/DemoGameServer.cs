using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YOTO;

public class DemoGameServer : GameServerBase
{
    #region 配置属性

    public GameServerStateCtrl stateCtrl = new GameServerStateCtrl();
    private RuleSystem ruleSystem = new RuleSystem();
    public CommonSystem commonSystem = new CommonSystem();
    private FoodSystem foodSystem = new FoodSystem();
    private VotSystem votSystem = new VotSystem();
    private PlayerSystem playerSystem = new PlayerSystem();
    private RestSystem restSystem = new RestSystem();
    private List<ServerSystemBase> systemList = new List<ServerSystemBase>();
    
    
    public DemoGameServer()
    {
        AddSystem(playerSystem);
        AddSystem(ruleSystem);
        AddSystem(commonSystem);
        AddSystem(foodSystem);
        AddSystem(votSystem);
        AddSystem(restSystem);
    }

    private void OnStateStart(StateInfo state)
    {
        Debug.LogWarning("Start State:" + state.State);
        switch (state.State)
        {
            case GameState.Rest:
      
                var loser = ruleSystem.OnFinishUseRule();
                restSystem.StartRestSystem(loser);
                break;
            case GameState.Selecting:
                ruleSystem.OnSelectHostPlayer();
                break;
            case GameState.Ready:
                commonSystem.OnFlyTextNotify("Ready", FlyTextType.Normal);
                break;
            case GameState.Playing:
                foodSystem.StartFoodSystem();
                break;
            case GameState.End:
         
                break;
        }
    }

    private void OnStateUpdate(StateInfo state, int second)
    {
        switch (state.State)
        {
            case GameState.Rest:
      
                break;
            case GameState.Selecting:
                break;
            case GameState.Ready:
                break;
            case GameState.Playing:
                commonSystem.GameTimerNotify(second);
                break;
            case GameState.End:
                break;
        }
    }

    private void OnStateEnd(StateInfo state)
    {
        Debug.LogWarning("End State:" + state.State);
        switch (state.State)
        {
            case GameState.Selecting:
                ruleSystem.SetRandomRule();
                break;
            case GameState.Playing:
                foodSystem.EndGenerateFood();
                stateCtrl.ReStartLevel();
                ServerDataPlugin.Instance.RemoveAllFoods();
                break;
            case GameState.Voting:
                votSystem.VotingEnd();
                break;
            case GameState.End:
                OnGameEndNotify();
                stateCtrl.OnJoinRoom();
                votSystem.Reset();
  
                break;
        }
    }

    #endregion

    #region Systems生命周期

    private void AddSystem(ServerSystemBase system)
    {
        systemList.Add(system);
        system.Init(this);
    }


    private void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<GameStartRequest>(OnGameReadyRequest);
        for (var i = 0; i < systemList.Count; i++)
        {
            systemList[i].AddEvent();
        }

        stateCtrl.OnStateEnd = OnStateEnd;
        stateCtrl.OnStateStart = OnStateStart;
        stateCtrl.OnStateUpdate =OnStateUpdate;
    }


    private void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<GameStartRequest>();
        for (var i = 0; i < systemList.Count; i++)
        {
            systemList[i].RemoveEvent();
        }

        stateCtrl.OnStateEnd = null;
        stateCtrl.OnStateStart = null;
        stateCtrl.OnStateUpdate = null;
    }

    #endregion

    #region 生命周期

    public override void Update(float dt)
    {
        // 可以在这里处理服务器每帧逻辑
        foreach (var food in ServerDataPlugin.Instance.GetFoodList())
        {
            food.Update(dt);
        }

        stateCtrl.Update(dt);
    }

    #region host

    public override void OnStartHost()
    {
        Debug.Log("✅ Host started");
    }

    public override void OnStopHost()
    {
        Debug.Log("🛑 Host stopped");
    }

    #endregion

    #region server

    public override void OnStartServer()
    {
        AddEvent();
        Debug.Log("✅ Server started");
    }

    public override void OnStopServer()
    {
        playerSystem.ClearPlayers();
        RemoveEvent();
        Debug.Log("🛑 Server stopped");
    }

    #endregion

    #region 客户端连接

    public override void OnServerConnect(int connectionId)
    {
        Debug.Log($"📡 Client {connectionId} connected to server");
    }

    public override void OnServerReady(int connectionId)
    {
        Debug.Log($"✅ Client {connectionId} is ready");
    }


    public override void OnServerDisconnect(int connectionId)
    {
        Debug.Log($"❌ Client {connectionId} disconnected from server");
        playerSystem.RemovePlayer(connectionId);
    }

    public override void OnServerAddPlayer(int connectionId)
    {
        Debug.Log($"Player {connectionId} added");
    }

    #endregion

    #endregion

    #region 游戏生命周期

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    private IResponse OnGameReadyRequest(GameStartRequest arg1, int arg2)
    {
        if (stateCtrl.GameIsStart()) return null;
        playerSystem.RefreshAllPlayerProperty();

        var notify = new GameStartNotify();
        notify.isSuccess = true;
        ServerMessageManager.Instance.SendNotify(notify);
        YOTOFramework.timeMgr.DelayCall(stateCtrl.OnGameStart, 2);
        return null;
    }

    /// <summary>
    /// 游戏完全结束阶段
    /// </summary>
    public void OnGameEndNotify()
    {
        GameEndNotify notify = new GameEndNotify();
        //判定最终赢家
        var pList = ServerDataPlugin.Instance.GetPlayerList().ToList();
        notify.rule = ServerDataPlugin.Instance.CurrentRule;
        notify.winPlayersDatas = new List<PlayerData>();
        notify.losePlayersDatas = new List<PlayerData>();
        if (pList.Count == 1)
        {
            notify.winPlayersDatas = pList;
        }
        else
        {
            foreach (var playerData in pList)
            {
                if (playerData.GetState() != PlayerState.Dead)
                {
                    notify.winPlayersDatas.Add(playerData);
                }
                else
                {
                    notify.losePlayersDatas.Add(playerData);
                }
            }
        }


        ServerMessageManager.Instance.SendNotify(notify);
    }

    #endregion
}