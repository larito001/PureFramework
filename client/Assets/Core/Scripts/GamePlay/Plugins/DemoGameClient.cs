using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGameClient : GameClientBase
{
    public void RequestMove(uint playerId, Vector3 targetPos)
    {
        var mgr = ClientMessageManager.Instance;
        mgr.RegisterResponseHandler<MoveResponse>(OnMoveResponse);
        mgr.SendRequest(new MoveRequest
        {
            playerId = playerId,
            targetPos = targetPos
        });
    }

    private void OnMoveResponse(MoveResponse obj)
    {
        Debug.Log($"moveResponse {obj.playerId}: {obj.confirmedPos}");
    }

    public override void Update()
    {
        // 每帧客户端逻辑
    }

    // ---------------- Client Lifecycle ----------------
    public override void OnStartClient()
    {
        Debug.Log("✅ Client started (attempting connection...)");
    }

    public override void OnStopClient()
    {
        Debug.Log("🛑 Client stopped");
    }

    public override void OnClientConnect()
    {
        Debug.Log("✅ Client connected to server");

#if !UNITY_EDITOR
        RequestMove(456, new Vector3(111, 222, 333));
#else
        RequestMove(123, new Vector3(111, 222, 333));
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
}