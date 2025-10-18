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
        new FoodDropPattern()
        {
            patternName = "正常",
            stages = new List<FoodDropStage>()
            {
                new FoodDropStage()
                {
                    startTime = 1,
                    endTime = 5,
                    dropCount = 1,
                    name = "TestFood",
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
                    dropCount = 1,
                    name = "TestFood",
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
                    endTime = 15,
                    dropCount = 1,
                    name = "TestFood",
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
                    startTime = 13,
                    endTime = 15,
                    dropCount = 1,
                    name = "TestFood",
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
                    startTime = 20,
                    endTime = 30,
                    dropCount = 1,
                    name = "TestFood",
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
                    startTime = 23,
                    endTime = 25,
                    dropCount = 1,
                    name = "TestFood",
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
                    startTime = 30,
                    endTime = 40,
                    dropCount = 2,
                    name = "TestFood",
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0.0f },
                        { Quality.Green, 0.0f },
                        { Quality.Blue, 0.5f },
                        { Quality.Purple, 0.0f },
                        { Quality.Glod, 0.0f },
                        { Quality.Red, 0.5f }
                    }
                }
            }
        },
        new FoodDropPattern()
        {
            patternName = "砸金蛋",
            stages = new List<FoodDropStage>()
            {
                new FoodDropStage()
                {
                    startTime = 0,
                    endTime = 1,
                    dropCount = 20,
                    name = "Egg",
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0f },
                        { Quality.Green, 0f },
                        { Quality.Blue, 0f },
                        { Quality.Purple, 0f },
                        { Quality.Glod, 1f },
                        { Quality.Red, 0f }
                    }
                },
            }
        },
        new FoodDropPattern()
        {
            patternName = "俄罗斯轮盘赌",
            stages = new List<FoodDropStage>()
            {
                new FoodDropStage()
                {
                    startTime = 0,
                    endTime = 1,
                    dropCount = 20,
                    name = "Egg",
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0f },
                        { Quality.Green, 0f },
                        { Quality.Blue, 0f },
                        { Quality.Purple, 0f },
                        { Quality.Glod, 0f },
                        { Quality.Red, 1f }
                    }
                },
            }
        },
        new FoodDropPattern()
        {
            patternName = "赌怪",
            stages = new List<FoodDropStage>()
            {
                new FoodDropStage()
                {
                    startTime = 0,
                    endTime = 1,
                    dropCount = 20,
                    name = "Egg",
                    gradeWeight = new Dictionary<Quality, float>()
                    {
                        { Quality.Normal, 0f },
                        { Quality.Green, 0f },
                        { Quality.Blue, 0f },
                        { Quality.Purple, 0f },
                        { Quality.Glod, 1f },
                        { Quality.Red, 0f }
                    }
                },
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
            roleName = "砸金蛋", //纯运气，运气
            roleDetail = "替换所有食物为鸡蛋，鸡蛋中有臭鸡蛋"
        },
        new GameRule()
        {
            ruleId = 2,
            roleName = "乡下吃席", //拼手速，力量
            roleDetail = "食物紧缺，得分最高获胜"
        },
        new GameRule()
        {
            ruleId = 3,
            roleName = "同学聚会", //两位数加减乘除法，智慧
            roleDetail = "抢夺时，使用加减乘除法获得分数"
        },
        new GameRule()
        {
            ruleId = 4,
            roleName = "商务局", //赎金,财富
            roleDetail = "输了可以使用金币赎身，避免一次喝酒"
        },
        new GameRule()
        {
            ruleId = 5,
            roleName = "俄罗斯轮盘赌", //掉落中有必死项，但概率极低,自信
            roleDetail = "掉落食物中有极小概率有剧毒"
        },
        new GameRule()
        {
            ruleId = 6,
            roleName = "赌怪", //高概率扣分，低概率获得极高分数,贪婪
            roleDetail = "掉落食物中有极小概率获得极高分数"
        },
    };

    public void SetCurrentRule(int id)
    {
        _currentRule = rules.First(x => x.ruleId == id);
        Debug.Log("设置规则" + _currentRule.roleName);
        UseRuleNotify notify = new UseRuleNotify();
        notify.rule = _currentRule;
        notify.playerId = _rulePlayerId;
        ServerMessageManager.Instance.SendNotify(notify);
    }

    public void SetRandomRule()
    {
        if (_currentRule == null)
        {
            SetCurrentRule(rules[UnityEngine.Random.Range(0, rules.Count)].ruleId);
        }
       
    }

    public void ResetRule()
    {
        _currentRule = null;
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
        switch (_currentRule.ruleId)
        {
            case 1:
                _currentPattern = allPatterns[1];
                break;
            case 2:
                _currentPattern = allPatterns[0];
                break;
            case 3:
                _currentPattern = allPatterns[0];
                break;
            case 4:
                _currentPattern = allPatterns[0];
                break;
            case 5:
                _currentPattern = allPatterns[2];
                break;
            case 6:
                _currentPattern = allPatterns[3];
                break;
        }

        // _currentPattern = allPatterns[];
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

    public bool RemovePlayerById(int id)
    {
        if (players.ContainsKey(id))
        {
            players.Remove(id);
            return true;
        }

        return false;
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