using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemoGameServer : GameServerBase
{
    private List<PlayerData> players = new List<PlayerData>();
    private readonly int playerMaxNum = 3;

    public override void Update()
    {
        // 可以在这里处理服务器每帧逻辑
    }

    // ---------------- Host ----------------
    public override void OnStartHost()
    {
        Debug.Log("✅ Host started");
    }

    public override void OnStopHost()
    {
        Debug.Log("🛑 Host stopped");
    }

    // ---------------- Server ----------------
    public override void OnStartServer()
    {
        Debug.Log("✅ Server started");
    }

    public override void OnStopServer()
    {
        Debug.Log("🛑 Server stopped");
    }

    public override void OnServerConnect(int connectionId)
    {
        Debug.Log("📡 Client connected to server"+connectionId);

        var mgr = ServerMessageManager.Instance;
        mgr.RegisterRequestHandler<LoginRequest>(OnLoginRequest);
    }



    public override void OnServerDisconnect(int connectionId)
    {
        Debug.Log("📴 Client disconnected from server");
        players.Remove( players.First(x => x.playerId == connectionId));
        RefreshPlayer();
    }

    public override void OnServerReady(int connectionId)
    {
        Debug.Log("✅ Client is ready");
    }

    public override void OnServerAddPlayer(int connectionId)
    {
        var mgr = ServerMessageManager.Instance;
        //发送LinkConfig 的请求

    }
    private IResponse OnLoginRequest(LoginRequest req,int connectionId)
    {
        Debug.Log($"Player {req.playerName}");

        PlayerData playerDataTemp = null;
        LoginResponse res = new LoginResponse
        {
            isSuccess = false,
            playerData = null
        };
        if (players.Count < playerMaxNum)
        {
            playerDataTemp = new PlayerData();
            playerDataTemp.playerId = connectionId ;
            playerDataTemp.playerName = req.playerName;
            players.Add(playerDataTemp);
            // 广播给所有客户端
            RefreshPlayer();

            res.isSuccess = true;
            res.playerData = playerDataTemp;
        }
        return res;
    }

    private void RefreshPlayer()
    {
        var mgr = ServerMessageManager.Instance;

        LoginNotify notify = new LoginNotify
        {
            playerDatas = players
        };
        mgr.SendNotify(notify);
    }
}