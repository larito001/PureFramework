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
    private readonly int playerMaxNum = 3;
    private GameState gameState = GameState.Idle;
    

    private void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<LoginRequest>(OnLoginRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<GameStartRequest>(OnGameStartRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<InputRequest>(OnInputRequest);
    }
    
    private IResponse OnGameStartRequest(GameStartRequest arg1, int arg2)
    {
        var response = new GameStartResponse();
        response.isSuccess = true;
        var notify = new GameStartNotify();
        notify.isSuccess = true;
        ServerMessageManager.Instance.SendNotify(notify);
        gameState=GameState.Playing;
        return response;
    }

    private void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<LoginRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<GameStartRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<InputRequest>();
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

    private IResponse OnInputRequest(InputRequest request, int id)
    {
        
        InputNotify notify = new InputNotify()
        {
            playerId =request.playerId,
            input = request.input
        };
        ServerMessageManager.Instance.SendNotify(notify);
        
        return new InputResponse();
    }

    

    #endregion
}