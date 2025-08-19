using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGameServer : GameServerBase
{
    public override void OnInit()
    {    var mgr = ServerMessageManager.Instance;
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
            
            return  res;
        });

    }

    public override void Update()
    {
    }

    public override void OnShutdown()
    {
    }
 
}