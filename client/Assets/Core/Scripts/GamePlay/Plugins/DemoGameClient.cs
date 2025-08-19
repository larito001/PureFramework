using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGameClient : GameClientBase
{
    public override void OnInit()
    {
    RequestMove(123,new Vector3(111,222,333));
    }

    public override void Update()
    {
    }

    public override void OnShutdown()
    {
    }
    public void RequestMove(uint playerId, Vector3 targetPos)
    {
        var mgr = ClientMessageManager.Instance;
        mgr.SendRequest(new MoveRequest
        {
            playerId = playerId,
            targetPos = targetPos
        });
    }
}