using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using YOTO;




#region 基类

// 请求基类
public interface IRequest : NetworkMessage
{
}

// 响应基类
public interface IResponse : NetworkMessage
{
}

#endregion

#region 角色

public struct PlayerPropertyNotify : IResponse
{
    public int playerId;
    public int satiety;
    public int satisfaction;
}

public struct PlayerDeadNotify : IResponse
{
    public int playerId;
}
public struct PlayerNeedDrinkNotify:IResponse
{
    public int playerId;
    public float currentRate;
}

#endregion

#region 食物

public enum FoodState
{
    Idle, //无
    Catching, //抓取中
    Looting, //抢夺中
    Discard, //丢弃
    Eat //吃掉
}

public enum Quality
{
    Normal = 0,
    Green,
    Blue,
    Purple,
    Glod,
    Red
}

public class FoodDropStage
{
    public float startTime;
    public float endTime;
    public float randomTime; //
    public int dropCount;
    public string name;
    public Dictionary<Quality, float> gradeWeight; // 品质权重（0~1）
}

public class FoodDropPattern
{
    public string patternName;
    public List<FoodDropStage> stages;
}

public struct CatchFoodRequest : IRequest
{
    public int foodId;
    public int playerId;
}

public struct CatchFoodNotify : IResponse
{
    public int foodId;
    public int playerId;
    public bool isSuccess;
}

public struct EndCatchFoodNotify : IResponse
{
    public int foodId;
    public int playerId;
    public bool isSuccess;
}

public struct StartLootNotify : IResponse
{
    public int foodId;
    public List<int> playerIds;
}

public enum LootRes
{
    Success, //胜利
    Dogfall, //平局
    Lose
}

public struct LootingInputRequest : IRequest
{
    public int playerId;
}

public struct IntKeyFloatValue
{
    public int key;
    public int value;

    public IntKeyFloatValue(int key, int value)
    {
        this.key = key;
        this.value = value;
    }
}

public struct LootingInputNotify : IResponse
{
    public List<IntKeyFloatValue> playerProgress;
}

public struct StopLootNotify : IResponse
{
    public int foodId;
    public LootRes res;
    public int winPlayerId;
    public List<int> losePlayers;
}

public struct FoodLootTimerNotify : IResponse
{
    public int index;
}

public struct FoodNotify : IResponse
{
    public List<FoodData> foodList;
}

#endregion

#region 随机事件（翻牌）

public class GameRule
{
    public int ruleId;
    public string roleName;
    public string roleDetail;
}

public struct RuleSelectNotify : IResponse
{
    public int playerId;
    public List<GameRule> rules;
}

public struct MainPlayerRuleSelectRequest : IRequest
{
    public int ruleId;
}

public struct SomeOneFindHostPlayerRequest : IRequest
{
    public int playerId;
}

public struct SomeOneFindHostPlayerNotifyt : IResponse
{
    public int playerId;
}

public struct VotRequest : IRequest
{
    public int playerId;
    public int votePlayerId;
}

public struct VotEndNotify : IResponse
{
    public List<Vector2Int> pidAndvots;
    public bool isSuccess;
}

#endregion

#region 登录

public struct LoginRequest : IRequest
{
    public string playerName;
}

public struct RefreshPlayerDatas : IResponse
{
    public List<PlayerData> playerDatas;
}

public struct GameStartRequest : IRequest
{
    public bool isSuccess;
}


public struct GameStartNotify : IResponse
{
    public bool isSuccess;
}
public struct PlayerLoadReadyRequest: IRequest
{
    public int playerId;
}
public struct GameEndNotify : IResponse
{
    public GameRule rule;
    public List<PlayerData> winPlayersDatas;
    public List<PlayerData> losePlayersDatas;
}

public struct GameTimerNotify : IResponse
{
    public int index;
}

// 例子：移动响应
public struct LoginResponse : IResponse
{
    public PlayerData playerData;
    public bool isSuccess;
}

public struct GotoRestNotify : IResponse
{
}

#endregion

#region 输入

public struct HeadPosRequest : IRequest
{
    public int playerId;
    public Vector3 pos;
}

public struct HeadPosNotify : IResponse
{
    public int playerId;
    public Vector3 pos;
}

#endregion

#region 通用

public struct FlyTextNotify : IResponse
{
    public string txt;
    public List<int> elsePlayers;
    public FlyTextType flyType;
}

#endregion