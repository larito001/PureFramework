using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerDataPlugin : LogicPluginBase
{
    public static ServerDataPlugin Instance;

    public override void Init()
    {
        Instance = this;
    }

    private Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();
    private Dictionary<int, FoodData> foods = new Dictionary<int, FoodData>();

    private FoodDropPattern _currentPattern; //当前套路
    private GameRule _currentRule; //当前规则
    private int _rulePlayerId = -1; //当前host玩家id
    public bool hosterIsLose = false;

    public int RulePlayerId
    {
        get { return _rulePlayerId; }
        private set { _rulePlayerId = value; }
    }

    public GameRule CurrentRule
    {
        get { return _currentRule; }
        private set { _currentRule = value; }
    }

    public FoodDropPattern CurrentPattern
    {
        get { return _currentPattern; }
        private set { _currentPattern = value; }
    }

    #region 套路

    public List<FoodDropPattern> allPatterns = new List<FoodDropPattern>()
    {
        // T1 早饱陷阱
        new FoodDropPattern()
        {
            patternName = "T1_早饱陷阱",
            stages = new List<FoodDropStage>()
            {
                new FoodDropStage()
                {
                    startTime = 1,
                    endTime = 5,
                    dropCount = 3,
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0.7f },
                        { Quality.Green, 0.2f },
                        { Quality.Blue, 0.08f },
                        { Quality.Purple, 0.02f },
                        { Quality.Glod, 0.0f },
                        { Quality.Red, 0.0f }
                    }
                },
                new FoodDropStage()
                {
                    startTime = 6,
                    endTime = 10,
                    dropCount = 2,
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0.8f },
                        { Quality.Green, 0.15f },
                        { Quality.Blue, 0.05f },
                        { Quality.Purple, 0.0f },
                        { Quality.Glod, 0.0f },
                        { Quality.Red, 0.0f }
                    }
                },
                new FoodDropStage()
                {
                    startTime = 11,
                    endTime = 20,
                    dropCount = 4,
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0.2f },
                        { Quality.Green, 0.3f },
                        { Quality.Blue, 0.3f },
                        { Quality.Purple, 0.15f },
                        { Quality.Glod, 0.04f },
                        { Quality.Red, 0.01f }
                    }
                },
                new FoodDropStage()
                {
                    startTime = 20,
                    endTime = 50,
                    dropCount = 4,
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0.0f },
                        { Quality.Green, 0.0f },
                        { Quality.Blue, 0.0f },
                        { Quality.Purple, 0.0f },
                        { Quality.Glod, 0.0f },
                        { Quality.Red, 1f }
                    }
                },
                new FoodDropStage()
                {
                    startTime =50,
                    endTime = 80,
                    dropCount = 4,
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0.0f },
                        { Quality.Green, 0.0f },
                        { Quality.Blue, 0.0f },
                        { Quality.Purple, 0.0f },
                        { Quality.Glod, 0.0f },
                        { Quality.Red, 1f }
                    }
                }
                ,
                new FoodDropStage()
                {
                    startTime = 80,
                    endTime = 100,
                    dropCount = 4,
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0.0f },
                        { Quality.Green, 0.0f },
                        { Quality.Blue, 0.0f },
                        { Quality.Purple, 0.0f },
                        { Quality.Glod, 0.0f },
                        { Quality.Red, 1f }
                    }
                }
            }
        },
        
    };

    #endregion

    #region 可选规则

    private List<GameRule> rules = new List<GameRule>()
    {
        new GameRule()
        {
            ruleId = 1,
            roleName = "最大获胜",
            roleDetail = "得分最高者获胜"
        },
        new GameRule()
        {
            ruleId = 2,
            roleName = "最小获胜",
            roleDetail = "得分最少的获胜"
        },
        new GameRule()
        {
            ruleId = 3,
            roleName = "抢夺最多获胜",
            roleDetail = "抢夺最多次数的玩家获胜"
        },
        new GameRule()
        {
            ruleId = 4,
            roleName = "抢夺最少获胜",
            roleDetail = "抢夺最少次数的玩家获胜"
        },
        new GameRule()
        {
            ruleId = 5,
            roleName = "喝一口顶两口",
            roleDetail = "喝一口顶两口"
        },
        
    };

    public void SetCurrentRule(int id)
    {
        _currentRule = rules.First(x => x.ruleId == id);
        Debug.Log("设置规则" + _currentRule.roleName);
    }

    public void SetRandomRule()
    {
        SetCurrentRule(rules[UnityEngine.Random.Range(0, rules.Count)].ruleId);
    }

    public List<GameRule> getRandomRules(int num)
    {
        return rules.OrderBy(x => UnityEngine.Random.value).Take(num).ToList();
    }

    #endregion

    public int GetRandomPlayer()
    {
        return players.Keys.ToList()[UnityEngine.Random.Range(0, players.Count)];
    }

    public Quality RandomFood(FoodDropStage stage)
    {
        float rand = UnityEngine.Random.value; // 0~1
        float sum = 0f;
        foreach (var kv in stage.gradeWeight)
        {
            sum += kv.Value;
            if (rand <= sum) return kv.Key;
        }

        // 保底
        return stage.gradeWeight.Keys.Last();
    }

    public void SetRandomPattern()
    {
        _currentPattern = allPatterns[UnityEngine.Random.Range(0, allPatterns.Count)];
        Debug.Log("当前套路：" + _currentPattern.patternName);
    }

    public Dictionary<int, PlayerData>.ValueCollection GetPlayerList()
    {
        return players.Values;
    }

    public Dictionary<int, FoodData>.ValueCollection GetFoodList()
    {
        return foods.Values;
    }


    public PlayerData GetPlayerById(int id)
    {
        if (players.ContainsKey(id))
        {
            return players[id];
        }

        return null;
    }

    public void AddPlayer(PlayerData playerData)
    {
        players.Add(playerData.playerId, playerData);
    }

    public bool CheckHavePlayer(int id)
    {
        return players.ContainsKey(id);
    }

    public void RemovePlayerById(int id)
    {
        if (players.ContainsKey(id))
        {
            players.Remove(id);
        }
    }

    public void RemoveAllPlayers()
    {
        players.Clear();
    }

    public FoodData GetFoodById(int id)
    {
        if (foods.ContainsKey(id))
        {
            return foods[id];
        }

        return null;
    }

    public void AddFood(FoodData foodData)
    {
        foods.Add(foodData.foodId, foodData);
    }

    public bool CheckHaveFood(int id)
    {
        return foods.ContainsKey(id);
    }

    public void RemoveFoodById(int id)
    {
        if (foods.ContainsKey(id))
        {
            foods[id].OnRemove();
            foods.Remove(id);
        }
    }

    public void RemoveAllFoods()
    {
        foreach (var food in foods.Values)
        {
            food.OnRemove();
        }
        foods.Clear();
        
    }


    public void SetRulePlayerId(int playerId)
    {
        _rulePlayerId = playerId;
    }
}