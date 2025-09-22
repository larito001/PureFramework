
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RestSystem : ServerSystemBase
{
    public override void AddEvent()
    {
        
    }

    public override void RemoveEvent()
    {
        
    }
    /// <summary>
    /// 进入休息阶段
    /// </summary>
    public void StartRestSystem(List<int> losePlayer)
    {
        GotoRestNotify notify = new GotoRestNotify();
        int loseNum = 0;
        foreach (var i in losePlayer)
        {
            var p = ServerDataPlugin.Instance.GetPlayerById(i);
            if (p != null)
            {
                p.PlayerLose();
            }
        }

        _server.commonSystem.OnFlyTextNotify("Have a Rest!", FlyTextType.Normal);
        var pList = ServerDataPlugin.Instance.GetPlayerList().ToList();
        var allCount = pList.Count;
        foreach (var playerData in pList)
        {
            if (playerData.GetState() == PlayerState.Dead)
            {
                loseNum++;
            }
        }


        if ((allCount - loseNum) <= 1)
        {
           _server.stateCtrl.GameEnd();
        }


        ServerMessageManager.Instance.SendNotify(notify);
    }
}
