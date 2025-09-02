using System.Collections.Generic;
using UnityEngine;
using YOTO;


public class StagePlugin : LogicPluginBase
{
    public List<FoodData> foodList = new List<FoodData>();
    public Dictionary<int, FoodEntity> foodDict = new Dictionary<int, FoodEntity>();
    private int playerId = -1;
    private List<PlayerData> players = new List<PlayerData>();
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
        GameStart = false;
        ClientMessageManager.Instance.RegisterResponseHandler<FoodNotify>(OnFoodGenerateNotify);
    }

    public void OnNetUninstall()
    {
        GameStart = false;
        ClientMessageManager.Instance.UnRegisterResponseHandler<FoodNotify>();
    }

    #endregion
    
    #region 业务

    private void OnFoodGenerateNotify(FoodNotify obj)
    {
        foodList = obj.foodList;
        for (var i = 0; i < foodList.Count; i++)
        {
            GenerateFoodsOnMap(foodList[i]);
        }
    }

    private void GenerateFoodsOnMap(FoodData foodData)
    {
        var food = FoodEntity.pool.GetItem(foodData);
        foodDict.Add(foodData.foodId, food);
        food.Location = foodData.position;
        food.InstanceGObj();
    }

    public void RemoveFood(int foodId)
    {
        for (var i = 0; i < foodList.Count; i++)
        {
            if (foodList[i].foodId == foodId)
            {
                foodList.RemoveAt(i);
                FoodEntity.pool.RecoverItem(foodDict[foodId]);

                foodDict.Remove(foodId);
                break;
            }
        }
    }

    public FoodEntity GetFoodEntityById(int foodId)
    {
        if (foodDict.ContainsKey(foodId))
        {
            return foodDict[foodId];
        }
        
        return null;
    }

    #endregion

    #region 游戏生命周期

    public void OnGameStart(int playerId, List<PlayerData> playerDatas)
    {
        GameStart = true;
        YOTOFramework.uIMgr.ClearUI();
        this.playerId = playerId;
        players = playerDatas;
        PlayerPlugin.Instance.GeneratePlayers(players);
    }

    public void OnGameEnd()
    {
    }

    #endregion
}