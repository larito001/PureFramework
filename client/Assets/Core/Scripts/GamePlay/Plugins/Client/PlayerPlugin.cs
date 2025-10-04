using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class PlayerPlugin : LogicPluginBase
{
    #region 单例、生命周期

    public static PlayerPlugin Instance;
    public Dictionary<int, PlayerEntity> players = new Dictionary<int, PlayerEntity>();


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
        ClientMessageManager.Instance.RegisterResponseHandler<FlyTextNotify>(OnFlyTextNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<PlayerPropertyNotify>(OnPlayerPropertyNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<SomeOneFindHostPlayerNotifyt>(OnSomeOneFindHostPlayerNotifyt);
        ClientMessageManager.Instance.RegisterResponseHandler<VotEndNotify>(OnVotEndNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<PlayerDeadNotify>(OnPlayerDeadNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<PlayerNeedDrinkNotify>(OnPlayerNeedDrinkNotify);
        YOTOFramework.eventMgr.AddEventListener(YOTO.YOTOEventType.Space, OnSpaceClick);
    }

    private void OnPlayerNeedDrinkNotify(PlayerNeedDrinkNotify obj)
    {
        Debug.LogError("玩家"+obj.playerId+"喝酒了");
        if (players.ContainsKey(obj.playerId))
        {
            players[obj.playerId].Drink(obj.currentRate);
        }
    }

    private void OnPlayerDeadNotify(PlayerDeadNotify obj)
    {
        Debug.LogError("玩家"+obj.playerId+"似了");
        if (players.ContainsKey(obj.playerId))
        {
            players[obj.playerId].Dead();
        }
    }


    public void OnNetUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<HeadPosNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<CatchFoodNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<EndCatchFoodNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<StartLootNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<StopLootNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<LootingInputNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<FlyTextNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<PlayerPropertyNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<SomeOneFindHostPlayerNotifyt>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<VotEndNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<PlayerDeadNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<PlayerNeedDrinkNotify>();
        YOTOFramework.eventMgr.RemoveEventListener(YOTO.YOTOEventType.Space, OnSpaceClick);
    }

    #endregion


    public PlayerEntity GetSelf()
    {
        return players[LoginPlugin.Instance.PlayerId];
    }
    
    #region 食物操作

    private void OnStartLootNotify(StartLootNotify obj)
    {
     
        //todo:开抢，对应id进入特殊状态
        foreach (var objPlayerId in obj.playerIds)
        {
            if (objPlayerId == LoginPlugin.Instance.PlayerId)
            {
                YOTOFramework.uIMgr.Show(UIEnum.LootingPanel);
            }
            
            players[objPlayerId].StartLooting(obj.foodId);
        }

        Debug.Log("开始抢夺");
    }

    private void OnStopLootNotify(StopLootNotify obj)
    {
        YOTOFramework.uIMgr.Hide(UIEnum.LootingPanel);
        //todo:抢夺结束，退出特殊状态
        Debug.Log("抢夺结束，player" + obj.winPlayerId + "赢了");
        
        
        if (players.ContainsKey(obj.winPlayerId))
        {
            players[obj.winPlayerId].EndLooting(true, obj.foodId);
        }

        for (var i = 0; i < obj.losePlayers.Count; i++)
        {
            if (players.ContainsKey(obj.losePlayers[i]))
                players[obj.losePlayers[i]].EndLooting(false, obj.foodId);
        }
    }


    public void CatchFood(FoodBase food)
    {
        
        if (!players[LoginPlugin.Instance.PlayerId].CheckCanCatch())
        {
            return;
        }

        //todo:检测距离是否足够，足够才能catch
        var selfTrans = players[LoginPlugin.Instance.PlayerId].ObjTrans;
        if (selfTrans != null)
        {
            float distance = (food.transform.position - selfTrans.position).magnitude;
            if (distance > 3f)
            {
                // 如果FlyTextMgr使用的是屏幕坐标
                FlyTextMgr.Instance.AddTextAtScreenCenter("Distanc Over",  FlyTextType.Normal);
                return;
            }
        }


        var mgr = ClientMessageManager.Instance;
        Debug.Log("CatchFood");
        mgr.SendRequest(new CatchFoodRequest()
        {
            playerId = LoginPlugin.Instance.PlayerId,
            foodId = food.foodId
        });
    }

    private void OnCatchFoodNotify(CatchFoodNotify obj)
    {
        players[obj.playerId].CatchFood(obj.foodId, obj.isSuccess);
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
        else
        {
            if (players.ContainsKey(obj.playerId))
            {
                players[obj.playerId].EndCatch(); 
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
        //todo:刷新玩家的progress
        YOTOFramework.eventMgr.TriggerEvent<List<IntKeyFloatValue>>(YOTOEventType.RefreshProgress, obj.playerProgress);
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
    public void RotatePlayerRequest(Vector3 pos)
    {
        var pid = LoginPlugin.Instance.PlayerId;
        var mgr = ClientMessageManager.Instance;
        // Debug.Log("RotateRequest");
        mgr.SendRequest(new HeadPosRequest()
        {
            playerId = pid,
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
        if (LoginPlugin.Instance.PlayerId != obj.playerId)
        {
            //旋转眼球
            if (players.ContainsKey(obj.playerId))
            {
                // Debug.Log("Player:"+obj.playerId+" 旋转眼球"+obj.pos);
                players[obj.playerId].SetEyesMove(obj.pos);
            }
        }
    }

    #endregion

    #region 生成、移除player

    public void GeneratePlayers(List<PlayerData> playerDatas,bool isShow =false)
    {
        RemoveAllPlayers();
        for (var i = 0; i < playerDatas.Count; i++)
        {
            // playerDatas[i]
            var pp = GameObject.Find("p" + (i + 1).ToString());
            if (pp != null)
            {
                var p = PlayerEntity.pool.GetItem(playerDatas[i]);
                p.Location = pp.transform.position;
                p.isShow=isShow;
                p.InstanceGObj();
                players.Add(playerDatas[i].playerId, p);
            }
        }
    }

    private void OnPlayerPropertyNotify(PlayerPropertyNotify obj)
    {
        if (players.ContainsKey(obj.playerId))
        {
            players[obj.playerId].RefreshPlayerProperty(obj.satiety, obj.satisfaction);
        }
    }

    List<int> removeList = new List<int>();

    public void RefreshPlayers(List<PlayerData> datas)
    {
        removeList.Clear();
        foreach (var id in players.Keys)
        {
            bool isHave = false;
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

    

    
    private void OnFlyTextNotify(FlyTextNotify obj)
    {

        // 如果FlyTextMgr使用的是屏幕坐标
        FlyTextMgr.Instance.AddTextAtScreenCenter(obj.txt,  obj.flyType);
    }

    public void OnFindHostPlayerClick()
    {
        SomeOneFindHostPlayerRequest req = new SomeOneFindHostPlayerRequest();
        req.playerId = LoginPlugin.Instance.PlayerId;
        ClientMessageManager.Instance.SendRequest(req);
    }
    private void OnSomeOneFindHostPlayerNotifyt(SomeOneFindHostPlayerNotifyt obj)
    {
        YOTOFramework.uIMgr.Show(UIEnum.VotingPanel);
    }
    private void OnVotEndNotify(VotEndNotify obj)
    {
        YOTOFramework.eventMgr.TriggerEvent<VotEndNotify>(YOTOEventType.VotEndNotify,obj);

    }

    public void OnVotClick(int playerId)
    {
        VotRequest req = new VotRequest();
        req.playerId = LoginPlugin.Instance.PlayerId;
        req.votePlayerId = playerId;
        ClientMessageManager.Instance.SendRequest(req);
    }
}