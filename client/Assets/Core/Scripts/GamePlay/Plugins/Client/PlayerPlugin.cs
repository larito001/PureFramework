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
    
    }
    
    public void OnNetUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<HeadPosNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<CatchFoodNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<EndCatchFoodNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<StartLootNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<StopLootNotify>();
    }

    #region 食物操作
    
    
    private void OnStartLootNotify(StartLootNotify obj)
    {
        //todo:开抢，对应id进入特殊状态
        Debug.Log("开始抢夺");
        
    }
    private void OnStopLootNotify(StopLootNotify obj)
    {
        //todo:抢夺结束，退出特殊状态
        Debug.Log("抢夺结束，player"+obj.winPlayerId+"赢了");
        
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
            players[obj.playerId].EndCatch();
        }
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
                Debug.Log("Player:"+obj.playerId+" 旋转眼球"+obj.pos);
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
    
    public void RemoveAllPlayers()
    {
        
    }

    #endregion

    
}
