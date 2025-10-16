
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RestSystem : ServerSystemBase
{
    
    private HashSet<int> readyPlayer = new HashSet<int>();
    public override void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<RestEnd>(PlayerRestEnd);
    }
    
    private IResponse PlayerRestEnd(RestEnd req, int pid)
    {
        if (!readyPlayer.Contains(req.playerId))
        {
            readyPlayer.Add(req.playerId);
        }
        //检查是否都ready了
        var list = ServerDataPlugin.Instance.GetPlayerList().ToList();
        bool haveNotEnd = false;
        for (var i = 0; i < list.Count; i++)
        {
            if (!readyPlayer.Contains(list[i].playerId))
            {
                haveNotEnd= true;
            }
          
        }

        if (!haveNotEnd)
        {
            //进入下一阶段
            _server.stateCtrl.ForcePopCurrentState(GameState.Rest);
        }
        
        return null;
    }

    public override void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<RestEnd>();
    }
    
    
    /// <summary>
    /// 进入休息阶段
    /// </summary>
    public void StartRestSystem(List<int> losePlayer)
    {
        readyPlayer.Clear();
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

        var list = ServerDataPlugin.Instance.GetPlayerList().ToList();

        for (var i = 0; i < list.Count; i++)
        {
            if (!losePlayer.Contains(list[i].playerId))
            {
                list[i].PlayerWin();
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
