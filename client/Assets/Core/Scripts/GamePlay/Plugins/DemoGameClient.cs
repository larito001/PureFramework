using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGameClient : GameClientBase
{
    public override void OnInit()
    {
        #if !UNITY_EDITOR
        RequestMove(456,new Vector3(111,222,333));
        #endif
#if UNITY_EDITOR
        RequestMove(123,new Vector3(111,222,333));    
#endif
  
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
        mgr.RegisterResponseHandler<MoveResponse>(OnMoveResponse);
        mgr.SendRequest(new MoveRequest
        {
            playerId = playerId,
            targetPos = targetPos
        });
    }

    private void OnMoveResponse(MoveResponse obj)
    {
        Debug.Log("moveResponse"+obj.playerId+":"+obj.confirmedPos);
    }
}