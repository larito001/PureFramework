using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class PlayerPlugin : LogicPluginBase
{
    public static PlayerPlugin Instance;
    private Dictionary<int,PlayerEntity> players = new Dictionary<int,PlayerEntity>();
    public PlayerPlugin()
    {
        Instance = this;
    }
    protected override void OnInstall()
    {
        base.OnInstall();
    }
    
    protected override void OnUninstall()
    {
        base.OnUninstall();
    }
    public void OnNetInstall()
    {
        ClientMessageManager.Instance.RegisterResponseHandler<HeadPosNotify>(OnHeadPosNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<CatchFoodNotify>(OnCatchFoodNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<EndCatchFoodNotify>(OnEndCatchFoodNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<StartLootNotify>(OnStartLootNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<StopLootNotify>(OnStopLootNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<LootingInputNotify>(OnLootingInputNotify);

         YOTOFramework.eventMgr.AddEventListener(YOTO.YOTOEventType.Space,OnSpaceClick);
    
    }
    
    public void OnNetUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<HeadPosNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<CatchFoodNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<EndCatchFoodNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<StartLootNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<StopLootNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<LootingInputNotify>();

        YOTOFramework.eventMgr.RemoveEventListener(YOTO.YOTOEventType.Space,OnSpaceClick); 
    }

    #region 食物操作
    
    
    private void OnStartLootNotify(StartLootNotify obj)
    {
        //todo:开抢，对应id进入特殊状态
        foreach (var objPlayerId in obj.playerIds)
        {
            players[objPlayerId].StartLooting(obj.foodId);
        }
        Debug.Log("开始抢夺");
        
    }
    private void OnStopLootNotify(StopLootNotify obj)
    {
        //todo:抢夺结束，退出特殊状态
        Debug.Log("抢夺结束，player"+obj.winPlayerId+"赢了");

        if (players.ContainsKey(obj.winPlayerId))
        {
            players[obj.winPlayerId].EndLooting(true,obj.foodId);
        
        }
        for (var i = 0; i < obj.losePlayers.Count; i++)
        {
            if(players.ContainsKey(obj.losePlayers[i]))
                players[obj.losePlayers[i]].EndLooting(false,obj.foodId);
        } 
    }


    public void CatchFood(int fId)
    {
        var mgr = ClientMessageManager.Instance;
        Debug.Log("CatchFood");
        mgr.SendRequest(new CatchFoodRequest()
        {
            playerId =LoginPlugin.Instance.PlayerId,
            foodId=fId
        });
    }
    private void OnCatchFoodNotify(CatchFoodNotify obj)
    {
        players[obj.playerId].CatchFood(obj.foodId,obj.isSuccess);
    }
    private void OnEndCatchFoodNotify(EndCatchFoodNotify obj)
    {
        if (obj.isSuccess)
        {
            if (players.ContainsKey(obj.playerId))
            {
                players[obj.playerId].EndCatch();
            }
            else
            {
                //处理catch过程中，玩家退出
                StagePlugin.Instance.RemoveFood(obj.foodId);
            }
            
      
        }
    }

    private void OnSpaceClick()
    {
        LootAdd();
    }
    private void LootAdd()
    {
        LootingInputRequest req = new LootingInputRequest();
        req.playerId = LoginPlugin.Instance.PlayerId;
        ClientMessageManager.Instance.SendRequest(req);
    }
    private void OnLootingInputNotify(LootingInputNotify obj)
    {
        
    }
    #endregion



    #region 旋转头
    /// <summary>
    /// 输入，摄像机调用
    /// </summary>
    /// <param name="input"></param>
    public void RotateSelfPlayerEyes(Vector2 input)
    {
        if (players.ContainsKey(LoginPlugin.Instance.PlayerId))
        {
            players[LoginPlugin.Instance.PlayerId].SetEyesMove(input); 
        }
    
    }
    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="pos"></param>
    public void RotatePlayerRequest(Vector3  pos)
    {
        var pid = LoginPlugin.Instance.PlayerId;
        var mgr = ClientMessageManager.Instance;
        Debug.Log("RotateRequest");
        mgr.SendRequest(new HeadPosRequest()
        {
            playerId =pid,
            pos = pos
        });
    }
    /// <summary>
    /// 头部旋转实际，自己的客户端负责控制（或者压根不控制）
    /// </summary>
    /// <param name="obj"></param>
    private void OnHeadPosNotify(HeadPosNotify obj)
    {
        // players.RotatePlayer();
        if (LoginPlugin.Instance.PlayerId!=obj.playerId)
        {
            //todo:旋转眼球
            if (players.ContainsKey(obj.playerId))
            {
                // Debug.Log("Player:"+obj.playerId+" 旋转眼球"+obj.pos);
                players[obj.playerId].SetEyesMove(obj.pos);
            }
      
        }
    }
    #endregion


    #region 生成、移除player

    public void GeneratePlayers(List<PlayerData> playerDatas)
    {
      
        for (var i = 0; i < playerDatas.Count; i++)
        {
            // playerDatas[i]
            var pp = GameObject.Find("p"+(i+1).ToString());
            if (pp != null)
            {
                var p= PlayerEntity.pool.GetItem(playerDatas[i]);
                p.Location = pp.transform.position;
                p.InstanceGObj();
                players.Add(playerDatas[i].playerId,p);
            }
          
        }

    }
    List<int> removeList = new List<int>();
    public void RefreshPlayers(List<PlayerData> datas)
    {
        removeList.Clear();
        foreach (var id in players.Keys)
        {
            bool isHave=false;
            foreach (var player in datas)
            {
                if (player.playerId == id)
                {
                    isHave = true;
                }
            }

            if (!isHave)
            {
                removeList.Add(id);
            }
            
        }
        foreach (var i in removeList)
        {
            PlayerEntity.pool.RecoverItem(players[i]);
            players.Remove(i);
        }
    }

    public void RemoveAllPlayers()
    {
        foreach (var player in players.Values)
        {
            PlayerEntity.pool.RecoverItem(player);
        }
        players.Clear();
    }

    #endregion

    
}
