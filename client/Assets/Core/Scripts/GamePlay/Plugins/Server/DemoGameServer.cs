using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemoGameServer : GameServerBase
{
    public enum GameState
    {
        Idle,
        Playing,
    }


    private readonly int playerMaxNum = 3;
    private GameState gameState = GameState.Idle;


    private void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<LoginRequest>(OnLoginRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<GameStartRequest>(OnGameStartRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<HeadPosRequest>(OnHeadRotationRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<CatchFoodRequest>(OnCatchFoodRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<LootingInputRequest>(OnLootingInputRequest);
    }


    private void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<LoginRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<GameStartRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<HeadPosRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<CatchFoodRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<LootingInputRequest>();
    }

    #region 生命周期

    public override void Update(float dt)
    {
        // 可以在这里处理服务器每帧逻辑
        foreach (var food in ServerDataPlugin.Instance.GetFoodList())
        {
            food.Update(dt);
        }
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
        ClearPlayers();
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
        RemovePlayer(connectionId);
    }

    public override void OnServerAddPlayer(int connectionId)
    {
        Debug.Log($"Player {connectionId} added");
    }

    #endregion

    #endregion

    #region 游戏外业务:登录，登出

    private IResponse OnLoginRequest(LoginRequest req, int connectionId)
    {
        Debug.Log($"Player {req.playerName}");

        PlayerData playerDataTemp = null;
        LoginResponse res = new LoginResponse
        {
            isSuccess = false,
            playerData = null
        };
        if (ServerDataPlugin.Instance.GetPlayerList().Count < playerMaxNum && gameState == GameState.Idle)
        {
            playerDataTemp = new PlayerData();
            playerDataTemp.playerId = connectionId;
            playerDataTemp.playerName = req.playerName;
            ServerDataPlugin.Instance.AddPlayer(playerDataTemp);
            // 广播给所有客户端
            RefreshPlayerNotify();
            res.isSuccess = true;
            res.playerData = playerDataTemp;
        }

        return res;
    }

    private void RefreshPlayerNotify()
    {
        LoginNotify notify = new LoginNotify
        {
            playerDatas = ServerDataPlugin.Instance.GetPlayerList().ToList()
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    private void RemovePlayer(int connectionId)
    {
        ServerDataPlugin.Instance.RemovePlayerById(connectionId);
        RefreshPlayerNotify();
        if (ServerDataPlugin.Instance.GetPlayerList().Count<=1)
        {
            OnGameEndNotify();
        }
   
    }

    private void ClearPlayers()
    {
       ServerDataPlugin.Instance.RemoveAllPlayers();
        RefreshPlayerNotify();
    }

    #endregion

    #region 游戏业务

    #region 游戏生命周期

    private IResponse OnGameStartRequest(GameStartRequest arg1, int arg2)
    {
        var response = new GameStartResponse();
        response.isSuccess = true;
        var notify = new GameStartNotify();
        notify.isSuccess = true;
        ServerMessageManager.Instance.SendNotify(notify);
        gameState = GameState.Playing;
        GenerateFoods();
        return response;
    }
    public void OnGameEndNotify()
    {
        //todo：玩家退回房间选择界面
    }
    #endregion

    #region 游戏gamePlay逻辑

    /// <summary>
    /// 广播生成食物
    /// </summary>
    private void GenerateFoods()
    {
        for (int i = 0; i < 10; i++)
        {
            var food = new FoodData();
            food.foodId = i;
            food.position = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            food.Init();
            ServerDataPlugin.Instance.AddFood(food);
        }

        FoodNotify notify = new FoodNotify();
        notify.foodList = ServerDataPlugin.Instance.GetFoodList().ToList();
        ServerMessageManager.Instance.SendNotify(notify);
    }

    /// <summary>
    /// 广播转发角色头的位置
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    private IResponse OnHeadRotationRequest(HeadPosRequest request, int id)
    {
        HeadPosNotify notify = new HeadPosNotify()
        {
            playerId = request.playerId,
            pos = request.pos
        };
        ServerMessageManager.Instance.SendNotify(notify);

        return null;
    }

    /// <summary>
    /// 抓取食物的请求
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    private IResponse OnCatchFoodRequest(CatchFoodRequest arg1, int arg2)
    {
        if (ServerDataPlugin.Instance.CheckHaveFood(arg1.foodId))
        {
            if (ServerDataPlugin.Instance.CheckHavePlayer(arg1.playerId))
            {
                var food = ServerDataPlugin.Instance.GetFoodById(arg1.foodId);
                food.StartCatch(ServerDataPlugin.Instance.GetPlayerById(arg1.playerId));
            }
        }

        return null;
    }

    private IResponse OnLootingInputRequest(LootingInputRequest arg1, int arg2)
    {
        var player = ServerDataPlugin.Instance.GetPlayerById(arg1.playerId);
        if (player.AddLoot())
        {
            var food = ServerDataPlugin.Instance.GetFoodById(player.useFoodId);
            var proDic = food.GetProgress();
            LootingInputNotify notify = new LootingInputNotify();
            notify.playerProgress = proDic;
            ServerMessageManager.Instance.SendNotify(notify);
        }

        return null;
    }
    
    #endregion

    #endregion
}