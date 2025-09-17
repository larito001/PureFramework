using System.Collections.Generic;
using UnityEngine;
using YOTO;


public class StagePlugin : LogicPluginBase
{
    // private List<FoodData> foodList = new List<FoodData>();

    private Dictionary<int, FoodEntity> foodDict = new Dictionary<int, FoodEntity>();

    // private List<PlayerData> players = new List<PlayerData>();
    private List<GameRule> rulesTemp ;
    public bool GameStart = false;
    
    #region 单例，事件注册

    public static StagePlugin Instance;

    public StagePlugin()
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
        ClientMessageManager.Instance.RegisterResponseHandler<FoodNotify>(OnFoodGenerateNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<RuleSelectNotify>(OnRuleSelectNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<GameTimerNotify>(OnGameTimerNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<FoodLootTimerNotify>(OnFoodLootTimerNotify);
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshRoleList, OnRefreshRoleList);
    }




    public void OnNetUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<FoodNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<RuleSelectNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<GameTimerNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<FoodLootTimerNotify>();
        YOTOFramework.eventMgr.RemoveEventListener(YOTOEventType.RefreshRoleList, OnRefreshRoleList);
    }

    #endregion

    #region 业务

    public List<GameRule> GetRules()
    {
        return rulesTemp;
    }
    private void OnFoodGenerateNotify(FoodNotify obj)
    {
        // foodList = obj.foodList;
        for (var i = 0; i < obj.foodList.Count; i++)
        {
            GenerateFoodsOnMap(obj.foodList[i]);
        }
    }

    private void GenerateFoodsOnMap(FoodData foodData)
    {
        var food = FoodEntity.pool.GetItem(foodData);
        foodDict.Add(foodData.foodId, food);
        var orgPos =GameObject.Find("FoodGeneratePos");
        food.Location = orgPos.transform.position+foodData.position;
        food.InstanceGObj();
    }

    public void RemoveFood(int foodId)
    {
        if (foodDict.ContainsKey(foodId))
        {
            FoodEntity.pool.RecoverItem(foodDict[foodId]);

            foodDict.Remove(foodId);
        }
    }
    public void RemoveAllFoods()
    {
        foreach (var food in foodDict.Values)
        {
            FoodEntity.pool.RecoverItem(food);
        }
        foodDict.Clear();
    }
    public FoodEntity GetFoodEntityById(int foodId)
    {
        if (foodDict.ContainsKey(foodId))
        {
            return foodDict[foodId];
        }

        return null;
    }

    private void OnRefreshRoleList()
    {
        PlayerPlugin.Instance.RefreshPlayers(LoginPlugin.Instance.GetPlayerDatas());
    }
    
    private void OnRuleSelectNotify(RuleSelectNotify obj)
    {
        if (obj.playerId==LoginPlugin.Instance.PlayerId)
        {
            rulesTemp = obj.rules;
            YOTOFramework.uIMgr.Show(UIEnum.RuleSelectPanel);
        }
    }
    public void SetRule(int ruleRuleId)
    {
        MainPlayerRuleSelectRequest request = new MainPlayerRuleSelectRequest();
        request.ruleId = ruleRuleId;
        ClientMessageManager.Instance.SendRequest(request);
    }
    #endregion

    #region 游戏生命周期

    public void OnGameStart()
    {
        GameStart = true;
        YOTOFramework.sceneMgr.LoadScene<NormalScene>();
    }

    public void OnGameError()
    {
        GameStart = false;
        YOTOFramework.sceneMgr.LoadScene<StartScene>();
    }

    public void OnGameEnd()
    {
        GameStart = false;

        YOTOFramework.sceneMgr.LoadScene<StartScene>();
        YOTOFramework.uIMgr.Show(UIEnum.RoomPanel);
    }
    private void OnGameTimerNotify(GameTimerNotify obj)
    {
        YOTOFramework.eventMgr.TriggerEvent<int>(YOTOEventType.GameTimerNotify, obj.index);
    }
    private void OnFoodLootTimerNotify(FoodLootTimerNotify obj)
    {
        YOTOFramework.eventMgr.TriggerEvent<int>(YOTOEventType.LootTimerNotify, obj.index);
    }
    #endregion



}