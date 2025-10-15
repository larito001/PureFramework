using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodData
{
    public static int idIndex = 1000;
    public string path = "testFood";
    public int foodId;
    public Vector3 position;
    private FoodState state;
    public int SatietyValue = 2; //饱腹值
    public int SatisfactionValue = 3; //满意度
    public Quality quality;
    List<int> playerIds = new List<int>();
    private const float LootOrgTimer = 5;

    // private Dictionary<int,PlayerData>playerIds = new Dictionary<int,PlayerData>();
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

    public void OnRemove()
    {
        foreach (var id in playerIds)
        {
            var info = ServerDataPlugin.Instance.GetPlayerById(id);
            if (info.OnCatchFoodEnd())
            {
                EndCatchFoodNotify notify = new EndCatchFoodNotify()
                {
                    foodId = foodId,
                    playerId = info.playerId,
                    isSuccess = false
                };
                ServerMessageManager.Instance.SendNotify(notify);
            }
            if (info.OnLootFoodEnd())
            {
                StopLootNotify notify = new StopLootNotify()
                {
                    foodId = foodId,
                    res = LootRes.Lose,
                    winPlayerId = id,
                    losePlayers = new List<int>()
                };
                ServerMessageManager.Instance.SendNotify(notify);
            }
  
  
        }

    }

    private void CheckPlayerIsAlive()
    {
        for (var i = playerIds.Count - 1; i >= 0; i--)
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
        else
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
                info.EatFood(SatietyValue, SatisfactionValue);
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
        LootIndex = (int)LootOrgTimer;
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
        var windata = ServerDataPlugin.Instance.GetPlayerById(maxId);
        windata.EatFood(SatietyValue, SatisfactionValue);
        StopLootNotify notify = new StopLootNotify()
        {
            foodId = foodId,
            winPlayerId = maxId,
            res = LootRes.Success,
            losePlayers = loseids
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    private int LootDelay = 1;
    private float lootTimer = 1;
    private int LootIndex = (int)LootOrgTimer;

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
            lootTimer -= dt;
            if (lootTimer <= 0)
            {
                lootTimer = 1;
                LootIndex--;
                FoodLootTimerNotify notify = new FoodLootTimerNotify()
                {
                    index = LootIndex
                };
                ServerMessageManager.Instance.SendNotify(notify);
            }

            if (timerTemp >= LootOrgTimer)
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
                playerProgress.Add(new IntKeyFloatValue(id, info.lootNum));
            }
        }

        return playerProgress;
    }
}