
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using YOTO;

#region 基类

// 请求基类
public interface IRequest : NetworkMessage { }

// 响应基类
public interface IResponse : NetworkMessage { }

#endregion


#region 角色
public enum PlayerState
{
    Idle,//无
    Catching,//抓取中
    Looting,//抢夺中
    Backing,//返回中
}
public class PlayerData
{
    public int playerId;
    public string playerName;
    private PlayerState State=PlayerState.Idle;
    public bool OnCatchFood()
    {
        if (State == PlayerState.Idle)
        {
            State=PlayerState.Catching;
            return true;
        }

        return false;
    }
    public bool OnLootFood()
    {
        if (State == PlayerState.Idle)
        {
            State=PlayerState.Looting;
            return true;
        }

        return false;
    }

    public bool OnLootAfterCatch()
    {
        if (State == PlayerState.Catching)
        {
            State = PlayerState.Looting;
            return true;
        }

        return false;
    }
    public void OnCatchFoodEnd()
    {
        if (State == PlayerState.Catching)
        {
            State = PlayerState.Idle;  
        }
    }
}

#endregion

#region 食物


public enum FoodState
{
    Idle,//无
    Catching,//抓取中
    Looting,//抢夺中
    Discard,//丢弃
    Eat//吃掉
}
public class FoodData
{
    public int foodId;
    public Vector3 position;
    private FoodState state;
    private Dictionary<int,PlayerData>playerIds = new Dictionary<int,PlayerData>();
    private float timerTemp = 0;
    public FoodState GetState()
    {
        return state;
    }
    public void Init()
    {
        state = FoodState.Idle;
        playerIds.Clear();
    }
    public void StartCatch(PlayerData playerInfo)
    {
        if (state == FoodState.Idle&&playerInfo.OnCatchFood())
        {
            playerIds.Add(playerInfo.playerId,playerInfo);
            state = FoodState.Catching;
            timerTemp = 0;
            CatchFoodNotify notify = new CatchFoodNotify()
            {
                playerId =playerInfo.playerId,
                foodId = foodId,
                isSuccess = true
            };
            ServerMessageManager.Instance.SendNotify(notify); 
        }else if (state == FoodState.Catching&&playerInfo.OnLootFood())
        {
            //通知所有玩家开启抢夺
            foreach (var info in playerIds.Values)
            {
                info.OnLootAfterCatch();
            }
            //todo:广播玩家开抢
            Debug.Log("开抢");
        }
        else
        {
            Debug.Log("抓取失败");
        }
        
    }

    public void EndCatch()
    {
        state = FoodState.Eat;
        foreach (var info in playerIds.Values)
        {
            info.OnCatchFoodEnd();
        }
        EndCatchFoodNotify notify = new EndCatchFoodNotify()
        {
            foodId = foodId,
            playerId = playerIds.First().Key,
            isSuccess = true
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }


    public void Update(float dt)
    {
        if (state == FoodState.Catching)
        {
            timerTemp += dt;
            if (timerTemp >= 2)
            {
                timerTemp = 0;
                EndCatch();
            }
        }
    }
}


public struct CatchFoodRequest: IRequest
{
    public int foodId;
    public int playerId;
}
public struct CatchFoodResponse: IResponse
{
    public int foodId;
    public bool isSuccess;
}
public struct CatchFoodNotify:IResponse
{
    public int foodId;
    public int playerId;
    public bool isSuccess;
}
public struct EndCatchFoodNotify:IResponse
{
    public int foodId;
    public int playerId;
    public bool isSuccess;
}
public enum LootRes
{
    Success,//胜利
    Dogfall,//平局
}
public struct StartLootNotify:IResponse
{
    public int foodId;
    public List<int> playerIds;
}
public struct LootingInputRequest:IRequest
{
    public int foodId;
    public int playerId;
}
public struct LootingInputResponse:IResponse
{
    
}
public struct LootingInputNotify:IResponse
{
    public int foodId;
    private Dictionary<int, int> playerProgress;
}
public struct StopLootNotify:IResponse
{
    public int foodId;
    public LootRes res;
    public int winPlayerId;
}

public struct FoodNotify:IResponse
{
    public List<FoodData> foodList;
}

#endregion

#region 登录


public struct LoginRequest : IRequest
{
    public string playerName;
}
public struct LoginNotify : IResponse
{
    public List<PlayerData> playerDatas;
}
public struct GameStartRequest: IRequest
{
    public bool isSuccess;
}
public struct GameStartResponse: IResponse
{
    public bool isSuccess;
}
public struct GameStartNotify: IResponse
{
    public bool isSuccess;
}
// 例子：移动响应
public struct LoginResponse : IResponse
{
    public PlayerData playerData;
    public bool isSuccess;
}


#endregion

#region  输入

public struct HeadPosRequest: IRequest
{
    public int playerId;
    public Vector3 pos ;
}
public struct HeadPosNotify: IResponse
{
    public int playerId;
    public Vector3 pos;
}


#endregion
