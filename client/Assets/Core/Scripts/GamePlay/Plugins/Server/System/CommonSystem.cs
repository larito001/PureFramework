

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonSystem : ServerSystemBase
{
    public override void AddEvent()
    {
        
    }

    public override void RemoveEvent()
    {
        
    }
    public void OnFlyTextNotify(string txt, FlyTextType flyType, List<int> elsePlayers = null)
    {
        FlyTextNotify notify = new FlyTextNotify();
        notify.txt = txt;
        notify.elsePlayers = elsePlayers;
        notify.flyType = flyType;
        ServerMessageManager.Instance.SendNotify(notify);
    }

    public void GameTimerNotify(int i)
    {
        GameTimerNotify notify = new GameTimerNotify();
        notify.index = i;
        ServerMessageManager.Instance.SendNotify(notify);
    }
}
