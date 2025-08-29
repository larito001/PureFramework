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
    private List<PlayerData> players = new List<PlayerData>();
    private Dictionary<int,FoodData> foods = new Dictionary<int,FoodData>(); 
    private readonly int playerMaxNum = 3;
    private GameState gameState = GameState.Idle;
    

    private void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<LoginRequest>(OnLoginRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<GameStartRequest>(OnGameStartRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<HeadPosRequest>(OnHeadRotationRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<CatchFoodRequest>(OnCatchFoodRequest);
        
    }

 


    private void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<LoginRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<GameStartRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<HeadPosRequest>();
    }

    #region 生命周期

    public override void Update()
    {
        // 可以在这里处理服务器每帧逻辑
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
        if (players.Count < playerMaxNum&&gameState==GameState.Idle)
        {
            playerDataTemp = new PlayerData();
            playerDataTemp.playerId = connectionId;
            playerDataTemp.playerName = req.playerName;
            players.Add(playerDataTemp);
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
            playerDatas = players
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    private void RemovePlayer(int connectionId)
    {
        players.Remove(players.First(x => x.playerId == connectionId));
        RefreshPlayerNotify();
    }

    private void ClearPlayers()
    {
        players.Clear();
        RefreshPlayerNotify();
    }

    #endregion

    #region 游戏业务
    private IResponse OnGameStartRequest(GameStartRequest arg1, int arg2)
    {
        var response = new GameStartResponse();
        response.isSuccess = true;
        var notify = new GameStartNotify();
        notify.isSuccess = true;
        ServerMessageManager.Instance.SendNotify(notify);
        gameState=GameState.Playing;

        GenerateFoods();
        return response;
    }

    private void GenerateFoods()
    {
        for (int i = 0; i < 10; i++)
        {
            var food = new FoodData();
            food.foodId = i;
            food.position = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            food.state = FoodState.Idle;
            foods.Add( food.foodId,food);
        }
        FoodNotify notify = new FoodNotify();
        notify.foodList = foods.Values.ToList();
        ServerMessageManager.Instance.SendNotify(notify);
        
    }
    
    private IResponse OnHeadRotationRequest(HeadPosRequest request, int id)
    {
        
        HeadPosNotify notify = new HeadPosNotify()
        {
            playerId =request.playerId,
            pos = request.pos
        };
        ServerMessageManager.Instance.SendNotify(notify);
        
        return new HeadPosResponse();
    }

    private IResponse OnCatchFoodRequest(CatchFoodRequest arg1, int arg2)
    {
        bool isSuccess = false;
        if (foods.ContainsKey(arg1.foodId))
        {
            if (foods[arg1.foodId].state == FoodState.Idle)
            {
                foods[arg1.foodId].state=FoodState.Catching;
                isSuccess = true;
        
            }else if (foods[arg1.foodId].state == FoodState.Catching)
            {
                //todo:开启抢夺
                foods[arg1.foodId].state=FoodState.Discard;
            }
        }
        CatchFoodNotify notify = new CatchFoodNotify()
        {
            playerId =arg1.playerId,
            foodId = arg1.foodId,
            isSuccess = isSuccess
        };
        ServerMessageManager.Instance.SendNotify(notify);
        NotifyFoodState();
        return null;
    }

    private void NotifyFoodState()
    {
        var notify = new RefreshFoodStateNotify()
        {
            newDatas = foods.Values.ToList()
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    #endregion
}