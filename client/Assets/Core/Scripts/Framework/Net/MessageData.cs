
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

#region 基类

// 请求基类
public interface IRequest : NetworkMessage { }

// 响应基类
public interface IResponse : NetworkMessage { }

#endregion


#region 角色

public class PlayerData
{
    public int playerId;
    public string playerName;
    
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
    private Dictionary<int,int>playerIds = new Dictionary<int,int>();
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
    public void StartCatch(int playerId)
    {
        playerIds.Add(playerId,0);
        state = FoodState.Catching;
        timerTemp = 0;
        CatchFoodNotify notify = new CatchFoodNotify()
        {
            playerId =playerId,
            foodId = foodId,
            isSuccess = true
        };
        ServerMessageManager.Instance.SendNotify(notify);
        //todo:倒计时3秒转换状态
    }

    public void EndCatch()
    {
        state = FoodState.Eat;
    }
    public void StartLooting(int playerId)
    {
        playerIds.Add(playerId,0);
        state = FoodState.Looting;
        StartLootNotify notify = new StartLootNotify()
        {
            foodId = foodId,
            playerIds = playerIds.Values.ToList()
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }
    
    public void EndLooting()
    {
        var maxID = -1;
        var maxScore = -1;
        foreach (var keyValuePair in playerIds)
        {
            if (keyValuePair.Value >= maxScore)
            {
                maxScore=keyValuePair.Value;
                maxID=keyValuePair.Key;
            }
        }
        //todo:广播结算
    }

    public void StartDiscard()
    {
        state= FoodState.Discard;
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
        else if (state == FoodState.Looting)
        {
            timerTemp += dt;
            if (timerTemp >= 2)
            {
                timerTemp = 0;
                EndLooting();
            }
        }
    }
}

public struct RefreshFoodStateNotify:IResponse
{
    public List<FoodData> newDatas;
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
public struct HeadPosResponse: IResponse
{
}

#endregion
