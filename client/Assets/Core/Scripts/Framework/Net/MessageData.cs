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

public enum PlayerState
{
    Idle, //无
    Catching, //抓取中
    Looting, //抢夺中
    Backing, //返回中
}

public class PlayerData
{
    public int playerId;
    public string playerName;
    private PlayerState State = PlayerState.Idle;
    public int lootNum = 0;
    public int useFoodId = -1;

    public PlayerState GetState()
    {
        return State;
    }

    public bool OnCatchFood(int foodid)
    {
        if (State == PlayerState.Idle)
        {
            State = PlayerState.Catching;
            useFoodId = foodid;
            return true;
        }

        return false;
    }

    public bool OnLootFood(int foodid)
    {
        if (State == PlayerState.Idle)
        {
            State = PlayerState.Looting;
            useFoodId = foodid;
            return true;
        }

        return false;
    }

    public bool OnLootAfterCatch(int foodid)
    {
        if (State == PlayerState.Catching)
        {
            lootNum = 0;
            State = PlayerState.Looting;
            useFoodId = foodid;
            return true;
        }

        return false;
    }

    public bool AddLoot()
    {
        if (State == PlayerState.Looting)
        {
            lootNum++;
            return true;
        }

        return false;
    }

    public void OnCatchFoodEnd()
    {
        if (State == PlayerState.Catching)
        {
            State = PlayerState.Idle;
            useFoodId = -1;
        }
    }

    public void OnLootFoodEnd()
    {
        if (State == PlayerState.Looting)
        {
            State = PlayerState.Idle;
            useFoodId = -1;
        }
    }
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

public class FoodData
{
    public int foodId;
    public Vector3 position;
    private FoodState state;

    List<int> playerIds = new List<int>();

    // private Dictionary<int,PlayerData>playerIds = new Dictionary<int,PlayerData>();
    private float timerTemp = 0;
    private int lootNum = 10;

    public FoodState GetState()
    {
        return state;
    }

    public void Init()
    {
        state = FoodState.Idle;
        playerIds.Clear();
    }

    private void CheckPlayerIsAlive()
    {
        for (var i =  playerIds.Count-1; i >=0; i--)
        {
            if (!ServerDataPlugin.Instance.CheckHavePlayer(playerIds[i]))
            {
                playerIds.RemoveAt(i); 
            }
       
        }
    }

    public void StartCatch(PlayerData playerInfo)
    {
        CheckPlayerIsAlive();
        if (state == FoodState.Idle && playerInfo.OnCatchFood(foodId))
        {
            playerIds.Add(playerInfo.playerId);
            state = FoodState.Catching;
            timerTemp = 0;
            CatchFoodNotify notify = new CatchFoodNotify()
            {
                playerId = playerInfo.playerId,
                foodId = foodId,
                isSuccess = true
            };
            ServerMessageManager.Instance.SendNotify(notify);
        }
        else if (state == FoodState.Catching && playerInfo.OnLootFood(foodId))
        {
            playerIds.Add(playerInfo.playerId);
            state = FoodState.Looting;
            //通知所有玩家开启抢夺
            foreach (var id in playerIds)
            {
                var info = ServerDataPlugin.Instance.GetPlayerById(id);
                if (info != null)
                {
                    info.OnLootAfterCatch(foodId);
                }
            }

            StartLootNotify notify = new StartLootNotify()
            {
                foodId = foodId,
                playerIds = playerIds
            };
            ServerMessageManager.Instance.SendNotify(notify);
            //todo:广播玩家开抢
            Debug.Log("开抢");
            timerTemp = 0;
        }
        else if (playerInfo.GetState() == PlayerState.Idle)
        {
            Debug.Log("抓取失败");
            CatchFoodNotify notify = new CatchFoodNotify()
            {
                playerId = playerInfo.playerId,
                foodId = foodId,
                isSuccess = false
            };
            ServerMessageManager.Instance.SendNotify(notify);
        }
    }

    public void EndCatch()
    {
        CheckPlayerIsAlive();
        state = FoodState.Eat;
        foreach (var id in playerIds)
        {
            var info = ServerDataPlugin.Instance.GetPlayerById(id);
            if (info != null)
            {
                info.OnCatchFoodEnd();
            }
        }

        int tempId = -1;
        if (playerIds.Count > 0)
        {
            tempId = playerIds.First();
        }
        EndCatchFoodNotify notify = new EndCatchFoodNotify()
        {
            foodId = foodId,
            playerId = tempId,
            isSuccess = true
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }


    private void EndLoot()
    {
        CheckPlayerIsAlive();
        state = FoodState.Eat;
        int maxId = -1;
        int maxNum = -1;
        foreach (var id in playerIds)
        {
            var info = ServerDataPlugin.Instance.GetPlayerById(id);
            if (info != null)
            {
                info.OnLootFoodEnd();
            }
        }


        List<int> loseids = new List<int>();
        foreach (var id in playerIds)
        {
            var data = ServerDataPlugin.Instance.GetPlayerById(id);
            if (data != null)
            {
                if (data.lootNum >= maxNum)
                {
                    maxId = data.playerId;
                    maxNum = data.lootNum;
                }

                loseids.Add(data.playerId);
            }
        }

        loseids.Remove(maxId);
        StopLootNotify notify = new StopLootNotify()
        {
            foodId = foodId,
            winPlayerId = maxId,
            res = LootRes.Success,
            losePlayers = loseids
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    public void Update(float dt)
    {
        if (state == FoodState.Catching)
        {
            timerTemp += dt;
            if (timerTemp >= 3)
            {
                timerTemp = 0;
                EndCatch();
            }
        }

        if (state == FoodState.Looting)
        {
            timerTemp += dt;
            if (timerTemp >= 20)
            {
                timerTemp = 0;
                EndLoot();
            }
        }
    }

    private List<IntKeyFloatValue> playerProgress = new List<IntKeyFloatValue>();

    public List<IntKeyFloatValue> GetProgress()
    {
        playerProgress.Clear();

        foreach (var id in playerIds)
        {
            var info = ServerDataPlugin.Instance.GetPlayerById(id);
            if (info != null)
            {
                float progress = info.lootNum /(float) lootNum;
                playerProgress.Add(new IntKeyFloatValue(id, progress));
            }
    
        }

        return playerProgress;
    }
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
}

public struct LootingInputRequest : IRequest
{
    public int playerId;
}

public struct IntKeyFloatValue
{
    public int key;
    public float value;

    public IntKeyFloatValue(int key, float value)
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

public struct FoodNotify : IResponse
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

public struct GameStartRequest : IRequest
{
    public bool isSuccess;
}

public struct GameStartResponse : IResponse
{
    public bool isSuccess;
}

public struct GameStartNotify : IResponse
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