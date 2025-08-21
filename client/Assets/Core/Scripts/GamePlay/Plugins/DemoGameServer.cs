using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGameServer : GameServerBase
{
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

    public override void OnServerConnect()
    {
        Debug.Log("📡 Client connected to server");

        var mgr = ServerMessageManager.Instance;
        mgr.RegisterRequestHandler<MoveRequest>((req) =>
        {
            Debug.Log($"Player {req.playerId} wants to move to {req.targetPos}");

            // 服务器校验和逻辑
            Vector3 finalPos = req.targetPos; // 可以做阻挡检测/修正

            // 广播给所有客户端
            var res = new MoveResponse
            {
                playerId = req.playerId,
                confirmedPos = finalPos
            };
            mgr.SendNotify(res);

            return res;
        });
    }

    public override void OnServerDisconnect()
    {
        Debug.Log("📴 Client disconnected from server");
    }

    public override void OnServerReady()
    {
        Debug.Log("✅ Client is ready");
    }

    public override void OnServerAddPlayer()
    {
        Debug.Log("👤 Player added for client");
    }
}