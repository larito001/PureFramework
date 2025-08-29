
using System.Collections;
using System.Collections.Generic;
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
    Eat,//被食用
}
public class FoodData
{
    public int foodId;
    public Vector3 position;
    public FoodState state; 
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
